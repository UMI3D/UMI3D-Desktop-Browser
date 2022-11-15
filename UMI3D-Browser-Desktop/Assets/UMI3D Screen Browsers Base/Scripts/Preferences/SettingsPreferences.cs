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

namespace umi3d.baseBrowser.preferences
{
    public class SettingsPreferences
    {
        [Serializable]
        public struct GeneralData
        {

        }

        #region Resolution

        public const string c_resolutionPath = "resolutionData";
        public enum ResolutionEnum
        {
            Low,
            Medium,
            High,
            Custom
        }
        [Serializable]
        public struct ResolutionData
        {
            public ResolutionEnum SegmentedResolution;
            public string Resolution;
            public bool SupportHDR;
            public float RenderScale;
            public bool ReduceAnimation;
        }

        public static bool TryGetResolutionData(out ResolutionData data) => PreferencesManager.TryGet(out data, c_resolutionPath);
        public static void StoreResolutionData(ResolutionData data) => PreferencesManager.StoreData(data, c_resolutionPath);

        #endregion

        #region Player

        public const string c_playerPath = "playerData";
        [Serializable]
        public struct PlayerData
        {
            public int CameraSensibility;
        }

        public static bool TryGetPlayerData(out PlayerData data) => PreferencesManager.TryGet(out data, c_playerPath);
        public static void StorePlayerData(PlayerData data) => PreferencesManager.StoreData(data, c_playerPath);

        #endregion

        #region Controller

        public const string c_controllerPath = "controllerData";
        [Serializable]
        public struct ControllerData
        {
            public bool JoystickStatic;
            public bool LeftHand;
        }

        public static bool TryGetControllerData(out ControllerData data) => PreferencesManager.TryGet(out data, c_controllerPath);
        public static void StoreControllerrData(ControllerData data) => PreferencesManager.StoreData(data, c_controllerPath);

        #endregion

        #region Audio

        public const string c_audioPath = "audioData";
        [Serializable]
        public struct AudioData
        {
            public float GeneralVolume;
        }

        public static bool TryGetAudiorData(out AudioData data) => PreferencesManager.TryGet(out data, c_audioPath);
        public static void StoreAudioData(AudioData data) => PreferencesManager.StoreData(data, c_audioPath);

        #endregion

        #region Notification

        public const string c_notificationPath = "notificationData";
        [Serializable]
        public struct NotificationData
        {
            public bool HideNotification;
        }

        public static bool TryGetNotificationData(out NotificationData data) => PreferencesManager.TryGet(out data, c_notificationPath);
        public static void StoreNotificationData(NotificationData data) => PreferencesManager.StoreData(data, c_notificationPath);

        #endregion
    }
}
