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
    public struct TextLineDisplayer
    {
        public CustomText Label;
        public CustomText Text;
        public VisualElement Box;

        public TextLineDisplayer(string label, string text)
        {
            Label = CreateText();
            Text = CreateText();

            Label.text = label;
            Text.text = text;

            Text.Color = TextColor.Menu;

            Box = new VisualElement { name = "box-displayer" };
            Box.AddToClassList(USSCustomClassBox());
            Box.Add(Label);
            Box.Add(Text);
        }

        public static System.Func<CustomText> CreateText;
        public static System.Func<string> USSCustomClassBox;
    }

    public new class UxmlTraits : CustomMenuScreen.UxmlTraits
    {
        protected UxmlStringAttributeDescription m_worldName = new UxmlStringAttributeDescription
        {
            name = "world-name",
            defaultValue = null
        };
        protected UxmlStringAttributeDescription m_environmentName = new UxmlStringAttributeDescription
        {
            name = "environment-name",
            defaultValue = null
        };
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
            if (Application.isPlaying) return;

            base.Init(ve, bag, cc);
            var custom = ve as CustomGameData;

            custom.Set
            (
                m_worldName.GetValueFromBag(bag, cc),
                m_environmentName.GetValueFromBag(bag, cc),
                m_timeEnvironment.GetValueFromBag(bag, cc),
                m_participantCount.GetValueFromBag(bag, cc)
              );
        }
    }

    public virtual string WorldName
    {
        get => WorldName_Displayer.Text.text;
        set => WorldName_Displayer.Text.text = value;
    }

    public virtual string EnvironmentName
    {
        get => EnvironmentName_Displayer.Text.text;
        set => EnvironmentName_Displayer.Text.text = value;
    }

    public virtual string Time
    {
        get => TimeInTheEnvironment_Displayer.Text.text;
        set => TimeInTheEnvironment_Displayer.Text.text = value.ToString();
    }

    public virtual int ParticipantCount
    {
        get 
        {
            if (!int.TryParse(ParticipantCount_Displayer.Text.text, out var count)) return 0;
            return count;
        }
        set => ParticipantCount_Displayer.Text.text = value.ToString();
    }

    public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/gameDataScreen";
    public override string USSCustomClassName => "game__data__screen";
    public virtual string USSCustomClassBox => $"{USSCustomClassName}-box__displayer";

    public CustomScrollView ScrollView;
    public TextLineDisplayer WorldName_Displayer;
    public TextLineDisplayer EnvironmentName_Displayer;
    public TextLineDisplayer TimeInTheEnvironment_Displayer;
    public TextLineDisplayer ParticipantCount_Displayer;

    public override void InitElement()
    {
        base.InitElement();

        WorldName_Displayer = new TextLineDisplayer("World name:", "");
        EnvironmentName_Displayer = new TextLineDisplayer("Environment name:", "");
        TimeInTheEnvironment_Displayer = new TextLineDisplayer("Time spent in the environment:", "");
        ParticipantCount_Displayer = new TextLineDisplayer("Number of participants in the environment:", "");

        Add(ScrollView);
        ScrollView.Add(WorldName_Displayer.Box);
        ScrollView.Add(EnvironmentName_Displayer.Box);
        ScrollView.Add(TimeInTheEnvironment_Displayer.Box);
        ScrollView.Add(ParticipantCount_Displayer.Box);
    }

    public override void Set() => Set(null, null, "00:00", 0);

    public virtual void Set(string worldName, string environmentName, string time, int participantCount)
    {
        Set("Game Data");

        WorldName = worldName;
        EnvironmentName = environmentName;
        Time = time;
        ParticipantCount = participantCount;
    }
}
