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
using UnityEngine.ResourceManagement.AsyncOperations;

namespace inetum.unityUtils
{
    /// <summary>
    /// <see cref="AsyncOperationHandle{TResult}"/> utils.
    /// </summary>
    public static class AOHUtils
    {
        public static AOHStack<T> WhenAll<T>(Action<AsyncOperationHandle<T>[]> allCompleted, params AsyncOperationHandle<T>[] stack)
        {
            var result = new AOHStack<T>(stack);
            result.allCompleted += allCompleted;
            return result;
        }
    }
}
