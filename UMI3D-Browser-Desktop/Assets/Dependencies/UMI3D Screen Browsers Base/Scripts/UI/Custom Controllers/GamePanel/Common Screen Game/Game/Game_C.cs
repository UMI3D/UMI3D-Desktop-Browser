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
using UnityEngine.UIElements;

namespace umi3d.commonScreen.game
{
    public sealed class Game_C : CustomGame
    {
        public new class UxmlFactory : UxmlFactory<Game_C, UxmlTraits> { }

        public Game_C() => Set();

        public override void InitElement()
        {
            if (Cursor == null) Cursor = new Cursor_C();
            if (LeadingArea == null) LeadingArea = new LeadingArea_C();
            if (TrailingArea == null) TrailingArea = new TrailingArea_C();
            if (TopArea == null) TopArea = new TopArea_C();
            if (BottomArea == null) BottomArea = new commonDesktop.game.BottomArea_C();

            base.InitElement();
        }
    }
}
