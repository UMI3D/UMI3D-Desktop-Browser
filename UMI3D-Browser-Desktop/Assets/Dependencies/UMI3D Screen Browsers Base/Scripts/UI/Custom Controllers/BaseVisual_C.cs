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
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace umi3d.commonScreen
{
    public abstract class BaseVisual_C : VisualElement
    {
        /// <summary>
        /// Event raised when a property changed, if this element is attached to a panel.
        /// </summary>
        public event System.Action<object, object, string> PropertyChangedEvent;

        /// <summary>
        /// Whether or not this element has been set.
        /// </summary>
        public bool IsSet { get; protected set; }
        /// <summary>
        /// Whether or not this element is attached to a panel.
        /// </summary>
        public bool IsAttachedToPanel { get; protected set; }

        /// <summary>
        /// Style sheet dedicated to the main theme.
        /// </summary>
        public virtual string StyleSheetPath_MainTheme => $"";

        /// <summary>
        /// Element main Uss class.
        /// </summary>
        public virtual string UssCustomClass_Emc => "";

        public BaseVisual_C()
        {
            this.RegisterCallback<AttachToPanelEvent>(AttachedToPanel);
            this.RegisterCallback<DetachFromPanelEvent>(DetachedFromPanel);
            this.RegisterCallback<CustomStyleResolvedEvent>(CustomStyleResolved);
            this.RegisterCallback<GeometryChangedEvent>(GeometryChanged);
            this.RegisterCallback<TransitionRunEvent>(TransitionRun);
            this.RegisterCallback<TransitionStartEvent>(TransitionStarted);
            this.RegisterCallback<TransitionEndEvent>(TransitionEnded);
            this.RegisterCallback<TransitionCancelEvent>(TransitionCanceled);
            IsSet = false;
            InstanciateChildren();
            _AttachStyleSheet();
            AttachUssClass();
            InitElement();
            IsSet = true;
            SetProperties();
        }

        /// <summary>
        /// Where to instanciate visual children of this element.
        /// </summary>
        protected virtual void InstanciateChildren()
        {
        }

        /// <summary>
        /// Add style and theme style sheets to this element.
        /// </summary>
        protected virtual void AttachStyleSheet()
        {
            this.AddStyleSheetFromPath(StyleSheetPath_MainTheme);
        }

        private void _AttachStyleSheet()
        {
            try
            {
                AttachStyleSheet();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        /// <summary>
        /// Add Uss custom classes to this element and its children.
        /// </summary>
        protected virtual void AttachUssClass()
        {
            AddToClassList(UssCustomClass_Emc);
        }

        /// <summary>
        /// Initialise this element.
        /// </summary>
        /// <remarks>This methode is called by the base class <see cref="BaseVisual_C"/>. This methode is in the range of <see cref="IsSet"/> equals to false.</remarks>
        protected virtual void InitElement()
        {
        }

        /// <summary>
        /// Set the properties.
        /// </summary>
        /// <remarks>This methode is in the range of <see cref="IsSet"/> equals to true.</remarks>
        protected virtual void SetProperties()
        {
        }

        /// <summary>
        /// Method called when this element is attached to a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to register to an event you should use this methode.</remarks>
        protected virtual void AttachedToPanel(AttachToPanelEvent evt)
        {
            IsAttachedToPanel = true;
            PropertyChangedEvent += PropertyChanged;
        }

        /// <summary>
        /// Method called when this element is detached from a panel.
        /// </summary>
        /// <param name="evt"></param>
        /// <remarks>If you want to unregister to an event you should use this methode.</remarks>
        protected virtual void DetachedFromPanel(DetachFromPanelEvent evt)
        {
            PropertyChangedEvent -= PropertyChanged;
            IsAttachedToPanel = false;
        }

        /// <summary>
        /// Method called when a custom style sheet is resolved.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void CustomStyleResolved(CustomStyleResolvedEvent evt)
        {

        }

        /// <summary>
        /// Method called when this element geometry changed.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void GeometryChanged(GeometryChangedEvent evt)
        {

        }

        /// <summary>
        /// Method called when a transition is created.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionRun(TransitionRunEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Method called when a transition start. (after transition'delay end)
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionStarted(TransitionStartEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Method called when a transition end properly without being canceled.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionEnded(TransitionEndEvent evt)
        {
            evt.StopPropagation();
            foreach (var property in evt.stylePropertyNames)
            {
                this.TriggerAnimationCallback(property);
            }
        }

        /// <summary>
        /// Method called when a transition is canceled.
        /// </summary>
        /// <param name="evt"></param>
        protected virtual void TransitionCanceled(TransitionCancelEvent evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Raise the <see cref="PropertyChangedEvent"/> event if this elemnet is attached to a panel, else call <see cref="PropertyChanged(object, object, string)"/>
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="callerName"></param>
        protected void OnPropertyChanged(object oldValue, object newValue, [CallerMemberName] string callerName = "")
        {
            if (IsAttachedToPanel) PropertyChangedEvent?.Invoke(oldValue, newValue, callerName);
            else PropertyChanged(oldValue, newValue, callerName);
        }

        protected virtual void PropertyChanged(object oldValue, object newValue, [CallerMemberName] string callerName = "")
        {
        }
    }
}
