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
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.ui.viewController
{
    public partial class ButtonManipulator : VisualManipulator
    {
        protected override void RegisterCallbacksOnTarget()
        {
            base.RegisterCallbacksOnTarget();
            //Buttons don't work with mouseDownEvent et mouseUpEvent
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }
    }
}
