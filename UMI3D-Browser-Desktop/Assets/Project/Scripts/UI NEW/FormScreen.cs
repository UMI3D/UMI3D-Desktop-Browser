using GLTFast.Schema;
using inetum.unityUtils;
using Mono.Cecil;
using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.interaction.form;
using UnityEngine;
using UnityEngine.UIElements;

public class FormScreen : BaseScreen
{
    private const int k_buttonCooldown = 1500;
    private bool m_IsAButtonAlreadyPressed;

    private RememberForm m_RemeberForm = new RememberForm();

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
        m_Root.Clear();
        m_Root.Add(GetVisualElements(form, callback));
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

        VisualElement result = CreateGroup(group, answers, callback);

        foreach (var div in group.Children)
        {
            result.Add(CreateGroupElement(div, answers, callback));
        }

        if (group.CanRemember)
        {
            AddRememberMe(result, answers, callback);
        }

        return result;
    }

    private VisualElement CreateGroup(GroupDto group, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        var result = new VisualElement();
        switch (group)
        {
            case GroupScrollViewDto scrollView:
                var scrollViewElement = new ScrollView();
                scrollViewElement.mode = scrollView.Mode;
                result = scrollViewElement;
                break;
            default:
                break;
        }
        if (group.SubmitOnValidate)
            result.RegisterCallback<ClickEvent>(e =>
            {
                Debug.Log(m_IsAButtonAlreadyPressed);
                if (!m_IsAButtonAlreadyPressed)
                {
                    ButtonActivated();
                    callback?.Invoke(answers);
                }
            });

        return result;
    }

    private VisualElement CreateGroupElement(DivDto div, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        var result = new VisualElement();
        switch (div)
        {
            case GroupDto childGroup:
                result.Add(GetGroupVisualElement(childGroup, answers, callback));
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
                result = CreateInputElement(baseInput, answers, callback);
                break;
            default:
                break;
        }
        return result;
    }

    private void AddRememberMe(VisualElement result, FormAnswerDto answers, Action<FormAnswerDto> callback)
    {
        var rememberElement = new ToggleButton_C();
        rememberElement.label = "Remember me!";

        var submit = result.Q<Button>(className: "submit");
        if (submit != null)
        {
            submit.clicked += () =>
            {
                if (!rememberElement.value) return;
                m_RemeberForm.SaveAnswer(answers);
            };
            result.Insert(result.IndexOf(submit), rememberElement);
        }

        var rememberedAnswer = m_RemeberForm.GetAnswer(answers);
        if (rememberedAnswer != null)
        {
            callback?.Invoke(rememberedAnswer);
        }
    }

    private static async void SetImage(ImageDto image, VisualElement element)
    {
        FileDto fileToLoad = UMI3DEnvironmentLoader.AbstractParameters.ChooseVariant(image.Resource.variants);
        IResourcesLoader loader = UMI3DEnvironmentLoader.AbstractParameters.SelectLoader(fileToLoad.extension);
        if (loader == null) return;

        var fileLoaded = await UMI3DResourcesManager.LoadFile(image.Id, fileToLoad, loader);
        var texture = fileLoaded as Texture2D;
        if (texture == null) return;

        element.style.backgroundImage = texture;
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
                    buttonElement.AddToClassList("submit");
                    if (!m_IsAButtonAlreadyPressed)
                    {
                        ButtonActivated();
                        buttonElement.clicked += () =>
                        {
                            callback?.Invoke(answers);
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
                break;
            case RangeDto<float> rangeFloat:
                break;
            default:
                break;
        }
        if (baseInput.Tooltip != null) 
        {
            result.RegisterCallback<MouseEnterEvent>(e =>
            {
                ShowTooltip(result, baseInput.Tooltip);
            });
            result.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideTooltip();
            });
        }

        answers.answers.Add(requestDto);
        return result;
    }

    private void SetServerStyle(VisualElement element, List<StyleDto> styles) 
    {
        if (styles == null) return;

        foreach (var style in styles)
        {
            switch (style.Variants.Find(v => v.DeviceType.HasFlag(umi3d.common.interaction.form.DeviceType.Screen)))
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
                case SizeStyleDto size:
                    element.style.width = size.Width;
                    element.style.height = size.Height;
                    break;
                default:
                    break;
            }
        }
    }

    private async void ButtonActivated()
    {
        m_IsAButtonAlreadyPressed = true;
        await Task.Delay(k_buttonCooldown);
        m_IsAButtonAlreadyPressed = false;
    }
}