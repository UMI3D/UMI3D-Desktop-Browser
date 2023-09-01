using inetum.unityUtils;
using System;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UIElements;

public class FormScreen : BaseScreen
{
    private RememberForm m_RemeberForm = new RememberForm();

    public FormScreen(VisualElement pRoot) : base(pRoot)
    {
    }

    public void GetParameterDtos(umi3d.common.interaction.form.FormDto pForm, Action<FormAnswerDto> pCallback)
    {
        if (pForm == null)
        {
            pCallback.Invoke(null);
            return;
        }
        m_Root.Clear();
        m_Root.Add(GetVisualElements(pForm, pCallback));
    }

    private VisualElement GetVisualElements(umi3d.common.interaction.form.FormDto pForm, Action<FormAnswerDto> pCallback)
    {
        var formElement = new VisualElement() { name = "form" };
        if (pForm.Pages.Count == 1)
        {
            var answers = CreateFormAnswer(pForm);
            formElement = GetGroupVisualElement(pForm.Pages[0].Group, answers, pCallback);
        }
        else
        {
            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.AddToClassList("menu-navigation");
            formElement.Add(radioButtonGroup);

            for (int i = 0; i < pForm.Pages.Count; i++)
            {
                PageDto page = pForm.Pages[i];
                var answers = CreateFormAnswer(pForm);
                var pageView = new VisualElement() { name = page.Name };
                pageView.Add(GetGroupVisualElement(page.Group, answers, pCallback));
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

    private static FormAnswerDto CreateFormAnswer(umi3d.common.interaction.form.FormDto pForm)
    {
        return new FormAnswerDto()
        {
            boneType = 0,
            hoveredObjectId = 0,
            id = pForm.Id,
            toolId = 0,
            answers = new List<ParameterSettingRequestDto>()
        };
    }

    private VisualElement GetGroupVisualElement(GroupDto pGroup, FormAnswerDto pAnswer, Action<FormAnswerDto> pCallback)
    {
        if (pGroup == null) return new VisualElement() { name = "Group null" };
        if (pGroup.Children == null) return new VisualElement() { name = "Group Empty" };

        VisualElement result = CreateGroup(pGroup, pAnswer, pCallback);

        foreach (var div in pGroup.Children)
        {
            result.Add(CreateGroupElement(div, pAnswer, pCallback));
        }

        if (pGroup.CanRemember)
        {
            AddRememberMe(result, pAnswer, pCallback);
        }

        return result;
    }

    private VisualElement CreateGroup(GroupDto pGroup, FormAnswerDto pAnswer, Action<FormAnswerDto> pCallback)
    {
        var result = new VisualElement();
        switch (pGroup)
        {
            case GroupScrollViewDto scrollView:
                var scrollViewElement = new ScrollView();
                scrollViewElement.mode = scrollView.Mode;
                result = scrollViewElement;
                break;
            default:
                break;
        }
        if (pGroup.SubmitOnValidate)
            result.RegisterCallback<ClickEvent>(e =>
            {
                Debug.Log(m_IsAButtonAlreadyPressed);
                if (!m_IsAButtonAlreadyPressed)
                {
                    ButtonActivated();
                    pCallback?.Invoke(pAnswer);
                }
            });

        return result;
    }

    private VisualElement CreateGroupElement(DivDto pDiv, FormAnswerDto pAnswers, Action<FormAnswerDto> pCallback)
    {
        var result = new VisualElement();
        switch (pDiv)
        {
            case GroupDto childGroup:
                result.Add(GetGroupVisualElement(childGroup, pAnswers, pCallback));
                break;
            case LabelDto label:
                var labelElement = new Label(label.Text);
                result = labelElement;
                SetServerStyle(labelElement, label.Styles);
                break;
            case ImageDto image:
                if (image.Resource == null) break;
                if (image.Resource.variants == null) break;
                if (image.Resource.variants.Count == 0) break;
                SetImage(image, result);
                SetServerStyle(result, image.Styles);
                break;
            case BaseInputDto baseInput:
                result = CreateInputElement(baseInput, pAnswers, pCallback);
                break;
            default:
                break;
        }
        return result;
    }

    private void AddRememberMe(VisualElement pResult, FormAnswerDto pAnswers, Action<FormAnswerDto> pCallback)
    {
        var rememberElement = new ToggleButton_C();
        rememberElement.label = "Remember me!";

        var submit = pResult.Q<Button>(className: "submit");
        if (submit != null)
        {
            submit.clicked += () =>
            {
                if (!rememberElement.value) return;
                m_RemeberForm.SaveAnswer(pAnswers);
            };
            pResult.Insert(pResult.IndexOf(submit), rememberElement);
        }

        var rememberedAnswer = m_RemeberForm.GetAnswer(pAnswers);
        if (rememberedAnswer != null)
        {
            pCallback?.Invoke(rememberedAnswer);
        }
    }

    private static async void SetImage(ImageDto pImage, VisualElement pElement)
    {
        FileDto fileToLoad = UMI3DEnvironmentLoader.AbstractParameters.ChooseVariant(pImage.Resource.variants);
        IResourcesLoader loader = UMI3DEnvironmentLoader.AbstractParameters.SelectLoader(fileToLoad.extension);
        if (loader == null) return;

        var fileLoaded = await UMI3DResourcesManager.LoadFile(pImage.Id, fileToLoad, loader);
        var texture = fileLoaded as Texture2D;
        if (texture == null) return;

        pElement.style.backgroundImage = texture;
    }

    private VisualElement CreateInputElement(BaseInputDto pBaseInput, FormAnswerDto pAnswers, Action<FormAnswerDto> pCallback)
    {
        VisualElement result = new VisualElement();
        var requestDto = new ParameterSettingRequestDto()
        {
            toolId = pBaseInput.Id,
            id = pBaseInput.Id,
            parameter = pBaseInput.GetValue()
        };
        switch (pBaseInput)
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
                    buttonElement.AddToClassList("submit");
                    if (!m_IsAButtonAlreadyPressed)
                    {
                        ButtonActivated();
                        buttonElement.clicked += () =>
                        {
                            pCallback?.Invoke(pAnswers);
                        };
                    }
                } else
                {
                    if (!m_IsAButtonAlreadyPressed)
                    {
                        ButtonActivated();
                        buttonElement.clicked += () => Debug.LogWarning("Not Implemented");
                    }
                }
                result = buttonElement;
                break;
            case RangeDto<int> rangeInt:
                var sliderInt = new SliderInt_C();
                sliderInt.label = rangeInt.Label;
                sliderInt.lowValue = rangeInt.Min;
                sliderInt.highValue = rangeInt.Max;
                sliderInt.value = rangeInt.Value;
                sliderInt.RegisterValueChangedCallback(e =>
                {
                    requestDto.parameter = e.newValue;
                });
                result = sliderInt;
                break;
            case RangeDto<float> rangeFloat:
                var sliderFloat = new SliderFloat_C();
                sliderFloat.label = rangeFloat.Label;
                sliderFloat.lowValue = rangeFloat.Min;
                sliderFloat.highValue = rangeFloat.Max;
                sliderFloat.value = rangeFloat.Value;
                sliderFloat.RegisterValueChangedCallback(e =>
                {
                    requestDto.parameter = e.newValue;
                });
                result = sliderFloat;
                break;
            default:
                break;
        }
        if (pBaseInput.Tooltip != null) 
        {
            result.RegisterCallback<MouseEnterEvent>(e => m_Tooltip.Show(result, pBaseInput.Tooltip));
            result.RegisterCallback<MouseLeaveEvent>(e => m_Tooltip.Hide());
        }

        pAnswers.answers.Add(requestDto);
        return result;
    }

    private void SetServerStyle(VisualElement pElement, List<StyleDto> pStyles) 
    {
        if (pStyles == null) return;

        foreach (var style in pStyles)
        {
            switch (style.Variants.Find(v => v.DeviceType.HasFlag(umi3d.common.interaction.form.DeviceType.Screen)))
            {
                case FlexStyleDto flex:
                    pElement.style.flexDirection = flex.Direction;
                    break;
                case PositionStyleDto position:
                    pElement.style.position = position.Position;
                    pElement.style.top = position.Top;
                    pElement.style.bottom = position.Bottom;
                    pElement.style.right = position.Right;
                    pElement.style.left = position.Left;
                    break;
                case SizeStyleDto size:
                    pElement.style.width = size.Width;
                    pElement.style.height = size.Height;
                    break;
                default:
                    break;
            }
        }
    }
}