/*
Copy 2019 - 2023 Inetum

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
#if UNITY_EDITOR

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.common.userCapture
{
    public class PoseConditionPanel : VisualElement
    {
        public class Uxmlfactory : UxmlFactory<PoseConditionPanel, PoseConditionPanel.UxmlTraits> { }

        public event Action<UMI3DPoseOveridder_so> onConditionCreated;

        Toggle tg_isInterpolationable;
        Toggle tg_isComposable;
        UintField_UI_Elements duration;
        UintField_UI_Elements min_duration;
        UintField_UI_Elements max_duration;

        Button add_condition;
        Button remove_condition;

        VisualElement condition_container;

        List<ConditionField> condition_fields = new List<ConditionField>();

        UMI3DPoseOveridder_so poseOveridder_So;

        public void Init()
        {
            GetRef();
            BindUI();
            Hide();
        }

        public void Enable()
        {
            Show();
            CreatePoseOverriderInstance();
        }

        public void Disable()
        {
            Hide();
            poseOveridder_So = null;
        }

        public UMI3DPoseOveridder_so GetPoseOveridder_So()
        {
            UMI3DPoseOveridder_so poseOverrider = (UMI3DPoseOveridder_so)ScriptableObject.CreateInstance(typeof(UMI3DPoseOveridder_so));
            poseOveridder_So.duration = new DurationDto((uint)duration.value, (uint)min_duration.value, (ulong)max_duration.value);
            poseOveridder_So.composable = tg_isComposable.value;
            poseOveridder_So.interpolationable = tg_isInterpolationable.value;
            poseOveridder_So.poseConditions = new PoseConditionDto[condition_fields.Count];

            condition_fields.ForEach(cf =>
            {
                poseOveridder_So.poseConditions.Append(cf.GetPoseConditionDto());
            });

            return poseOveridder_So;
        }

        private void GetRef()
        {
            tg_isInterpolationable = this.Q<Toggle>("tg_isInterpolationable");
            tg_isComposable = this.Q<Toggle>("tg_isComposable");

            duration = this.Q<UintField_UI_Elements>("duration");
            min_duration = this.Q<UintField_UI_Elements>("min_duration");
            max_duration = this.Q<UintField_UI_Elements>("max_duration");

            add_condition = this.Q<Button>("add_condition");
            remove_condition = this.Q<Button>("remove_condition");

            condition_container = this.Q<VisualElement>("condition_container");
        }

        private void BindUI()
        {
            InitFields();
            BindButtons();
        }

        private void InitFields()
        {
            duration.Init();
            min_duration.Init();
            max_duration.Init();
        }

        private void BindButtons()
        {
            condition_container.Clear();

            add_condition.clicked += () =>
            {
                ConditionField condition = new ConditionField();
                condition_fields.Add(condition);
                condition_container.Add(condition);
            };

            remove_condition.clicked += () =>
            {
                if (condition_fields != null && condition_fields.Count > 0)
                {
                    VisualElement visualElement = condition_fields?.Last();
                    if (visualElement != null)
                    {
                        condition_fields.Remove(visualElement as ConditionField);
                        condition_container.Remove(visualElement);
                    }
                }

            };
        }


        private void Hide()
        {
            style.display = DisplayStyle.None;
        }

        private void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        private void CreatePoseOverriderInstance()
        {
            poseOveridder_So = ScriptableObject.CreateInstance("UMI3DPoseOveridder_so") as UMI3DPoseOveridder_so;
            onConditionCreated?.Invoke(poseOveridder_So);
        }
    }
}

#endif