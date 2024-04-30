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
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.mobileBrowser.interactions
{
    public class MainMobileAction : baseBrowser.inputs.interactions.EventInteraction
    {
        public static int MenuCount;
        public static event System.Action Down;
        public static event System.Action Up;

        private static bool m_down;
        private static Coroutine m_dissociateCoroutine;

        public static void OnClickedDown() => Down?.Invoke();
        public static void OnClickedUp() => Up?.Invoke();

        private void OnEnable()
        {
            Down += OnDown;
            Up += OnUp;
        }

        private void OnDisable()
        {
            Down -= OnDown;
            Up -= OnUp;
        }

        private void OnDown() => BaseCursor.State = BaseCursor.CursorState.Clicked;
        private void OnUp() => BaseCursor.State = BaseCursor.CursorState.Default;

        public override void Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            StartCoroutine(_Associate(environmentId, interaction, toolId, hoveredObjectId));
        }

        public override void Dissociate()
        {
            m_dissociateCoroutine = StartCoroutine(_Dissociate());
        }

        public void ForceDissociate()
        {
            base.Dissociate();
            m_down = false;
            Down -= DownClicked;
            Up -= UpClicked;
            if (m_dissociateCoroutine != null)
            {
                StopCoroutine(m_dissociateCoroutine); 
                m_dissociateCoroutine = null;
            }
        }

        private System.Collections.IEnumerator _Associate(ulong environmentId, AbstractInteractionDto interaction, ulong toolId, ulong hoveredObjectId)
        {
            if (m_down) yield return new WaitUntil(() => !m_down);
            base.Associate(environmentId, interaction, toolId, hoveredObjectId);
            Down += DownClicked;
            Up += UpClicked;
        }

        private void DownClicked()
        {
            if (m_down) return;
            m_down = true;
            if (MenuCount == 1) menuItem.NotifyValueChange(true);
        }

        private void UpClicked()
        {
            if (!m_down) return;
            m_down = false;
            if (MenuCount == 1) menuItem.NotifyValueChange(false);
        }

        private System.Collections.IEnumerator _Dissociate()
        {
            if (m_down) yield return new WaitUntil(() => { return !m_down; });
            ForceDissociate();
            m_dissociateCoroutine = null;
        }
    }
}
