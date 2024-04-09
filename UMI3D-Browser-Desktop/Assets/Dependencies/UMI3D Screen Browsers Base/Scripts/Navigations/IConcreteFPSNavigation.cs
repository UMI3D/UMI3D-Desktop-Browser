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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.baseBrowser.Navigation
{
    public interface IConcreteFPSNavigation
    {
        /// <summary>
        /// Rotates camera.
        /// </summary>
        void HandleView();
        /// <summary>
        /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravity.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="height"></param>
        void Walk(ref Vector2 move, ref float height);

        bool Update();

        /// <summary>
        /// Return a {0;1} <see cref="Vector3"/> representing a movement.<br/>
        /// <list type="bullet">
        /// <item>x: Left to right (positive: right)</item>
        /// <item>y: Down to up (positive: up)</item>
        /// <item>z: back to front (positive: front)</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        Vector3 HandleUserInput();
    }
}
