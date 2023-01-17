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
using UnityEngine.UIElements;

namespace umi3d.commonScreen.Displayer
{
    public class InteractableMappingRow_C : CustomInteractableMappingRow
    {
        public new class UxmlFactory : UxmlFactory<InteractableMappingRow_C, UxmlTraits> { }

        public InteractableMappingRow_C() => Set();

        public override void InitElement()
        {
            if (ActionNameText == null) ActionNameText = new Text_C();

            base.InitElement();
        }

        protected List<CustomMapping> WaitingMappings = new List<CustomMapping>();
        protected List<CustomMapping> ActiveMappings = new List<CustomMapping>();

        protected override CustomMapping CreateMapping()
        {
            CustomMapping mapping = null;
            if (WaitingMappings.Count == 0) mapping = new Mapping_C();
            else
            {
                mapping = WaitingMappings[WaitingMappings.Count - 1];
                WaitingMappings.RemoveAt(WaitingMappings.Count - 1);
            }
            ActiveMappings.Add(mapping);

            return mapping;
        }
        protected override void RemoveMapping(CustomMapping mapping)
        {
            if (mapping == null) return;
            ActiveMappings.Remove(mapping);
            WaitingMappings.Add(mapping);
        }
    }
}
