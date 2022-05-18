/*
Copyright 2019 - 2021 Inetum

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
using umi3DBrowser.UICustomStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class ProgressBar_E
    {
        public event Action Complete;
        public Icon_E Bar { get; protected set; } = null;
        public float Percentage
        {
            get => m_percentage;
            set
            {
                var newValue = Mathf.Clamp(value, 0f, 1f);

                if (newValue > m_percentage)
                {
                    Anime(Bar.Root, m_percentage, newValue, 500, true, (visual, percentage) =>
                    {
                        visual.style.width = Length.Percent(percentage);
                    });
                }
                else
                    Bar.Root.style.width = Length.Percent(newValue);

                m_percentage = newValue;
                if (m_percentage >= 1f) Complete?.Invoke();
            }
        }

        private float m_percentage;

        public void SetBar(string partialStylePath, StyleKeys keys)
            => Bar.UpdateRootStyleAndKeysAndManipulator(partialStylePath, keys);

        public void LaunchTimeBar(int time)
        {
            Anime(Bar.Root, 0f, 100f, time, true, (visual, percentage) =>
            {
                visual.style.width = Length.Percent(percentage);
                m_percentage = percentage;
            }).OnCompleted(() => Complete?.Invoke());
        }
    }

    public partial class ProgressBar_E : Box_E
    {
        public ProgressBar_E(string partialStylePath, StyleKeys keys) :
            base(partialStylePath, keys)
        { }

        protected override void Initialize()
        {
            base.Initialize();

            Root.name = "progressBar";
            Bar = new Icon_E();
            Bar.InsertRootTo(Root);
        }
    } 
}
