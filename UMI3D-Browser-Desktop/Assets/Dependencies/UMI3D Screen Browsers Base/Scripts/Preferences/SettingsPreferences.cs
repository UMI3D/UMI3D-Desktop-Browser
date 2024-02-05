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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.baseBrowser.preferences
{
    [Serializable]
    public struct Language
    {
        public string Name;
    }

    public class SettingsPreferences
    {
        public const string c_dataFolderPath = "BrowserData";

        #region General

        public const string c_generalPath = "generalData";
        [Serializable]
        public struct GeneralData
        {
            public Language LanguageChoice;
            public bool HasChosenLanguage;
        }

        public static bool TryGetGeneralData(out GeneralData data) => PreferencesManager.TryGet(out data, c_generalPath, c_dataFolderPath);
        public static void StoreGeneralData(GeneralData data) => PreferencesManager.StoreData(data, c_generalPath, c_dataFolderPath);

        #endregion

        #region Resolution

        public const string c_resolutionPath = "resolutionData";
        public enum ResolutionEnum
        {
            Low,
            Medium,
            High,
            Custom
        }
        public enum QualityEnum
        {
            VLow,
            Low,
            Medium,
            High,
            VHigh,
            Ultra
        }
        public enum UIZoom
        {
            Small,
            Medium,
            Large,
            Custom
        }
        [Serializable]
        public struct ResolutionData
        {
            public ResolutionEnum GameResolution;
            public string FullScreenResolution;
            public QualityEnum Quality;
            public bool HDR;
            public float RenderScale;
            public UIZoom UISize;
            public float DPI;
            public bool ReduceAnimation;
        }

        public static bool TryGetResolutionData(out ResolutionData data) => PreferencesManager.TryGet(out data, c_resolutionPath, c_dataFolderPath);
        public static void StoreResolutionData(ResolutionData data) => PreferencesManager.StoreData(data, c_resolutionPath, c_dataFolderPath);

        #endregion

        #region Controller

        public const string c_controllerPath = "controllerData";
        [Serializable]
        public struct ControllerData
        {
            public int CameraSensibility;

            public bool JoystickStatic;
            public bool LeftHand;

            #region Navigation

            public InputAction Forward;
            public InputAction Backward;
            public InputAction Left;
            public InputAction Right;
            public InputAction Sprint;
            public InputAction Jump;
            public InputAction Crouch;
            public InputAction FreeHead;

            #endregion

            #region Shortcut

            public InputAction MuteUnmuteMic;
            public InputAction PushToTalk;
            public InputAction MuteUnmuteGeneralVolume;
            public InputAction DecreaseGeneralVolume;
            public InputAction IncreaseGeneralVolume;
            public InputAction Cancel;
            public InputAction Submit;
            public InputAction DisplayHideGameMenu;
            public InputAction DisplayHideContextualMenu;
            public InputAction DisplayHideNotifications;
            public InputAction DisplayHideUsersList;
            public InputAction DisplayHideEmotesWindow;

            #endregion
        }

        public static bool TryGetControllerData(out ControllerData data) => PreferencesManager.TryGet(out data, c_controllerPath, c_dataFolderPath);
        public static void StoreControllerrData(ControllerData data) => PreferencesManager.StoreData(data, c_controllerPath, c_dataFolderPath);

        #endregion

        #region Audio

        public const string c_audioPath = "audioData";
        public enum MicModeEnum
        {
            AlwaysSend,
            Amplitude,
            PushToTalk,
        }
        [Serializable]
        public struct AudioData
        {
            public float GeneralVolume;
            public float LastGeneralVolumeNotZero;
            public string CurrentMic;
            public bool NoiseReduction;
            public MicModeEnum Mode;
            public float Amplitude;
            public float DelayBeforeShutMic;
            public KeyCode PushToTalkKey;
        }

        public static bool TryGetAudiorData(out AudioData data) => PreferencesManager.TryGet(out data, c_audioPath, c_dataFolderPath);
        public static void StoreAudioData(AudioData data) => PreferencesManager.StoreData(data, c_audioPath, c_dataFolderPath);

        #endregion

        #region Notification

        public const string c_notificationPath = "notificationData";
        [Serializable]
        public struct NotificationData
        {
            public bool HideNotification;
        }

        public static bool TryGetNotificationData(out NotificationData data) => PreferencesManager.TryGet(out data, c_notificationPath, c_dataFolderPath);
        public static void StoreNotificationData(NotificationData data) => PreferencesManager.StoreData(data, c_notificationPath, c_dataFolderPath);

        #endregion
    }
}
