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
using System.Collections.Generic;

namespace umi3d.browserRuntime.connection
{
    /// <summary>
    /// Interface to stack <see cref="IConnectionState"/> and be notified when the stack is updated.
    /// </summary>
    public interface IConnectionStateData
    {
        /// <summary>
        /// Event raised when a connection state has been added;
        /// </summary>
        event Action<IConnectionState> StateAdded;
        /// <summary>
        /// Event raised when the stack has been cleared.
        /// </summary>
        event Action Cleared;

        IEnumerator<IConnectionState> States { get; }

        IConnectionState this[int index] { get; }

        /// <summary>
        /// Return true if the state whose Guid is <paramref name="id"/> is present in the collection.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Contains(Guid id);
        /// <summary>
        /// Add a <see cref="IConnectionState"/> to the stack. <br/>
        /// Return true if the item has been added.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Add(IConnectionState data, Guid id);
        /// <summary>
        /// Clear the stack.
        /// </summary>
        void Clear();
    }
}
