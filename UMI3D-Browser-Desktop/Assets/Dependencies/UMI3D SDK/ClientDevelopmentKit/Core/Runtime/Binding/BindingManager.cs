﻿/*
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

using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;
using UnityEngine;

namespace umi3d.cdk
{
    /// <summary>
    /// Handle binding operations and computations.
    /// </summary>
    public interface IBindingManager
    {
        /// <summary>
        /// Are bindings computed for this client?
        /// </summary>
        public bool AreBindingsActivated { get; }

        /// <summary>
        /// Currently computed bindings per UMI3D node id.
        /// </summary>
        public Dictionary<ulong, AbstractBinding> Bindings { get; }

        /// <summary>
        /// Load and add a new binding operation.
        /// </summary>
        /// <param name="dto"></param>
        public void AddBinding(BindingDto dto);

        /// <summary>
        /// Add a binding that already has been loaded.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="boundNodeId"></param>
        public void AddBinding(AbstractBinding binding, ulong boundNodeId);

        /// <summary>
        /// Remove a binding to compute.
        /// </summary>
        /// <param name="dto"></param>
        public void RemoveBinding(RemoveBindingDto dto);

        /// <summary>
        /// Load a binding et prepare it for computing.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="boundNodeId"></param>
        /// <returns></returns>
        public AbstractBinding Load(AbstractBindingDataDto dto, ulong boundNodeId);

        /// <summary>
        /// Enable/disable bindings computation for this client.
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateBindingsActivation(UpdateBindingsActivationDto dto);
    }

    /// <summary>
    /// Core binding manager. Handles binding operations and computations.
    /// </summary>
    public class BindingManager : Singleton<BindingManager>, IBindingManager
    {
        #region dependency injection

        private readonly UMI3DLoadingHandler coroutineService;
        private readonly UMI3DEnvironmentLoader environmentService;

        public BindingManager() : base()
        {
            coroutineService = UMI3DLoadingHandler.Instance;
            environmentService = UMI3DEnvironmentLoader.Instance;
        }

        #endregion dependency injection

        public bool AreBindingsActivated { get; private set; } = true;

        public Dictionary<ulong, AbstractBinding> Bindings { get; private set; } = new();

        /// <summary>
        /// Execute every binding.
        /// </summary>
        private Coroutine bindingCoroutine;

        public void DisableBindings()
        {
            AreBindingsActivated = false;
        }

        public void EnableBindings()
        {
            AreBindingsActivated = true;
        }

        public void UpdateBindingsActivation(UpdateBindingsActivationDto dto)
        {
            if (dto.areBindingsActivated == AreBindingsActivated)
                return;

            if (dto.areBindingsActivated)
                EnableBindings();
            else
                DisableBindings();
        }

        public void AddBinding(BindingDto dto)
        {
            AbstractBinding binding = Load(dto.data, dto.boundNodeId);
            AddBinding(binding, dto.boundNodeId);
        }

        public void AddBinding(AbstractBinding binding, ulong boundNodeId)
        {
            if (binding is not null)
                Bindings[boundNodeId] = binding;

            if (Bindings.Count > 0 && AreBindingsActivated && bindingCoroutine is null)
                bindingCoroutine = coroutineService.AttachCoroutine(BindingCoroutine());
        }

        public AbstractBinding Load(AbstractBindingDataDto dto, ulong boundNodeId)
        {
            switch (dto)
            {
                case AbstractSimpleBindingDataDto simpleBindingDto:
                    {
                        UMI3DNodeInstance node = environmentService.GetNodeInstance(boundNodeId);
                        return new NodeBinding(simpleBindingDto, node.transform);
                    }
                case MultiBindingDataDto multiBindingDataDto:
                    {
                        UMI3DNodeInstance node = environmentService.GetNodeInstance(boundNodeId);
                        IEnumerable<(AbstractSimpleBinding binding, bool partialFit)> orderedBindingData = multiBindingDataDto.Bindings
                                                                                            .OrderByDescending(x => x.priority)
                                                                                            .Select(x => (new NodeBinding(x, node.transform) as AbstractSimpleBinding,
                                                                                                                                x.partialFit));

                        AbstractSimpleBinding[] orderedBindings = orderedBindingData.Select(x => x.binding).ToArray();
                        bool[] partialFits = orderedBindingData.Select(x => x.partialFit).ToArray();

                        return new MultiBinding(orderedBindings, partialFits, node.transform);
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Coroutine that executes each binding in no particular order.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BindingCoroutine()
        {
            while (AreBindingsActivated)
            {
                foreach (var binding in Bindings)
                {
                    binding.Value.Apply(out bool success);
                    if (!success)
                        yield break;
                }
                yield return null;
            }
        }

        public void RemoveBinding(RemoveBindingDto dto)
        {
            if (Bindings.ContainsKey(dto.boundNodeId))
            {
                Bindings.Remove(dto.boundNodeId);
                if (Bindings.Count == 0 && bindingCoroutine is not null)
                    bindingCoroutine = coroutineService.AttachCoroutine(BindingCoroutine());
            }
        }
    }
}