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
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public sealed class LeadingArea_C : CustomLeadingArea
    {
        public new class UxmlFactory : UxmlFactory<LeadingArea_C, UxmlTraits> { }

        public LeadingArea_C() => Set();

        public override void InitElement()
        {
            if (PinnedToolsArea == null)
            {
                if (Application.isPlaying) PinnedToolsArea = PinnedToolsArea_C.Instance;
                else PinnedToolsArea = new PinnedToolsArea_C();
            }
            if (InteractableMapping == null) InteractableMapping = new InteractableMapping_C();
            if (JoystickArea == null) JoystickArea = new commonMobile.game.JoystickArea_C();
    
            base.InitElement();
        }
    }
}
