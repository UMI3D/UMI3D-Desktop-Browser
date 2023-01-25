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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.menu;
using umi3d.commonMobile.game;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public class PinnedToolsArea_C : CustomPinnedToolsArea
    {
        /// <summary>
        /// Current instance of this ui element. If there is no instance a new one is created.
        /// </summary>
        public static PinnedToolsArea_C Instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                s_instance = new PinnedToolsArea_C();
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        protected static PinnedToolsArea_C s_instance;

        public new class UxmlFactory : UxmlFactory<PinnedToolsArea_C, UxmlTraits> { }

        public PinnedToolsArea_C() => Set();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void InitElement()
        {
            if (SDC == null) SDC = new Container.ScrollableDataCollection_C<AbstractMenuItem>();
            SDC.MakeItem = () => new Container.Toolbox_C();

            if (Sub_SDC == null) Sub_SDC = new Container.ScrollableDataCollection_C<AbstractMenuItem>();
            Sub_SDC.MakeItem = () => new Container.Toolbox_C();

            base.InitElement();
        }
    }
}
