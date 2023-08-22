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
using UnityEngine;

namespace inetum.unityUtils
{
    /// <summary>
    /// Singleton that attach coroutines to the loading handler.
    /// </summary>
    /// Easily mock-able for edit mode unit tests.
    public class CoroutineManager : Singleton<CoroutineManager>, ICoroutineService, ILateRoutineService
    {
        #region Dependency Injection

        private CoroutineManagerMono coroutineManagerMono;
        private PersistentCoroutineManagerMono persistentCoroutineManagerMono;

        private Dictionary<Coroutine, bool> coroutines = new();
        private Dictionary<IEnumerator, bool> lateRoutines = new();

        public CoroutineManager() : base()
        {
            LazyInitialisationCoroutineManager();
            LazyInitialisationPersistentCoroutineManager();
        }

        internal CoroutineManager(CoroutineManagerMono coroutineManagerMono, PersistentCoroutineManagerMono persistentCoroutineManagerMono) : base()
        {
            LazyInitialisationCoroutineManager(coroutineManagerMono);
            LazyInitialisationPersistentCoroutineManager(persistentCoroutineManagerMono);
        }

        #endregion Dependency Injection

        /// <summary>
        /// Set <see cref="coroutineManagerMono"/> in a lazy way.
        /// </summary>
        /// <param name="coroutineManagerMono"></param>
        void LazyInitialisationCoroutineManager(CoroutineManagerMono coroutineManagerMono = null)
        {
            if (this.coroutineManagerMono != null)
            {
                return;
            }

            if (coroutineManagerMono != null)
            {
                this.coroutineManagerMono = coroutineManagerMono;
            }
            else
            {
                this.coroutineManagerMono = CoroutineManagerMono.Instance;
            }
        }

        /// <summary>
        /// Set <see cref="persistentCoroutineManagerMono"/> in a lazy way.
        /// </summary>
        /// <param name="persistentCoroutineManagerMono"></param>
        void LazyInitialisationPersistentCoroutineManager(PersistentCoroutineManagerMono persistentCoroutineManagerMono = null)
        {
            if (this.persistentCoroutineManagerMono != null)
            {
                return;
            }

            if (persistentCoroutineManagerMono != null)
            {
                this.persistentCoroutineManagerMono = persistentCoroutineManagerMono;
            }
            else
            {
                this.persistentCoroutineManagerMono = PersistentCoroutineManagerMono.Instance;
            }
        }

        /// <inheritdoc/>
        public virtual Coroutine AttachCoroutine(IEnumerator coroutine, bool isPersistent = false)
        {
            LazyInitialisationCoroutineManager();
            LazyInitialisationPersistentCoroutineManager();

            Debug.Assert(coroutine != null, "Coroutine null when trying to attache");
            ICoroutineService routineService = isPersistent ? persistentCoroutineManagerMono : coroutineManagerMono;
            var resRoutine = routineService.AttachCoroutine(coroutine);
            if (isPersistent)
            {
                Debug.Assert(resRoutine != null, $"resRoutine null when trying to attache a persistent coroutine");
            }
            else
            {
                Debug.Assert(CoroutineManagerMono.Exists, "CoroutineManagerMono does not exist.");
                Debug.Assert(resRoutine != null, $"resRoutine null when trying to attache a non persistent coroutine");
            }
            coroutines.Add(resRoutine, isPersistent);
            return resRoutine;
        }

        /// <inheritdoc/>
        public virtual void DetachCoroutine(Coroutine coroutine)
        {
            LazyInitialisationCoroutineManager();
            LazyInitialisationPersistentCoroutineManager();

            if (coroutines.TryGetValue(coroutine, out bool isPersistent))
            {
                ICoroutineService routineService = isPersistent ? persistentCoroutineManagerMono : coroutineManagerMono;
                routineService.DetachCoroutine(coroutine);
            }
            else
                throw new Exception("Can't detach, coroutine not found");
        }

        public virtual IEnumerator AttachLateRoutine(IEnumerator routine, bool isPersistent = false)
        {
            LazyInitialisationCoroutineManager();
            LazyInitialisationPersistentCoroutineManager();

            ILateRoutineService routineService = isPersistent ? persistentCoroutineManagerMono : coroutineManagerMono;
            lateRoutines.Add(routine, isPersistent);
            return routineService.AttachLateRoutine(routine);
        }

        public virtual void DetachLateRoutine(IEnumerator routine)
        {
            LazyInitialisationCoroutineManager();
            LazyInitialisationPersistentCoroutineManager();

            if (lateRoutines.TryGetValue(routine, out bool isPersistent))
            {
                ILateRoutineService routineService = isPersistent ? persistentCoroutineManagerMono : coroutineManagerMono;
                routineService.DetachLateRoutine(routine);
            }
            else
                throw new Exception("Can't detach, routine not found");
        }
    }
}