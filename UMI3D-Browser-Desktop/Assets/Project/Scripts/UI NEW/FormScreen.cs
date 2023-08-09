using GLTFast.Schema;
using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UIElements;

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
        _root.Clear();
        _root.Add(GetVisualElements(form, callback));
    }

    private VisualElement GetVisualElements(umi3d.common.interaction.form.FormDto form, Action<FormAnswerDto> callback)
    {
        var formElement = new VisualElement() { name = "form" };
        if (form.Pages.Count == 1)
        {
            var answers = CreateFormAnswer(form);
            formElement = GetGroupVisualElement(form.Pages[0].Group, answers, callback);
        }
        else
        {
            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.AddToClassList("menu-navigation");
            formElement.Add(radioButtonGroup);

            for (int i = 0; i < form.Pages.Count; i++)
            {
                PageDto page = form.Pages[i];
                var answers = CreateFormAnswer(form);
                var pageView = new VisualElement() { name = page.Name };
                pageView.Add(GetGroupVisualElement(page.Group, answers, callback));
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

    private static FormAnswerDto CreateFormAnswer(umi3d.common.interaction.form.FormDto form)
    {
        return new FormAnswerDto()
        {
            boneType = 0,
            hoveredObjectId = 0,
            id = form.Id,
            toolId = 0,
            answers = new List<ParameterSettingRequestDto>()
        };
    }

    private VisualElement GetGroupVisualElement(GroupDto group, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        if (group == null) return new VisualElement() { name = "Group null" };
        if (group.Children == null) return new VisualElement() { name = "Group Empty" };

        var result = new VisualElement();
        foreach (var div in group.Children)
        {
            // Label
            if (div is LabelDto label)
            {
                result.Add(new Label(label.Text));
                continue;
            }

            // Group
            if (div is GroupDto childGroup)
                result.Add(GetGroupVisualElement(childGroup, answers, callback));

            // Inputs
            if (div is BaseInputDto baseInputDto)
            {
                var requestDto = new ParameterSettingRequestDto()
                {
                    toolId = baseInputDto.Id,
                    id = baseInputDto.Id,
                    parameter = baseInputDto.GetValue()
                };
                switch (baseInputDto)
                {
                    case TextDto text:
                        // Field
                        var textElement = new TextField(text.Label);
                        textElement.value = text.Value;
                        textElement.SetPlaceholderText(text.PlaceHolder);
                        textElement.isPasswordField = text.Type == TextType.Password;
                        result.Add(textElement);
                        // Answer
                        textElement.RegisterValueChangedCallback(e =>
                        {
                            requestDto.parameter = text.Value;
                        });
                        break;
                    case ButtonDto button:
                        var buttonElement = new Button
                        {
                            text = button.Label
                        };
                        result.Add(buttonElement);
                        if (button.Type == umi3d.common.interaction.form.ButtonType.Submit)
                        {
                            buttonElement.clicked += () => callback(answers);
                            break;
                        }
                        break;
                    case RangeDto<int> rangeInt:
                        break;
                    case RangeDto<float> rangeFloat:
                        break;
                    default:
                        break;
                }
                answers.answers.Add(requestDto);
            }
        }

        return result;
    }
}