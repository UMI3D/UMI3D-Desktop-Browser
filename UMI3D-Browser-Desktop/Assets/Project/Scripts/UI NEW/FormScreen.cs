using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Label = UnityEngine.UIElements.Label;

public class FormScreen : BaseScreen
{
    public FormScreen(VisualElement element) : base(element)
    {

    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto form, Action<FormAnswerDto> callback)
    {
        if (form == null)
        {
            callback.Invoke(null);
            return;
        }

        FormAnswerDto answer = new FormAnswerDto()
        {
            boneType = 0,
            hoveredObjectId = 0,
            id = form.Id,
            toolId = 0,
            answers = new List<ParameterSettingRequestDto>()
        };

        _root.Add(GetVisualElements(form, answer));
    }

    private VisualElement GetVisualElements(umi3d.common.interaction.form.FormDto form, FormAnswerDto to)
    {
        var formElement = new VisualElement() { name = "form" };
        if (form.Pages.Count == 1)
        {
            formElement = GetGroupVisualElement(form.Pages[0].Group);
        }
        else
        {
            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.AddToClassList("menu-navigation");
            formElement.Add(radioButtonGroup);

            for (int i = 0; i < form.Pages.Count; i++)
            {
                PageDto page = form.Pages[i];
                var pageView = new VisualElement() { name = page.Name };
                pageView.Add(GetGroupVisualElement(page.Group));
                formElement.Add(pageView);

                var radioButton = new RadioButton(page.Name);
                radioButton.RegisterValueChangedCallback(e =>
                {
                    if (e.newValue)
                        pageView.RemoveFromClassList("hidden");
                    else
                        pageView.AddToClassList("hidden");
                });
                radioButtonGroup.Add(radioButton);

                if (i == 0)
                    radioButton.value = true;
                else
                    pageView.AddToClassList("hidden");

            }
        }
        return formElement;
    }

    private VisualElement GetGroupVisualElement(GroupDto group)
    {
        if (group == null) return new VisualElement() { name = "Group null" };
        if (group.Children == null) return new VisualElement() { name = "Group Empty" };

        var result = new VisualElement();
        foreach (var div in group.Children)
        {
            // Label
            var label = div as LabelDto;
            if (label != null)
            {
                result.Add(new Label(label.Text));
                continue;
            }

            // Group
            var childGroup = div as GroupDto;
            if (childGroup != null)
                result.Add(GetGroupVisualElement(childGroup));

            // Inputs
            switch (div)
            {
                case TextDto text:
                    var textElement = new TextField(text.Label);
                    textElement.value = text.Value;
                    textElement.SetPlaceholderText(text.PlaceHolder);
                    textElement.isPasswordField = text.Type == TextType.Password;
                    result.Add(textElement);
                    break;
                case ButtonDto button:
                    var buttonElement = new Button();
                    buttonElement.text = button.Label;
                    result.Add(buttonElement);
                    break;
                case RangeDto<int> rangeInt:
                    break;
                case RangeDto<float> rangeFloat:
                    break;
                default:
                    break;
            }
        }

        return result;
    }
}