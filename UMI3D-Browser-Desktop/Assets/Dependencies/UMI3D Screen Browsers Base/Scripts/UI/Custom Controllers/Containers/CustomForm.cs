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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomForm : CustomAbstractScrollableContainer
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override ElementCategory Category
    {
        get => base.Category;
        set
        {
            base.Category = value;
            VScroll.Category = value;
        }
    }

    public virtual string USSCustomClassForm => "form";

    public CustomScrollView VScroll;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void InitElement()
    {
        base.InitElement();
        AddToClassList(USSCustomClassForm);

        Add(VScroll);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override VisualElement contentContainer => m_isSet && VScroll != null ? VScroll.contentContainer : this;
}
