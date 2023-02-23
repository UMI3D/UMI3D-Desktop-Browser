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
using System;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.utils
{
    public interface ISmartData<T>
    {
        /// <summary>
        /// Event raised just before the value changed.
        /// </summary>
        /// <remarks>When added the action will be raised with <see cref="Value"/> as previous and new value</remarks>
        event Action<ChangeEvent<T>> ValueWillChanged;
        /// <summary>
        /// Event raised just after the value changed.
        /// </summary>
        /// <remarks>When added the action will be raised with <see cref="Value"/> as previous and new value</remarks>
        event Action<ChangeEvent<T>> ValueChanged;

        /// <summary>
        /// The value that is wrapped in this container.
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// Set the value without raised <see cref="ValueChanged"/>.
        /// </summary>
        /// <param name="value"></param>
        void SetValueWithoutNotify(T value);
    }

    public class Source<T>: ISmartData<T>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        public event Action<ChangeEvent<T>> ValueWillChanged
        {
            add
            {
                value?.Invoke(ChangeEvent<T>.GetPooled(m_value, m_value));
                m_valueWillChanged += value;
            }
            remove => m_valueWillChanged -= value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        public event Action<ChangeEvent<T>> ValueChanged
        {
            add
            {
                value?.Invoke(ChangeEvent<T>.GetPooled(m_value, m_value));
                m_valueChanged += value;
            }
            remove => m_valueChanged -= value;
        }

        /// <summary>
        /// Event raised just before the value changed.
        /// </summary>
        protected event Action<ChangeEvent<T>> m_valueWillChanged;
        /// <summary>
        /// Event raised just after the value changed.
        /// </summary>
        protected event Action<ChangeEvent<T>> m_valueChanged;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T Value
        {
            get => m_value;
            set
            {
                var oldValue = m_value;
                var e = ChangeEvent<T>.GetPooled(oldValue, value);
                OnvalueWillChanged(e);
                SetValueWithoutNotify(value);
                OnvalueChanged(e);
            }
        }
        protected T m_value;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValueWithoutNotify(T value)
            => m_value = value;

        protected virtual void OnvalueWillChanged(ChangeEvent<T> e)
            => m_valueWillChanged?.Invoke(e);

        protected virtual void OnvalueChanged(ChangeEvent<T> e)
            => m_valueChanged?.Invoke(e);

        /// <summary>
        /// Implicite convertor: Instanciate a <see cref="Source{T}"/> with a value of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Source<T>(T value)
            => new Source<T> { Value = value };

        /// <summary>
        /// Implicite convertor: return <see cref="Value"/>.
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Source<T> source) => source.Value;
    }
}
