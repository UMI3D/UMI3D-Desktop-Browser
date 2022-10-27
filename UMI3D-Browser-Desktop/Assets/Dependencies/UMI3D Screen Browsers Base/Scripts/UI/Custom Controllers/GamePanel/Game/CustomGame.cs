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
using UnityEngine;
using UnityEngine.UIElements;

public class CustomGame : VisualElement, ICustomElement, IGameView
{
    public virtual string StyleSheetGamePath => $"USS/game";
    public virtual string StyleSheetPath => $"{ElementExtensions.StyleSheetGamesFolderPath}/game";
    public virtual string USSCustomClassName => "game";

    public System.Action<object> SetMovement;
    public System.Action<object> UnSetMovement;
    public CustomCursor Cursor;
    public CustomLeadingArea LeadingArea;
    public CustomTrailingArea TrailingArea;
    public CustomTopArea TopArea;
    public CustomBottomArea BottomArea;

    protected bool m_hasBeenInitialized;

    public virtual void InitElement()
    {
        try
        {
            this.AddStyleSheetFromPath(StyleSheetGamePath);
            this.AddStyleSheetFromPath(StyleSheetPath);
        }
        catch (System.Exception e)
        {
            throw e;
        }
        AddToClassList(USSCustomClassName);

        Add(Cursor);
        Add(LeadingArea);
        Add(TrailingArea);
        Add(TopArea);
        //Add(BottomArea);
    }

    public virtual void Set()
    {
        if (!m_hasBeenInitialized)
        {
            InitElement();
            m_hasBeenInitialized = true;
        }
    }

    public void TransitionIn(VisualElement persistentVisual)
        => Transition(persistentVisual, false);

    public void TransitionOut(VisualElement persistentVisual)
        => Transition(persistentVisual, true);

    protected virtual void Transition(VisualElement persistentVisual, bool revert)
    {
        this.AddAnimation
        (
            persistentVisual,
            () => style.opacity = 0,
            () => style.opacity = 1,
            "opacity",
            0.5f,
            revert: revert,
            callback: revert ? RemoveFromHierarchy : null
        );
    }
}
