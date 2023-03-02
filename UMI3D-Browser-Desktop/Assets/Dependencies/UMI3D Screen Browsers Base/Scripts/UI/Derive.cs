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
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.utils
{
    public class Derive<T>: ISmartData<T>
    {
        protected Source<T> m_source;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        public event Action<ChangeEvent<T>> ValueWillChanged
        {
            add
            {
                if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
                m_source.ValueWillChanged += value;
            }
            remove
            {
                if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
                m_source.ValueWillChanged -= value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks><inheritdoc/></remarks>
        public event Action<ChangeEvent<T>> ValueChanged
        {
            add
            {
                if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
                m_source.ValueChanged += value;
            }
            remove
            {
                if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
                m_source.ValueChanged -= value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T Value 
        { 
            get  => m_source != null 
                ? m_source.Value 
                : throw new ArgumentNullException("Derived data used before being binding");
            set
            {
                if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
                m_source.Value = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetValueWithoutNotify(T value)
        {
            if (m_source == null) throw new ArgumentNullException("Derived data used before being binding");
            m_source.SetValueWithoutNotify(value);
        }

        protected Derive() { }

        /// <summary>
        /// Implicite convertor: Instanciate a <see cref="Derive{T}"/> with a value of <see cref="Source{T}"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Derive<T>(Source<T> value)
            => new Derive<T> { m_source = value };

        /// <summary>
        /// Implicite convertor: return <see cref="Value"/>.
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Derive<T> derive) => derive.Value;
    }
}
