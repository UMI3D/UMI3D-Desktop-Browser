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
using UnityEngine;
using UnityEngine.Events;

namespace umi3DBrowser.UICustomStyle
{
    [CreateAssetMenu(fileName = "NewTheme", menuName = "Browser_SO/Theme")]
    public partial class Theme_SO : ScriptableObject
    {
        [Header("Menu Colors")]
        [SerializeField]
        private Color m_menuPrimaryLight;
        [SerializeField]
        private Color m_menuPrimaryDark;
        [SerializeField]
        private Color m_menuSecondaryLight;
        [SerializeField]
        private Color m_menuSecondaryDark;
        [SerializeField]
        private Color m_menuTransparentLight;
        [SerializeField]
        private Color m_menuTransparentDark;

        [Header("Label Colors")]
        [SerializeField]
        private Color m_labelPrimaryLight;
        [SerializeField]
        private Color m_labelPrimaryDark;
        [SerializeField]
        private Color m_labelSecondaryLight;
        [SerializeField]
        private Color m_labelSecondaryDark;

        [Header("Icon Colors")]
        [SerializeField]
        private Color m_iconPrimaryLight;
        [SerializeField]
        private Color m_iconPrimaryDark;
        [SerializeField]
        private Color m_iconSecondaryLight;
        [SerializeField]
        private Color m_iconSecondaryDark;

        public Color MenuPrimaryLight => m_menuPrimaryLight;
        public Color MenuPrimaryDark => m_menuPrimaryDark;
        public Color MenuSecondaryLight => m_menuSecondaryLight;
        public Color MenuSecondaryDark => m_menuSecondaryDark;
        public Color MenuTransparentLight => m_menuTransparentLight;
        public Color MenuTransparentDark => m_menuTransparentDark;

        public Color LabelPrimaryLight => m_labelPrimaryLight;
        public Color LabelPrimaryDark => m_labelPrimaryDark;
        public Color LabelSecondaryLight => m_labelSecondaryLight;
        public Color LabelSecondaryDark => m_labelSecondaryDark;

        public Color IconPrimaryLight => m_iconPrimaryLight;
        public Color IconPrimaryDark => m_iconPrimaryDark;
        public Color IconSecondaryLight => m_iconSecondaryLight;
        public Color IconSecondaryDark => m_iconSecondaryDark;
    }
}