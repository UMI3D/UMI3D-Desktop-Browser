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
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace umi3d.baseBrowser.inputs.interactions
{
    public class KeyboardInteraction : BaseKeyInteraction
    {
        public static List<KeyboardInteraction> S_Interactions = new List<KeyboardInteraction>();

        protected override void CreateMenuItem()
        {
            base.CreateMenuItem();

            Mapped?.Invoke(this, menuItem.Name, null);
        }

        public override void Dissociate()
        {
            base.Dissociate();

            Unmapped?.Invoke(this);
        }

        public static System.Action<KeyboardInteraction, string, List<(string, MappingType)>> Mapped;
        public static System.Action<KeyboardInteraction> Unmapped;
    }
}