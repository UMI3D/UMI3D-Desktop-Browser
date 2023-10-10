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
using umi3d.commonScreen.Container;
using umi3d.commonScreen.Displayer;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.menu
{
    public class TipsScreen_C : BaseMenuScreen_C
    {

        public new class UxmlFactory : UxmlFactory<TipsScreen_C, UxmlTraits> { }
        public new class UxmlTraits : BaseMenuScreen_C.UxmlTraits
        {
            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="ve"></param>
            /// <param name="bag"></param>
            /// <param name="cc"></param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }

        public override string StyleSheetPath => $"{ElementExtensions.StyleSheetMenusFolderPath}/libraryScreen";
        public override string UssCustomClass_Emc => "library__screen";
        public virtual string USSCustomClassHeader => $"{UssCustomClass_Emc}-header";
        public virtual string USSCustomClassScrollView => $"{UssCustomClass_Emc}-scroll__view";
        
        private ScrollView_C _tipsTables_SV = new ScrollView_C { name = "scroll-view" };

        private List<Tip_C> _tips = new();
        private List<Text_C> _categories = new();
        private TipsTable[] _tipsTables;

        protected override void AttachUssClass()
        {
            base.AttachUssClass();
            _tipsTables_SV.AddToClassList(USSCustomClassScrollView);
        }

        protected override void InitElement()
        {
            base.InitElement();
            Add(_tipsTables_SV);

            _tipsTables_SV.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        }

        protected override void SetProperties()
        {
            base.SetProperties();
            Title = new LocalisationAttribute("Tips", "LauncherScreen", "Tips");
        }
        public override VisualElement contentContainer => IsSet ? _tipsTables_SV.contentContainer : this;

        public void InitTips()
        {
            /*_tipsTables = Resources.LoadAll<TipsTable>("");
            _tipsTables = new TipsTable[1];
            _tips.Clear();
            _categories.Clear();

            foreach (var tipsTable in _tipsTables)
            {
                var category = new Text_C();
                category.LocalisedText = tipsTable.Name;
                _categories.Add(category);
                _tipsTables_SV.Add(category);

                foreach (var tip in tipsTable.Tips)
                {
                    var myTip = new Tip_C();
                    myTip.Title = tip.Title;
                    myTip.Message = tip.Message;
                    myTip.Tip = tip;

                    _tips.Add(myTip);
                    _tipsTables_SV.Add(myTip);
                }
            }*/
        }
    }
}