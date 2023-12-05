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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    public sealed class RequestedHandler<T> : IRequestHandler<T>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IRequestHandler<T>> Completed;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Task != null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsDone
        {
            get
            {
                return IsValid && Task.IsCompleted;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int CountTry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#if UNITY_2020_1_OR_NEWER
        public UnityWebRequest.Result Result { get; }
#else
        Result Result { get; }
#endif

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T RequestValue
        {
            get
            {
                if (IsValid && Task.IsCompleted)
                {
                    return Task.Result;
                }
                else
                {
                    throw new Exception("Request value is not accessible.");
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<T> Task { get; private set; }

        public RequestedHandler(Task<T> task)
        {
            this.Task = task;
        }

        void Update()
        {

        }
    }
}
