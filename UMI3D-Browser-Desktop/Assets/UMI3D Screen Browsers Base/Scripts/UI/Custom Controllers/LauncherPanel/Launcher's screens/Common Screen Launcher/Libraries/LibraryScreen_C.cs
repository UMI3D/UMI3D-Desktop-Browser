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

namespace umi3d.commonScreen.menu
{
    public class LibraryScreen_C : CustomLibraryScreen
    {
        public new class UxmlFactory : UxmlFactory<LibraryScreen_C, UxmlTraits> { }

        public LibraryScreen_C() => Set();

        public override void InitElement()
        {
            if (TitleLabel == null) TitleLabel = new Displayer.Text_C();
            if (Button_Back == null) Button_Back = new Displayer.Button_C();

            if (Header == null) Header = new LibraryHeader_C();
            if (Libraries_SV == null) Libraries_SV = new Container.ScrollView_C();

            base.InitElement();
        }

        protected override CustomDialoguebox CreateDialogueBox()
            => new Displayer.Dialoguebox_C();

        protected override CustomLibrary CreateLibrary()
            => new Library_C();
    }
}
