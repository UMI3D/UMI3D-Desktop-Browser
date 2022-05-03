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
using UnityEngine.UIElements;

namespace umi3dDesktopBrowser.ui.viewController
{
    public abstract partial class AbstractBaseField_E<T> : INotifyValueChanged<T>
    {
        public virtual T value
        {
            get => m_field.value;
            set => m_field.value = value;
        }

        public virtual void SetValueWithoutNotify(T newValue)
            => m_field.SetValueWithoutNotify(newValue);
    }

    public abstract partial class AbstractBaseField_E<T>
    {
        public event Action<T, T> OnValueChanged;

        protected BaseField<T> m_field => (BaseField<T>)Root;
    }

    public abstract partial class AbstractBaseField_E<T>
    {
        public AbstractBaseField_E(BaseField<T> visual, string styleResourcePath, StyleKeys keys) :
            base(visual, styleResourcePath, keys)
        { }

        public void RiseOnValueChanged(T previous, T newValue)
            => OnValueChanged(previous, newValue);

        protected virtual void OnValueChandedEvent(ChangeEvent<T> e)
            => OnValueChanged(e.previousValue, e.newValue);
    }

    public abstract partial class AbstractBaseField_E<T> : View_E
    {
        protected override void Initialize()
        {
            base.Initialize();
            m_field.RegisterValueChangedCallback(OnValueChandedEvent);
        }

        public override void Reset()
        {
            base.Reset();
            m_field.UnregisterValueChangedCallback(OnValueChandedEvent);
        }
    }
}