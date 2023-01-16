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

namespace umi3d.commonScreen.game
{
    public class InteractableMapping_C : CustomInteractableMapping
    {
        public new class UxmlFactory : UxmlFactory<InteractableMapping_C, UxmlTraits> { }

        public InteractableMapping_C() => Set();

        public override void InitElement()
        {
            if (InteractableNameText == null) InteractableNameText = new Displayer.Text_C();
            if (ScrollView == null) ScrollView = new Container.ScrollView_C();

            base.InitElement();
        }

        protected List<CustomInteractableMappingRow> WaitingRows = new List<CustomInteractableMappingRow>();
        protected List<CustomInteractableMappingRow> ActiveRows = new List<CustomInteractableMappingRow>();

        protected override CustomInteractableMappingRow CreateMappingRow()
        {
            CustomInteractableMappingRow row = null;
            if (WaitingRows.Count == 0) row = new Displayer.InteractableMappingRow_C();
            else
            {
                row = WaitingRows[WaitingRows.Count - 1];
                WaitingRows.RemoveAt(WaitingRows.Count - 1);
            }
            ActiveRows.Add(row);

            return row;
        }
        protected override void RemoveMappingRow(CustomInteractableMappingRow row)
        {
            if (row == null) return;
            ActiveRows.Remove(row);
            WaitingRows.Add(row);
        }
    }
}
