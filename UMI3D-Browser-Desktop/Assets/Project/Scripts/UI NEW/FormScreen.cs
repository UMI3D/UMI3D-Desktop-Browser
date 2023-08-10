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

        VisualElement result = CreateGroupe(group);

        foreach (var div in group.Children)
        {
            result.Add(CreateGroupElement(div, answers, callback));
        }

        return result;
    }

    private static VisualElement CreateGroupe(GroupDto group)
    {
        switch (group)
        {
            case GroupScrollViewDto scrollView:
                var scrollViewElement = new ScrollView();
                scrollViewElement.mode = scrollView.Mode;
                return scrollViewElement;
            default:
                return new VisualElement();
        }
    }

    private VisualElement CreateGroupElement(DivDto div, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        var result = new VisualElement();
        switch (div)
        {
            case LabelDto label:
                var labelElement = new Label(label.Text);
                result = labelElement;
                SetServerStyle(labelElement, label.Styles);
                break;
            case GroupDto childGroup:
                result.Add(GetGroupVisualElement(childGroup, answers, callback));
                break;
            case BaseInputDto baseInput:
                result = CreateInputElement(baseInput, answers, callback);
                break;
            default:
                break;
        }
        return result;
    }

    private VisualElement CreateInputElement(BaseInputDto baseInput, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        VisualElement result = new VisualElement();
        var requestDto = new ParameterSettingRequestDto()
        {
            toolId = baseInput.Id,
            id = baseInput.Id,
            parameter = baseInput.GetValue()
        };
        switch (baseInput)
        {
            case TextDto text:
                // Field
                var textElement = new TextField(text.Label);
                SetServerStyle(textElement, text.Styles);
                textElement.value = text.Value;
                textElement.SetPlaceholderText(text.PlaceHolder);
                textElement.isPasswordField = text.Type == TextType.Password;
                result = textElement;
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
                SetServerStyle(buttonElement, button.Styles);
                if (button.Type == umi3d.common.interaction.form.ButtonType.Submit)
                {
                    buttonElement.clicked += () => callback(answers);
                    break;
                }
                result = buttonElement;
                break;
            case RangeDto<int> rangeInt:
                break;
            case RangeDto<float> rangeFloat:
                break;
            default:
                break;
        }

        answers.answers.Add(requestDto);
        return result;
    }

    private void SetServerStyle(VisualElement element, List<StyleDto> styles) 
    {
        if (styles == null) return;

        foreach (var style in styles)
        {
            switch (style)
            {
                case FlexStyleDto flex:
                    element.style.flexDirection = flex.Direction;
                    break;
                case PositionStyleDto position:
                    element.style.position = position.Position;
                    element.style.top = position.Top;
                    element.style.bottom = position.Bottom;
                    element.style.right = position.Right;
                    element.style.left = position.Left;
                    break;
                default:
                    break;
            }
        }
    }
}