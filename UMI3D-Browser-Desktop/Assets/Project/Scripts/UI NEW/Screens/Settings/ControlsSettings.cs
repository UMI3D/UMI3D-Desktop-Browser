using inetum.unityUtils;
using System;
using System.Text.RegularExpressions;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.baseBrowser.Navigation;
using umi3d.baseBrowser.preferences;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.inputs.interactions.BaseKeyInteraction;
using static umi3d.baseBrowser.preferences.SettingsPreferences;
using static UnityEngine.Rendering.DebugUI;

public class ControlsSettings : BaseSettings
{
    private ScrollView m_Navigation;
    private ScrollView m_Shortcut;

    private SliderInt_C m_CameraSensibility;

    private ControllerData m_ControllerData;
    private BaseFPSData m_FpsData;

    public ControlsSettings(VisualElement pRoot) : base(pRoot)
    {
        m_Navigation = m_Root.Q<ScrollView>("ControlsNavigation");
        m_Shortcut = m_Root.Q<ScrollView>("ControlsShortcut");
        m_FpsData = Resources.Load<BaseFPSData>("Scriptables/GamePanel/FPSData");

        if (TryGetControlsData(out m_ControllerData))
        {
            SetupNavigation(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
            SetupShortcut(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
        }
        else
        {
            var item1 = true;
            DefaultNavigationKey(ref item1, ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
            SetupCameraSensibility();
            m_CameraSensibility.value = 5;
            DefaultShortcutKey(ControllerInputEnum.Keyboard, ControllerInputEnum.Mouse);
        }

        SetupMenu();
    }

    #region Navigation
    private void SetupNavigation(params ControllerInputEnum[] pControllers)
    {
        var item1 = true;
        try
        {
            AddNavigationKey(NavigationEnum.Forward, m_ControllerData.Forward, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.Backward, m_ControllerData.Backward, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.Left, m_ControllerData.Left, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.Right, m_ControllerData.Right, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.sprint, m_ControllerData.Sprint, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.Jump, m_ControllerData.Jump, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.Crouch, m_ControllerData.Crouch, ref item1, pControllers);
            AddNavigationKey(NavigationEnum.FreeView, m_ControllerData.FreeHead, ref item1, pControllers);
        }
        catch (Exception ex)
        {
            DefaultNavigationKey(ref item1, pControllers);
        }

        SetupCameraSensibility();
        m_CameraSensibility.value = m_ControllerData.CameraSensibility;
    }

    private void SetupCameraSensibility()
    {
        m_CameraSensibility = new SliderInt_C();
        m_CameraSensibility.label = "Camera Sensibility";
        m_CameraSensibility.lowValue = 2;
        m_CameraSensibility.highValue = 10;
        m_CameraSensibility.RegisterValueChangedCallback(e => OnCameraSensibilityChanged(e.newValue));
        m_Navigation.Add(m_CameraSensibility);
    }

    private void OnCameraSensibilityChanged(int pValue)
    {
        m_FpsData.AngularViewSpeed = new Vector2(pValue, pValue);
        m_ControllerData.CameraSensibility = pValue;
        StoreControllerData(m_ControllerData);
    }

    private void DefaultNavigationKey(ref bool pItem1, params ControllerInputEnum[] pControllers)
    {
        var item1 = true;
        var forward = new InputAction("forward");
        forward.AddBinding("<Keyboard>/w");
        forward.AddBinding("<Keyboard>/upArrow");
        m_ControllerData.Forward = forward;
        AddNavigationKey(NavigationEnum.Forward, forward, ref item1, pControllers);

        var backward = new InputAction("backward");
        backward.AddBinding("<Keyboard>/s");
        backward.AddBinding("<Keyboard>/downArrow");
        m_ControllerData.Backward = backward;
        AddNavigationKey(NavigationEnum.Backward, backward, ref item1, pControllers);

        var left = new InputAction("left");
        left.AddBinding("<Keyboard>/a");
        left.AddBinding("<Keyboard>/leftArrow");
        m_ControllerData.Left = left;
        AddNavigationKey(NavigationEnum.Left, left, ref item1, pControllers);

        var right = new InputAction("right");
        right.AddBinding("<Keyboard>/d");
        right.AddBinding("<Keyboard>/rightArrow");
        m_ControllerData.Right = right;
        AddNavigationKey(NavigationEnum.Right, right, ref item1, pControllers);

        var sprint = new InputAction("sprint");
        sprint.AddBinding("<Keyboard>/shift");
        m_ControllerData.Sprint = sprint;
        AddNavigationKey(NavigationEnum.sprint, sprint, ref item1, pControllers);

        var jump = new InputAction("jump");
        jump.AddBinding("<Keyboard>/space");
        m_ControllerData.Jump = jump;
        AddNavigationKey(NavigationEnum.Jump, jump, ref item1, pControllers);

        var crouch = new InputAction("crouch");
        crouch.AddBinding("<Keyboard>/c");
        m_ControllerData.Crouch = crouch;
        AddNavigationKey(NavigationEnum.Crouch, crouch, ref item1, pControllers);

        var freeHead = new InputAction("freeHead");
        freeHead.AddBinding("<Keyboard>/alt");
        m_ControllerData.FreeHead = freeHead;
        AddNavigationKey(NavigationEnum.FreeView, freeHead, ref item1, pControllers);
    }

    private void AddNavigationKey(NavigationEnum pNavigationKey, InputAction pAction, ref bool pItem1, params ControllerInputEnum[] pControllers)
    {
        if (pAction == null) throw new NullReferenceException($"Action null for {pNavigationKey}");

        int currentIndex = 0;
        FindControl(pAction, ref currentIndex, out string control1, pControllers);
        FindControl(pAction, ref currentIndex, out string control2, pControllers);

        var keyBind = new KeyBind_C();
        keyBind.Label = Regex.Replace(pNavigationKey.ToString(), "([a-z])([A-Z])", "$1 $2");
        keyBind.Key1 = control1;
        keyBind.Key2 = control2;
        keyBind.AddToClassList(pItem1 ? "item1" : "item2");
        pItem1 = !pItem1;

        m_Navigation.Add(keyBind);
    }
    #endregion

    #region Shortcut
    private void SetupShortcut(params ControllerInputEnum[] pControllers)
    {
        var item1 = true;
        try
        {
            AddShortcutKey(ShortcutEnum.MuteUnmuteMic, m_ControllerData.MuteUnmuteMic, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.PushToTalk, m_ControllerData.PushToTalk, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.MuteUnmuteGeneralVolume, m_ControllerData.MuteUnmuteGeneralVolume, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.DecreaseVolume, m_ControllerData.DecreaseGeneralVolume, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.IncreaseVolume, m_ControllerData.IncreaseGeneralVolume, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.Cancel, m_ControllerData.Cancel, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.Submit, m_ControllerData.Submit, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.DisplayHideGameMenu, m_ControllerData.DisplayHideGameMenu, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.DisplayHideContextualMenu, m_ControllerData.DisplayHideContextualMenu, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.DisplayHideNotifications, m_ControllerData.DisplayHideNotifications, ref item1, pControllers);
            AddShortcutKey(ShortcutEnum.DisplayHideUsersList, m_ControllerData.DisplayHideUsersList, ref item1, pControllers);

        }
        catch (Exception ex)
        {
            DefaultShortcutKey(pControllers);
        }
    }

    private void DefaultShortcutKey(params ControllerInputEnum[] pControllers)
    {
        var item1 = true; 
        var muteUnmuteMic = new InputAction("muteUnmuteMic");
        muteUnmuteMic.AddBinding("<Keyboard>/#(m)");
        m_ControllerData.MuteUnmuteMic = muteUnmuteMic;
        AddShortcutKey(ShortcutEnum.MuteUnmuteMic, muteUnmuteMic, ref item1, pControllers);

        var pushToTalk = new InputAction("pushToTalk");
        pushToTalk.AddBinding("<Keyboard>/b");
        m_ControllerData.PushToTalk = pushToTalk;
        AddShortcutKey(ShortcutEnum.PushToTalk, pushToTalk, ref item1, pControllers);

        var muteUnmuteGeneralVolume = new InputAction("muteUnmuteGeneralVolume");
        muteUnmuteGeneralVolume.AddBinding("<Keyboard>/l");
        m_ControllerData.MuteUnmuteGeneralVolume = muteUnmuteGeneralVolume;
        AddShortcutKey(ShortcutEnum.MuteUnmuteGeneralVolume, muteUnmuteGeneralVolume, ref item1, pControllers);

        var decreaseGeneralVolume = new InputAction("decreaseGeneralVolume");
        decreaseGeneralVolume.AddCompositeBinding("ButtonWithOneModifier")
            .With("Button", "<Keyboard>/#(-)")
            .With("Modifier", "<Keyboard>/ctrl");
        m_ControllerData.DecreaseGeneralVolume = decreaseGeneralVolume;
        AddShortcutKey(ShortcutEnum.DecreaseVolume, decreaseGeneralVolume, ref item1, pControllers);

        var increaseGeneralVolume = new InputAction("increaseGeneralVolume");
        increaseGeneralVolume.AddCompositeBinding("ButtonWithOneModifier")
            .With("Button", "<Keyboard>/#(+)")
            .With("Modifier", "<Keyboard>/ctrl");
        m_ControllerData.IncreaseGeneralVolume = increaseGeneralVolume;
        AddShortcutKey(ShortcutEnum.IncreaseVolume, increaseGeneralVolume, ref item1, pControllers);

        var cancel = new InputAction("cancel");
        cancel.AddBinding("<Keyboard>/escape");
        m_ControllerData.Cancel = cancel;
        AddShortcutKey(ShortcutEnum.Cancel, cancel, ref item1, pControllers);

        var submit = new InputAction("submit");
        submit.AddBinding("<Keyboard>/enter");
        m_ControllerData.Submit = submit;
        AddShortcutKey(ShortcutEnum.Submit, submit, ref item1, pControllers);

        var displayHideGameMenu = new InputAction("displayHideGameMenu");
        displayHideGameMenu.AddBinding("<Keyboard>/escape");
        m_ControllerData.DisplayHideGameMenu = displayHideGameMenu;
        AddShortcutKey(ShortcutEnum.DisplayHideGameMenu, displayHideGameMenu, ref item1, pControllers);

        var displayHideContextualMenu = new InputAction("displayHideContextualMenu");
        displayHideContextualMenu.AddBinding("<Mouse>/leftButton");
        m_ControllerData.DisplayHideContextualMenu = displayHideContextualMenu;
        AddShortcutKey(ShortcutEnum.DisplayHideContextualMenu, displayHideContextualMenu, ref item1, pControllers);

        var displayHideNotifications = new InputAction("displayHideNotifications");
        displayHideNotifications.AddBinding("<Mouse>/rightButton");
        m_ControllerData.DisplayHideNotifications = displayHideNotifications;
        AddShortcutKey(ShortcutEnum.DisplayHideNotifications, displayHideNotifications, ref item1, pControllers);

        var displayHideUsersList = new InputAction("displayHideUsersList");
        displayHideUsersList.AddBinding("<Keyboard>/u");
        m_ControllerData.DisplayHideUsersList = displayHideUsersList;
        AddShortcutKey(ShortcutEnum.DisplayHideUsersList, displayHideUsersList, ref item1, pControllers);
    }

    private void AddShortcutKey(ShortcutEnum pShortcutKey, InputAction pAction, ref bool pItem1, params ControllerInputEnum[] pControllers)
    {
        if (pAction == null) throw new NullReferenceException($"Action null for {pShortcutKey}");

        int currentIndex = 0;
        FindControl(pAction, ref currentIndex, out string control1, pControllers);
        FindControl(pAction, ref currentIndex, out string control2, pControllers);

        var keyBind = new KeyBind_C();
        keyBind.Label = Regex.Replace(pShortcutKey.ToString(), "([a-z])([A-Z])", "$1 $2");
        keyBind.Key1 = control1;
        keyBind.Key2 = control2;
        keyBind.AddToClassList(pItem1 ? "item1" : "item2");
        pItem1 = !pItem1;

        m_Shortcut.Add(keyBind);
    }
    #endregion

    private void SetupMenu()
    {
        m_Root.Q<RadioButton>("ControlsButtonNavigation").RegisterValueChangedCallback(e =>
        {
            if (e.newValue)
            {
                m_Navigation.RemoveFromClassList("hidden");
                m_Shortcut.AddToClassList("hidden");
            } else
            {
                m_Navigation.AddToClassList("hidden");
                m_Shortcut.RemoveFromClassList("hidden");
            }
        });

        m_Navigation.RemoveFromClassList("hidden");
        m_Shortcut.AddToClassList("hidden");
    }

    private static bool FindControl(InputAction action, ref int index, out string control, params ControllerInputEnum[] controllers)
    {
        control = "-";

        if (controllers == null) return false;

        int currentIndex = action.FirstBindingIndex(out InputBinding binding, index, controllers);
        if (currentIndex == -1) return false;

        control = "";

        try
        {
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
        }
        catch (System.IndexOutOfRangeException)
        {
            control = "undef";
        }

        return true;
    }
    public static bool TryGetControlsData(out ControllerData data) => PreferencesManager.TryGet(out data, c_controllerPath, c_dataFolderPath);
    public static void StoreControllerData(ControllerData data) => PreferencesManager.StoreData(data, c_controllerPath, c_dataFolderPath);

}