/*
Copyright 2019 - 2022 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections.Generic;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsController : CustomSettingScreen
{
    public struct KeyBindingDisplayer
    {
        public CustomText Command;
        public CustomButton Key1;
        public CustomButton Key2;
        public VisualElement Box;

        public KeyBindingDisplayer(string command, string key1, string key2) : this(command)
        {
            Key1.text = key1;
            Key1.text = key2;
        }
        public KeyBindingDisplayer(string command)
        {
            Command = CreateText();
            Command.text = command;
            Command.Color = TextColor.Menu;

            Key1 = CreateButton();
            Key1.Size = ElementSize.Small;
            Key2 = CreateButton();
            Key2.Size = ElementSize.Small;

            Box = new VisualElement { name = "box-displayer" };
            Box.AddToClassList(USSCustomClassBox());
            Box.Add(Command);
            Box.Add(Key1);
            Box.Add(Key2);
        }

        public static System.Func<CustomText> CreateText;
        public static System.Func<CustomButton> CreateButton;
        public static System.Func<string> USSCustomClassBox;
    }

    public new class UxmlTraits : CustomSettingScreen.UxmlTraits
    {
        protected UxmlEnumAttributeDescription<ControllerEnum> m_controller = new UxmlEnumAttributeDescription<ControllerEnum>
        {
            name = "controller"
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomSettingsController;

            custom.Set();
        }
    }

    /// <summary>
    /// Current controller used with this browser.
    /// </summary>
    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            void AddNavigationControls()
            {
                ScrollView.Add(NavigationLabel);
                ScrollView.Add(Forward.Box);
                ScrollView.Add(Backward.Box);
                ScrollView.Add(Left.Box);
                ScrollView.Add(Right.Box);
                ScrollView.Add(Sprint.Box);
                ScrollView.Add(Jump.Box);
                ScrollView.Add(Crouch.Box);
                ScrollView.Add(FreeHead.Box);
            }
            void RemoveNavigationControls()
            {
                NavigationLabel.RemoveFromHierarchy();
                Forward.Box.RemoveFromHierarchy();
                Backward.Box.RemoveFromHierarchy();
                Left.Box.RemoveFromHierarchy();
                Right.Box.RemoveFromHierarchy();
                Sprint.Box.RemoveFromHierarchy();
                Jump.Box.RemoveFromHierarchy();
                Crouch.Box.RemoveFromHierarchy();
                FreeHead.Box.RemoveFromHierarchy();
            }

            void AddShortcutControls()
            {
                ScrollView.Add(ShortcutLabel);
                ScrollView.Add(Mute.Box);
                ScrollView.Add(PushToTalk.Box);
                ScrollView.Add(GeneralVolume.Box);
                ScrollView.Add(DecreaseVolume.Box);
                ScrollView.Add(IncreaseVolume.Box);
                ScrollView.Add(Cancel.Box);
                ScrollView.Add(Submit.Box);
                ScrollView.Add(GameMenu.Box);
                ScrollView.Add(ContextualMenu.Box);
                ScrollView.Add(Notification.Box);
                ScrollView.Add(UserList.Box);
            }
            void RemoveShortcutControls()
            {
                ShortcutLabel.RemoveFromHierarchy();
                Mute.Box.RemoveFromHierarchy();
                PushToTalk.Box.RemoveFromHierarchy();
                GeneralVolume.Box.RemoveFromHierarchy();
                DecreaseVolume.Box.RemoveFromHierarchy();
                IncreaseVolume.Box.RemoveFromHierarchy();
                Cancel.Box.RemoveFromHierarchy();
                Submit.Box.RemoveFromHierarchy();
                GameMenu.Box.RemoveFromHierarchy();
                ContextualMenu.Box.RemoveFromHierarchy();
                Notification.Box.RemoveFromHierarchy();
                UserList.Box.RemoveFromHierarchy();
            }

            m_controller = value;
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    AddNavigationControls();
                    AddShortcutControls();
                    break;
                case ControllerEnum.Touch:
                    ScrollView.Add(JoystickStaticToggle);
                    ScrollView.Add(LeftHandToggle);
                    RemoveNavigationControls();
                    RemoveShortcutControls();
                    break;
                case ControllerEnum.GameController:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    AddNavigationControls();
                    AddShortcutControls();
                    break;
                default:
                    break;
            }
        }
    }

    public override string USSCustomClassName => "setting-controller";
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box";
    public virtual string USSCustomClassControlsSection => $"{USSCustomClassName}-controls-section";

    public CustomSlider CamreraSensibility;

    public CustomToggle JoystickStaticToggle;
    public CustomToggle LeftHandToggle;

    #region Navigation key

    public CustomText NavigationLabel;
    public KeyBindingDisplayer Forward;
    public KeyBindingDisplayer Backward;
    public KeyBindingDisplayer Left;
    public KeyBindingDisplayer Right;
    public KeyBindingDisplayer Sprint;
    public KeyBindingDisplayer Jump;
    public KeyBindingDisplayer Crouch;
    public KeyBindingDisplayer FreeHead;

    #endregion

    #region Shortcuts key

    public CustomText ShortcutLabel;
    public KeyBindingDisplayer Mute;
    public KeyBindingDisplayer PushToTalk;
    public KeyBindingDisplayer GeneralVolume;
    public KeyBindingDisplayer DecreaseVolume;
    public KeyBindingDisplayer IncreaseVolume;
    public KeyBindingDisplayer Cancel;
    public KeyBindingDisplayer Submit;
    public KeyBindingDisplayer GameMenu;
    public KeyBindingDisplayer ContextualMenu;
    public KeyBindingDisplayer Notification;
    public KeyBindingDisplayer UserList;

    #endregion

    protected ControllerEnum m_controller;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void InitElement()
    {
        base.InitElement();

        fpsData = Resources.Load<BaseFPSData>("Scriptables/GamePanel/FPSData");

        CamreraSensibility.label = "Camera sensibility";
        CamreraSensibility.lowValue = 2f;
        CamreraSensibility.highValue = 10f;
        CamreraSensibility.showInputField = true;
        CamreraSensibility.RegisterValueChangedCallback(ce => OnCameraSensibilityValueChanged(ce.newValue));

        ScrollView.Add(CamreraSensibility);

        JoystickStaticToggle.label = "Static joystick";
        JoystickStaticToggle.RegisterValueChangedCallback(ce => JoystickStaticUpdated(ce.newValue));

        LeftHandToggle.label = "Left hand interface";
        LeftHandToggle.RegisterValueChangedCallback(ce => LeftHandUpdated(ce.newValue));

        NavigationLabel.text = "Navigations";
        NavigationLabel.AddToClassList(USSCustomClassControlsSection);
        Forward = new KeyBindingDisplayer("Forward");
        Backward = new KeyBindingDisplayer("Backward");
        Left = new KeyBindingDisplayer("Left");
        Right = new KeyBindingDisplayer("Right");
        Sprint = new KeyBindingDisplayer("Sprint");
        Jump = new KeyBindingDisplayer("Jump");
        Crouch = new KeyBindingDisplayer("Crouch");
        FreeHead = new KeyBindingDisplayer("Free Head");

        ShortcutLabel.text = "Shortcuts";
        ShortcutLabel.AddToClassList(USSCustomClassControlsSection);
        Mute = new KeyBindingDisplayer("Mute/Unmute mic");
        PushToTalk = new KeyBindingDisplayer("Push to talk");
        GeneralVolume = new KeyBindingDisplayer("Mute/Unmute General volume");
        DecreaseVolume = new KeyBindingDisplayer("Decrease general volume");
        IncreaseVolume = new KeyBindingDisplayer("Increase general volume");
        Cancel = new KeyBindingDisplayer("Cancel");
        Submit = new KeyBindingDisplayer("Submit");
        GameMenu = new KeyBindingDisplayer("Display/Hide game menu");
        ContextualMenu = new KeyBindingDisplayer("Display/Hide contextual menu");
        Notification = new KeyBindingDisplayer("Display/Hide notifications");
        UserList = new KeyBindingDisplayer("Display/Hide users list");

        if (TryGetControllerData(out Data))
        {
            OnCameraSensibilityValueChanged(Data.CameraSensibility);
            JoystickStaticUpdated(Data.JoystickStatic);
            LeftHandUpdated(Data.LeftHand);
            InitBindings(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
        }
        else
        {
            OnCameraSensibilityValueChanged(5f);
            JoystickStaticUpdated(false);
            LeftHandUpdated(false);
            DefaultBindings(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
        }
    }

    public override void Set() => Set("Controls");

    #region Implementation

    public ControllerData Data;
    public BaseFPSData fpsData;

    /// <summary>
    /// Called when the camera sensibility changed.
    /// </summary>
    /// <param name="value"></param>
    public virtual void OnCameraSensibilityValueChanged(float value)
    {
        value = (int)value;
        CamreraSensibility.SetValueWithoutNotify(value);
        fpsData.AngularViewSpeed = new Vector2(value, value);
        Data.CameraSensibility = (int)value;
        StoreControllerrData(Data);
    }

    /// <summary>
    /// Called when joystick static has been updated.
    /// </summary>
    /// <param name="value"></param>
    protected void JoystickStaticUpdated(bool value)
    {
        JoystickStaticToggle.SetValueWithoutNotify(value);
        CustomJoystickArea.IsJoystickStatic = value;
        CustomJoystickArea.JoystickStaticModeUpdated?.Invoke();
        Data.JoystickStatic = value;
        StoreControllerrData(Data);
    }

    /// <summary>
    /// Called when the left hand or right hand status has been updated.
    /// </summary>
    /// <param name="value"></param>
    protected void LeftHandUpdated(bool value)
    {
        LeftHandToggle.SetValueWithoutNotify(value);
        CustomGame.LeftHand = value;
        Data.LeftHand = value;
        StoreControllerrData(Data);
    }

    /// <summary>
    /// Reset to the default bindings.
    /// </summary>
    /// <param name="controllers"></param>
    public void DefaultBindings(params ControllerInputEnum[] controllers)
    {
        #region Navigation

        var forward = new InputAction("forward");
        forward.AddBinding("<Keyboard>/w");
        forward.AddBinding("<Keyboard>/upArrow");
        NavigationBindingsUpdated(NavigationEnum.Forward, forward, controllers);

        var backward = new InputAction("backward");
        backward.AddBinding("<Keyboard>/s");
        backward.AddBinding("<Keyboard>/downArrow");
        NavigationBindingsUpdated(NavigationEnum.Backward, backward, controllers);

        var left = new InputAction("left");
        left.AddBinding("<Keyboard>/a");
        left.AddBinding("<Keyboard>/leftArrow");
        NavigationBindingsUpdated(NavigationEnum.Left, left, controllers);

        var right = new InputAction("right");
        right.AddBinding("<Keyboard>/d");
        right.AddBinding("<Keyboard>/rightArrow");
        NavigationBindingsUpdated(NavigationEnum.Right, right, controllers);

        var sprint = new InputAction("sprint");
        sprint.AddBinding("<Keyboard>/shift");
        NavigationBindingsUpdated(NavigationEnum.sprint, sprint, controllers);

        var jump = new InputAction("jump");
        jump.AddBinding("<Keyboard>/space");
        NavigationBindingsUpdated(NavigationEnum.Jump, jump, controllers);

        var crouch = new InputAction("crouch");
        crouch.AddBinding("<Keyboard>/c");
        NavigationBindingsUpdated(NavigationEnum.Crouch, crouch, controllers);

        var freeHead = new InputAction("freeHead");
        freeHead.AddBinding("<Keyboard>/alt");
        NavigationBindingsUpdated(NavigationEnum.FreeView, freeHead, controllers);

        #endregion

        #region Shortcut

        var muteUnmuteMic = new InputAction("muteUnmuteMic");
        muteUnmuteMic.AddBinding("<Keyboard>/#(m)");
        ShortcutBindingsUpdated(ShortcutEnum.MuteUnmuteMic, muteUnmuteMic, controllers);

        var pushToTalk = new InputAction("pushToTalk");
        pushToTalk.AddBinding("<Keyboard>/b");
        ShortcutBindingsUpdated(ShortcutEnum.PushToTalk, pushToTalk, controllers);

        var muteUnmuteGeneralVolume = new InputAction("muteUnmuteGeneralVolume");
        muteUnmuteGeneralVolume.AddBinding("<Keyboard>/l");
        ShortcutBindingsUpdated(ShortcutEnum.MuteUnmuteGeneralVolume, muteUnmuteGeneralVolume, controllers);

        var decreaseGeneralVolume = new InputAction("decreaseGeneralVolume");
        decreaseGeneralVolume.AddCompositeBinding("ButtonWithOneModifier")
            .With("Button", "<Keyboard>/#(-)")
            .With("Modifier", "<Keyboard>/ctrl");
        ShortcutBindingsUpdated(ShortcutEnum.DecreaseVolume, decreaseGeneralVolume, controllers);

        var increaseGeneralVolume = new InputAction("increaseGeneralVolume");
        increaseGeneralVolume.AddCompositeBinding("ButtonWithOneModifier")
            .With("Button", "<Keyboard>/#(+)")
            .With("Modifier", "<Keyboard>/ctrl");
        ShortcutBindingsUpdated(ShortcutEnum.IncreaseVolume, increaseGeneralVolume, controllers);

        var cancel = new InputAction("cancel");
        cancel.AddBinding("<Keyboard>/escape");
        ShortcutBindingsUpdated(ShortcutEnum.Cancel, cancel, controllers);

        var submit = new InputAction("submit");
        submit.AddBinding("<Keyboard>/enter");
        ShortcutBindingsUpdated(ShortcutEnum.Submit, submit, controllers);

        var displayHideGameMenu = new InputAction("displayHideGameMenu");
        displayHideGameMenu.AddBinding("<Keyboard>/escape");
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideGameMenu, displayHideGameMenu, controllers);

        var displayHideContextualMenu = new InputAction("displayHideContextualMenu");
        displayHideContextualMenu.AddBinding("<Mouse>/leftButton");
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideContextualMenu, displayHideContextualMenu, controllers);

        var displayHideNotifications = new InputAction("displayHideNotifications");
        displayHideNotifications.AddBinding("<Mouse>/rightButton");
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideNotifications, displayHideNotifications, controllers);

        var displayHideUsersList = new InputAction("displayHideUsersList");
        displayHideUsersList.AddBinding("<Keyboard>/u");
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideUsersList, displayHideUsersList, controllers);

        #endregion
    }

    /// <summary>
    /// Initialize the bindings.
    /// </summary>
    /// <param name="controllers"></param>
    public void InitBindings(params ControllerInputEnum[] controllers)
    {
        NavigationBindingsUpdated(NavigationEnum.Forward, Data.Forward, controllers);
        NavigationBindingsUpdated(NavigationEnum.Backward, Data.Backward, controllers);
        NavigationBindingsUpdated(NavigationEnum.Left, Data.Left, controllers);
        NavigationBindingsUpdated(NavigationEnum.Right, Data.Right, controllers);
        NavigationBindingsUpdated(NavigationEnum.sprint, Data.Sprint, controllers);
        NavigationBindingsUpdated(NavigationEnum.Jump, Data.Jump, controllers);
        NavigationBindingsUpdated(NavigationEnum.Crouch, Data.Crouch, controllers);
        NavigationBindingsUpdated(NavigationEnum.FreeView, Data.FreeHead, controllers);

        ShortcutBindingsUpdated(ShortcutEnum.MuteUnmuteMic, Data.MuteUnmuteMic, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.PushToTalk, Data.PushToTalk, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.MuteUnmuteGeneralVolume, Data.MuteUnmuteGeneralVolume, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.DecreaseVolume, Data.DecreaseGeneralVolume, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.IncreaseVolume, Data.IncreaseGeneralVolume, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.Cancel, Data.Cancel, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.Submit, Data.Submit, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideGameMenu, Data.DisplayHideGameMenu, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideContextualMenu, Data.DisplayHideContextualMenu, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideNotifications, Data.DisplayHideNotifications, controllers);
        ShortcutBindingsUpdated(ShortcutEnum.DisplayHideUsersList, Data.DisplayHideUsersList, controllers);
    }

    /// <summary>
    /// Called when navigation mapping has been updated.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <param name="controllers"></param>
    public void NavigationBindingsUpdated(NavigationEnum command, InputAction action, params ControllerInputEnum[] controllers)
    {
        int currentIndex = 0;
        FindControl(action, ref currentIndex, out string control1, controllers);
        FindControl(action, ref currentIndex, out string control2, controllers);

        switch (command)
        {
            case NavigationEnum.Forward:
                Forward.Key1.text = control1;
                Forward.Key2.text = control2;
                Data.Forward = action;
                break;
            case NavigationEnum.Backward:
                Backward.Key1.text = control1;
                Backward.Key2.text = control2;
                Data.Backward = action;
                break;
            case NavigationEnum.Left:
                Left.Key1.text = control1;
                Left.Key2.text = control2;
                Data.Left = action;
                break;
            case NavigationEnum.Right:
                Right.Key1.text = control1;
                Right.Key2.text = control2;
                Data.Right = action;
                break;
            case NavigationEnum.sprint:
                Sprint.Key1.text = control1;
                Sprint.Key2.text = control2;
                Data.Sprint = action;
                break;
            case NavigationEnum.Jump:
                Jump.Key1.text = control1;
                Jump.Key2.text = control2;
                Data.Jump = action;
                break;
            case NavigationEnum.Crouch:
                Crouch.Key1.text = control1;
                Crouch.Key2.text = control2;
                Data.Crouch = action;
                break;
            case NavigationEnum.FreeView:
                FreeHead.Key1.text = control1;
                FreeHead.Key2.text = control2;
                Data.FreeHead = action;
                break;
            default:
                break;
        }

        StoreControllerrData(Data);
    }

    /// <summary>
    /// Called when a shortcut mapping has been updated.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="action"></param>
    /// <param name="controllers"></param>
    public void ShortcutBindingsUpdated(ShortcutEnum command, InputAction action, params ControllerInputEnum[] controllers)
    {
        int currentIndex = 0;
        FindControl(action, ref currentIndex, out string control1, controllers);
        FindControl(action, ref currentIndex, out string control2, controllers);

        switch (command)
        {
            case ShortcutEnum.MuteUnmuteMic:
                Mute.Key1.text = control1;
                Mute.Key2.text = control2;
                Data.MuteUnmuteMic = action;
                break;
            case ShortcutEnum.PushToTalk:
                PushToTalk.Key1.text = control1;
                PushToTalk.Key2.text = control2;
                Data.PushToTalk = action;
                break;
            case ShortcutEnum.MuteUnmuteGeneralVolume:
                GeneralVolume.Key1.text = control1;
                GeneralVolume.Key2.text = control2;
                Data.MuteUnmuteGeneralVolume = action;
                break;
            case ShortcutEnum.DecreaseVolume:
                DecreaseVolume.Key1.text = control1;
                DecreaseVolume.Key2.text = control2;
                Data.DecreaseGeneralVolume = action;
                break;
            case ShortcutEnum.IncreaseVolume:
                IncreaseVolume.Key1.text = control1;
                IncreaseVolume.Key2.text = control2;
                Data.IncreaseGeneralVolume = action;
                break;
            case ShortcutEnum.Cancel:
                Cancel.Key1.text = control1;
                Cancel.Key2.text = control2;
                Data.Cancel = action;
                break;
            case ShortcutEnum.Submit:
                Submit.Key1.text = control1;
                Submit.Key2.text = control2;
                Data.Submit = action;
                break;
            case ShortcutEnum.DisplayHideGameMenu:
                GameMenu.Key1.text = control1;
                GameMenu.Key2.text = control2;
                Data.DisplayHideGameMenu = action;
                break;
            case ShortcutEnum.DisplayHideContextualMenu:
                ContextualMenu.Key1.text = control1;
                ContextualMenu.Key2.text = control2;
                Data.DisplayHideContextualMenu = action;
                break;
            case ShortcutEnum.DisplayHideNotifications:
                Notification.Key1.text = control1;
                Notification.Key2.text = control2;
                Data.DisplayHideNotifications = action;
                break;
            case ShortcutEnum.DisplayHideUsersList:
                UserList.Key1.text = control1;
                UserList.Key2.text = control2;
                Data.DisplayHideUsersList = action;
                break;
            case ShortcutEnum.DisplayHideEmoteWindow:
                break;
            default:
                break;
        }

        StoreControllerrData(Data);
    }

    protected bool FindControl(InputAction action, ref int index, out string control, params ControllerInputEnum[] controllers)
    {
        control = "No binding";

        if (controllers == null) return false;

        int currentIndex = action.FirstBindingIndex(out InputBinding binding, index, controllers);
        if (currentIndex == -1) return false;

        control = "";
        if (binding.isComposite)
        {
            var mapping = action.GetCompositMappingFromBindingIndex(currentIndex);
            for (int i = 0; i < mapping.Count; i++)
            {
                if (i > 0 && i < mapping.Count) control += " + ";
                control += $"[{mapping[i].Item2}]";
            }
            if (mapping.Count == 2) index = currentIndex + 3;
            else index = currentIndex + 4;
        }
        else
        {
            control = $"[{action.GetSimpleMappingFromBindingIndex(currentIndex).Item2}]";
            index = currentIndex + 1;
        }
            
        return true;
    }

    #endregion
}
