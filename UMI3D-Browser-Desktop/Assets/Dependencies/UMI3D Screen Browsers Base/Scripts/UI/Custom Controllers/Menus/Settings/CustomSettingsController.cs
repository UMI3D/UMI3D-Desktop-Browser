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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.preferences.SettingsPreferences;

public class CustomSettingsController : CustomSettingScreen
{
    public enum ControllerInputEnum { Gamepad, Keyboard, Mouse}

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

    public ControllerEnum Controller
    {
        get => m_controller;
        set
        {
            m_controller = value;
            switch (value)
            {
                case ControllerEnum.MouseAndKeyboard:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    break;
                case ControllerEnum.Touch:
                    ScrollView.Add(JoystickStaticToggle);
                    ScrollView.Add(LeftHandToggle);
                    break;
                case ControllerEnum.GameController:
                    JoystickStaticToggle.RemoveFromHierarchy();
                    LeftHandToggle.RemoveFromHierarchy();
                    break;
                default:
                    break;
            }
        }
    }

    public override string USSCustomClassName => "setting-controller";
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box";

    public CustomSlider CamreraSensibility;

    public CustomToggle JoystickStaticToggle;
    public CustomToggle LeftHandToggle;

    #region Navigation key

    public KeyBindingDisplayer Forward;
    public KeyBindingDisplayer Backward;
    public KeyBindingDisplayer Left;
    public KeyBindingDisplayer Right;
    public KeyBindingDisplayer Sprint;
    public KeyBindingDisplayer Jump;
    public KeyBindingDisplayer Crouch;
    public KeyBindingDisplayer FreeHead;

    #endregion

    protected ControllerEnum m_controller;

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

        Forward = new KeyBindingDisplayer("Forward");
        Backward = new KeyBindingDisplayer("Backward");
        Left = new KeyBindingDisplayer("Left");
        Right = new KeyBindingDisplayer("Right");
        Sprint = new KeyBindingDisplayer("Sprint");
        Jump = new KeyBindingDisplayer("Jump");
        Crouch = new KeyBindingDisplayer("Crouch");
        FreeHead = new KeyBindingDisplayer("Free Head");
        ScrollView.Add(Forward.Box);
        ScrollView.Add(Backward.Box);
        ScrollView.Add(Left.Box);
        ScrollView.Add(Right.Box);
        ScrollView.Add(Sprint.Box);
        ScrollView.Add(Jump.Box);
        ScrollView.Add(Crouch.Box);
        ScrollView.Add(FreeHead.Box);

        DefaultBindings(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);

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

    public virtual void OnCameraSensibilityValueChanged(float value)
    {
        value = (int)value;
        CamreraSensibility.SetValueWithoutNotify(value);
        fpsData.AngularViewSpeed = new Vector2(value, value);
        Data.CameraSensibility = (int)value;
        StoreControllerrData(Data);
    }

    protected void JoystickStaticUpdated(bool value)
    {
        JoystickStaticToggle.SetValueWithoutNotify(value);
        CustomJoystickArea.IsJoystickStatic = value;
        CustomJoystickArea.JoystickStaticModeUpdated?.Invoke();
        Data.JoystickStatic = value;
        StoreControllerrData(Data);
    }

    protected void LeftHandUpdated(bool value)
    {
        LeftHandToggle.SetValueWithoutNotify(value);
        CustomGame.LeftHand = value;
        Data.LeftHand = value;
        StoreControllerrData(Data);
    }

    public void DefaultBindings(params ControllerInputEnum[] controllers)
    {
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
        //crouch.AddBinding("<Keyboard>/c");
        crouch.AddCompositeBinding("ButtonWithOneModifier")
            .With("Button", "<Keyboard>/c")
            .With("Modifier", "<Keyboard>/leftCtrl");
        NavigationBindingsUpdated(NavigationEnum.Crouch, crouch, controllers);

        var freeHead = new InputAction("freeHead");
        freeHead.AddBinding("<Keyboard>/alt");
        NavigationBindingsUpdated(NavigationEnum.FreeView, freeHead, controllers);
    }

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
    }

    public void NavigationBindingsUpdated(NavigationEnum command, InputAction action, params ControllerInputEnum[] controllers)
    {
        //UnityEngine.Debug.Log($"------------------------bindings = {action.bindings.Count}");
        //for (int i = 0; i < action.bindings.Count; i++)
        //{
        //    action.ChangeBinding(i).Erase();
        //}
        //UnityEngine.Debug.Log($"bindings = {action.bindings.Count}-------------------------");

        //action.ChangeBinding(1).WithPath("<Keyboard>/alt");
        
        if (action.bindings[0].isComposite)
        {
            UnityEngine.Debug.Log($"{action.bindings[0].path}");
        }
        if (action.bindings.Count > 1 && action.bindings[1].isPartOfComposite)
        {
            UnityEngine.Debug.Log($"is part of composit {action.bindings[1].ToDisplayString()}");
        }

        var controls = action.controls;
        int currentIndex = -1;
        FindControl(controls, ref currentIndex, out string control1, controllers);
        FindControl(controls, ref currentIndex, out string control2, controllers);

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

    protected bool FindControl(ReadOnlyArray<InputControl> controls, ref int index, out string control, params ControllerInputEnum[] controllers)
    {
        var lowIndex = index;
        control = "No binding";

        if (controllers == null) return false;

        var inputControl = controls.FirstOrDefault(_control =>
        {
            if (controls.IndexOf(item => item == _control) <= lowIndex) return false;

            bool matchController = false;
            foreach (var controller in controllers)
            {
                if (_control.device.displayName == controller.ToString()) matchController = true;
            }
            if (!matchController) return false;
            
            return true;
        });

        if (inputControl == null) return false;

        control = inputControl.displayName;
        index = controls.IndexOf(item => item == inputControl);
        return true;
    }

    #endregion
}
