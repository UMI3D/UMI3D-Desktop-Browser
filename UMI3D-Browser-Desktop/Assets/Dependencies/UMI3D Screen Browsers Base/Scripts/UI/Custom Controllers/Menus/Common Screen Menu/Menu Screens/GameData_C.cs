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
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class GameData_C : BaseMenuScreen_C
    {
        public struct TextLineDisplayer
        {
            public Text_C Label;
            public Text_C Text;
            public VisualElement Box;

            public TextLineDisplayer(LocalisationAttribute label)
            {
                Label = new Text_C();
                Text = new Text_C();

                Label.LocalisedText = label;

                Text.Color = TextColor.Menu;

                Box = new VisualElement { name = "box-displayer" };
                Box.AddToClassList(USSCustomClassBox());
                Box.Add(Label);
                Box.Add(Text);
            }

            public static System.Func<string> USSCustomClassBox;

            public static implicit operator TextLineDisplayer(LocalisationAttribute local)
                => new TextLineDisplayer(local);
        }

        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
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

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                if (Application.isPlaying) return;

                base.Init(ve, bag, cc);
                var custom = ve as GameData_C;

                custom.WorldName = m_worldName.GetValueFromBag(bag, cc);
                custom.EnvironmentName = m_environmentName.GetValueFromBag(bag, cc);
                custom.Time = m_timeEnvironment.GetValueFromBag(bag, cc);
                custom.ParticipantCount = m_participantCount.GetValueFromBag(bag, cc);
            }
        }

        public virtual string WorldName
        {
            get => WorldName_Displayer.Text.text;
            set => WorldName_Displayer.Text.LocalisedText = value;
        }

        public virtual string EnvironmentName
        {
            get => EnvironmentName_Displayer.Text.text;
            set => EnvironmentName_Displayer.Text.LocalisedText = value;
        }

        public virtual string Time
        {
            get => TimeInTheEnvironment_Displayer.Text.text;
            set => TimeInTheEnvironment_Displayer.Text.LocalisedText = value.ToString();
        }

        public virtual int ParticipantCount
        {
            get
            {
                if (!int.TryParse(ParticipantCount_Displayer.Text.text, out var count)) return 0;
                return count;
            }
            set => ParticipantCount_Displayer.Text.LocalisedText = value.ToString();
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/gameDataScreen";
        public override string UssCustomClass_Emc => "game__data__screen";
        public virtual string USSCustomClassBox => $"{UssCustomClass_Emc}-box__displayer";

        public ScrollView_C ScrollView = new ScrollView_C { name = "scroll-view" };
        public TextLineDisplayer WorldName_Displayer;
        public TextLineDisplayer EnvironmentName_Displayer;
        public TextLineDisplayer TimeInTheEnvironment_Displayer;
        public TextLineDisplayer ParticipantCount_Displayer;

        public GameData_C() { }

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            TextLineDisplayer.USSCustomClassBox = () => USSCustomClassBox;
        }

        protected override void InitElement()
        {
            base.InitElement();
            WorldName_Displayer = new LocalisationAttribute("World name:", "GameDataScreen", "WorldName");
            EnvironmentName_Displayer = new LocalisationAttribute("Environment name:", "GameDataScreen", "EnvironmentName");
            TimeInTheEnvironment_Displayer = new LocalisationAttribute("Time spent in the environment:", "GameDataScreen", "TimeSpent");
            ParticipantCount_Displayer = new LocalisationAttribute("Number of participants in the environment:", "GameDataScreen", "NbParticipants");

            Add(ScrollView);
            ScrollView.Add(WorldName_Displayer.Box);
            ScrollView.Add(EnvironmentName_Displayer.Box);
            ScrollView.Add(TimeInTheEnvironment_Displayer.Box);
            ScrollView.Add(ParticipantCount_Displayer.Box);
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Game Data", "GameDataScreen", "GameData");
            WorldName = null;
            EnvironmentName = null;
            Time = "00:00";
            ParticipantCount = 0;
        }
    }
}
