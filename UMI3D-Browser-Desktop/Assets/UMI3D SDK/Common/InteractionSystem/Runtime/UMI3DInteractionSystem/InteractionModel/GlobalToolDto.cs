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

<<<<<<< HEAD:UMI3D-Browser-Desktop/Assets/Project/Scripts/UI_CustomStyle/Theme_SO.cs
namespace umi3DBrowser.UICustomStyle
{
    [CreateAssetMenu(fileName = "NewTheme", menuName = "Browser_SO/Theme")]
    public partial class Theme_SO : ScriptableObject
    {
        [SerializeField]
        private Color m_primary;
        [SerializeField]
        private Color m_secondary;
        [SerializeField]
        private Color m_tertiary;

        public Color Primary => m_primary;
        public Color Secondary => m_secondary;
        public Color Tertiary => m_tertiary;
=======
namespace umi3d.common.interaction
{
    public class GlobalToolDto : AbstractToolDto, IEntity
    {
        /// <summary>
        /// Toolbox in which the global tool is (if any).
        /// </summary>
        /// <see cref="isInsideToolbox"/>
        public ulong toolboxId;
        public bool isInsideToolbox;

        public GlobalToolDto() : base() { }
>>>>>>> US_4912_CDK_2.5:UMI3D-Browser-Desktop/Assets/UMI3D SDK/Common/InteractionSystem/Runtime/UMI3DInteractionSystem/InteractionModel/GlobalToolDto.cs
    }
}