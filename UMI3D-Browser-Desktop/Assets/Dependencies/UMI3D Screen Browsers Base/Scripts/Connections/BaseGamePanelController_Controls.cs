/*
Copyright 2019 - 2023 Inetum
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
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.cdk.collaboration;
using umi3d.commonScreen.game;
using UnityEngine;
using UnityEngine.InputSystem;
using static umi3d.baseBrowser.cursor.BaseCursor;
using static umi3d.commonScreen.game.GamePanel_C;
using static umi3d.commonScreen.game.NotifAndUsersArea_C;

namespace umi3d.baseBrowser.connection
{
    public partial class BaseGamePanelController
    {

        protected virtual void InitControls()
        {
            InitControls_ContextualMenu();
            InitControls_CancelAndSubmit();

            KeyboardShortcut.AddUpListener(ShortcutEnum.DisplayHideGameMenu, () =>
            {
                if (GamePanel.CurrentView == GameViews.GameMenu)
                {
                    GamePanel.AddScreenToStack = GameViews.Game;
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                    CloseGameWindows();
                }
                else
                {
                    GamePanel.AddScreenToStack = GameViews.GameMenu;
                    BaseCursor.SetMovement(this, CursorMovement.Free);
                }
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.DisplayHideNotifications, () =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                ) return;

                if
                (
                    !Game.DisplayNotifUsersArea
                    || Game.NotifAndUserArea.AreaPanel != NotificationsOrUsers.Notifications)
                {
                    BaseCursor.SetMovement(this, CursorMovement.Free);

                    if (!Game.DisplayNotifUsersArea) Game.DisplayNotifUsersArea = true;
                    Game.NotifAndUserArea.AreaPanel = NotificationsOrUsers.Notifications;
                }
                else
                {
                    CloseGameWindows();
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                }
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.DisplayHideUsersList, () =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                ) return;

                if
                (
                    !Game.DisplayNotifUsersArea
                    || Game.NotifAndUserArea.AreaPanel != NotificationsOrUsers.Users)
                {
                    BaseCursor.SetMovement(this, CursorMovement.Free);

                    if (!Game.DisplayNotifUsersArea) Game.DisplayNotifUsersArea = true;
                    Game.NotifAndUserArea.AreaPanel = NotificationsOrUsers.Users;
                }
                else
                {
                    CloseGameWindows();
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                }
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.FreeCursor, () =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                ) return;

                if (BaseCursor.Movement == CursorMovement.Center) BaseCursor.SetMovement(this, CursorMovement.Free);
                else
                {
                    CloseGameWindows();
                    BaseCursor.SetMovement(this, CursorMovement.Center);
                }
            });

            KeyboardEmote.EmotePressed += index =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                    || BaseCursor.Movement == CursorMovement.Free
                ) return;

                if (EmoteWindow_C.Emotes == null || EmoteWindow_C.Emotes.Count <= index) return;
                var emote = EmoteWindow_C.Emotes[index];
                EmoteManager.Instance.PlayEmote(emote);
            };

            BaseConnectionProcess.Instance.EnvironmentLeave += () => NotifAndUsersArea_C.Instance = null;
        }

        protected void InitControls_ContextualMenu()
        {
            KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideContextualMenu, () =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                ) return;

                if (!Game.IsLeadingAndtrailingClicked(GetMouseWorldPosition())) return;

                m_contextualMenuActionDown?.Invoke();
            });
            KeyboardShortcut.AddUpListener(ShortcutEnum.DisplayHideContextualMenu, () =>
            {
                if
                (
                    GamePanel.CurrentView == GameViews.GameMenu
                    || GamePanel.CurrentView == GameViews.Loader
                ) return;

                if (!Game.IsLeadingAndtrailingClicked(GetMouseWorldPosition())) return;

                m_contextualMenuActionUp?.Invoke();
            });

            Game.TrailingArea.ButtonsArea.MainActionDown = m_contextualMenuActionDown;
            Game.TrailingArea.ButtonsArea.MainActionUp = m_contextualMenuActionUp;
        }

        protected void InitControls_CancelAndSubmit()
        {
            KeyboardShortcut.AddUpListener(ShortcutEnum.Cancel, () =>
            {
                UnityEngine.Debug.Log("<color=green>TODO: </color>" + $"Cancel");
            });

            KeyboardShortcut.AddUpListener(ShortcutEnum.Submit, () =>
            {
                m_next?.Invoke();
            });
        }

        protected Vector2 GetMouseWorldPosition()
        {
            Vector2 result = Vector2.zero;
            switch (PanelSettings.scaleMode)
            {
                case UnityEngine.UIElements.PanelScaleMode.ConstantPixelSize:
                    break;
                case UnityEngine.UIElements.PanelScaleMode.ConstantPhysicalSize:
                    float ratio = PanelSettings.referenceDpi / PanelSettings.fallbackDpi;
                    result = Mouse.current.position.ReadValue();
                    result.y = Screen.height - result.y;
                    result *= ratio;
                    break;
                case UnityEngine.UIElements.PanelScaleMode.ScaleWithScreenSize:
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}