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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using umi3d.common;
using UnityEngine.Networking;

namespace umi3d.browserRuntime.connection
{
    public sealed class AsyncRequestedHandler : IAsyncRequestHandler
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event Action<IAsyncRequestHandler> Completed
        {
            add 
            {
                if (IsDone)
                {
                    value?.Invoke(this);
                }
                completed += value;
            } 
            remove 
            {
                completed -= value;
            }
        }

        public event Action<IAsyncRequestHandler> Aborted
        {
            add
            {
                if (HasBeenCanceled)
                {
                    value.Invoke(this);
                }
                aborted += value;
            }
            remove 
            {
                aborted -= value;
            }
        }

        public Func<bool> IsCancellationRequired { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsValid
        {
            get
            {
                return webRequest != null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsDone
        {
            get
            {
                if (!IsValid || !hasBeenExecuted)
                {
                    return false;
                }
                else if (isBeingExecuted)
                {
                    return operation.isDone;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool HasBeenCanceled
        {
            get
            {
                return canceled;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int TryCount
        {
            get
            {
                return tries;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public float Progress
        {
            get
            {
                if (!IsValid || !hasBeenExecuted)
                {
                    return 0f;
                }
                else if (isBeingExecuted)
                {
                    return operation.progress;
                }
                else
                {
                    return 1f;
                }
            }
        }

#if UNITY_2020_1_OR_NEWER
        public UnityWebRequest.Result Result
        {
            get
            {
                if (!IsValid || !hasBeenExecuted)
                {
                    return UnityWebRequest.Result.InProgress;
                }
                else if (isBeingExecuted)
                {
                    return webRequest.result;
                }
                else
                {
                    return result;
                }
            }
        }
#else
        public Result Result { get; }
#endif
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Error
        {
            get
            {
                if (!IsValid || !hasBeenExecuted)
                {
                    return null;
                }
                else if (isBeingExecuted)
                {
                    return webRequest.error;
                }
                else
                {
                    return error;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string DownloadedText
        {
            get
            {
                if (!IsDone)
                {
                    return null;
                }
                else
                {
                    return downloadedText;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task RequestTask { get; private set; }

        UnityWebRequest webRequest;
        UnityWebRequestAsyncOperation operation;
        bool canceled = false;
#if UNITY_2020_1_OR_NEWER
        UnityWebRequest.Result result;
#else
        Result result;
#endif
        string error;
        string downloadedText;

        Func<int, Task<UnityWebRequest>> webRequestFactory;
        int tries = 0;
        Action<IAsyncRequestHandler> completed;
        Action<IAsyncRequestHandler> aborted;
        bool hasBeenExecuted = false;
        bool isBeingExecuted = false;

        public AsyncRequestedHandler(Func<int, Task<UnityWebRequest>> webRequestFactory)
        {
            this.webRequestFactory = webRequestFactory;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            if (hasBeenExecuted)
            {
                UnityEngine.Debug.LogError($"{nameof(Execute)} method can only be called once.");
                return;
            }
            RequestTask = FactoryRequestTask();
            await RequestTask;
        }

        async Task FactoryRequestTask()
        {
            tries++;
            webRequest = await webRequestFactory(tries);
            canceled = false;
            hasBeenExecuted = true;
            isBeingExecuted = true;

            operation = webRequest.SendWebRequest();
            operation.completed += handler =>
            {
                error = webRequest.error;
#if UNITY_2020_1_OR_NEWER
                result = webRequest.result;
#else
                throw new System.NotImplementedException();
#endif
                downloadedText = webRequest.downloadHandler.text;

                isBeingExecuted = false;
                OnCompleted();
                (handler as UnityWebRequestAsyncOperation).webRequest.Dispose();
            };

            while (!operation.isDone)
            {
                if (IsCancellationRequired?.Invoke() ?? false)
                {
                    Abort();
                }
                await Task.Yield();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public async Task<int> Retry()
        {
            if (!hasBeenExecuted || isBeingExecuted)
            {
                UnityEngine.Debug.LogError($"You cannot retry a request that as not been executed or finished.");
                return -1;
            }

            hasBeenExecuted = false;
            canceled = false;
            await Execute();
            return tries;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Abort()
        {
            if (canceled)
            {
                return;
            }

            canceled = true;
            webRequest?.Abort();
            aborted?.Invoke(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public T GetDownloadedData<T>(TypeNameHandling typeNameHandling = TypeNameHandling.All, IList<JsonConverter> converters = null)
        {
            if (!IsDone)
            {
                throw new Exception("Request value is not accessible.");
            }
            else
            {
                return UMI3DDtoSerializer.FromJson<T>(DownloadedText, typeNameHandling, converters);
            }
        }

        void OnCompleted()
        {
            if (canceled)
            {
                return;
            }
            completed?.Invoke(this);
        }
    }
}
