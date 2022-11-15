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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomGameData : CustomMenuScreen
{
    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_timeEnvironment = new UxmlStringAttributeDescription
        {
            name = "time",
            defaultValue = "00:00"
        };
        protected UxmlIntAttributeDescription m_participantCount = new UxmlIntAttributeDescription
        {
            name = "participant-count",
            defaultValue = 0
        };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var custom = ve as CustomGameData;

            custom.Set
            (
                m_timeEnvironment.GetValueFromBag(bag, cc),
                m_participantCount.GetValueFromBag(bag, cc)
              );
        }
    }

    public virtual string Time
    {
        get => TimeInTheEnvironment_Visual.value;
        set => TimeInTheEnvironment_Visual.value = value.ToString();
    }

    public virtual int ParticipantCount
    {
        get 
        {
            if (!int.TryParse(ParticipantCount_Visual.value, out var count)) return 0;
            return count;
        }
        set => ParticipantCount_Visual.value = value.ToString();
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/gameDataScreen";
    public override string USSCustomClassName => "game-data-screen";

    public CustomScrollView ScrollView;
    public CustomTextfield TimeInTheEnvironment_Visual;
    public CustomTextfield ParticipantCount_Visual;

    public override void InitElement()
    {
        base.InitElement();

        TimeInTheEnvironment_Visual.name = "time-environment";
        TimeInTheEnvironment_Visual.isReadOnly = true;
        TimeInTheEnvironment_Visual.label = "Time spent in the environment:";

        ParticipantCount_Visual.name = "participant-count";
        ParticipantCount_Visual.isReadOnly = true;
        ParticipantCount_Visual.label = "Number of participants in the environment:";

        Add(ScrollView);
        ScrollView.Add(TimeInTheEnvironment_Visual);
        ScrollView.Add(ParticipantCount_Visual);
    }

    public override void Set() => Set("00:00", 0);

    public virtual void Set(string time, int participantCount)
    {
        Set("Game Data");

        Time = time;
        ParticipantCount = participantCount;
    }
}
