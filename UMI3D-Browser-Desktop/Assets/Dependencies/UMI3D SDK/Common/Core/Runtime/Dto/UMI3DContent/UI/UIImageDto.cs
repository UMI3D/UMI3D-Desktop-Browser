﻿/*
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

namespace umi3d.common
{
    /// <summary>
    /// DTO describing an Image in the UI.
    /// </summary>
    [System.Serializable]
    public class UIImageDto : UIRectDto
    {
        /// <summary>
        /// Tint color.
        /// </summary>
        public ColorDto color { get; set; }

        /// <summary>
        /// Sprite (2D graphic object constructed from a bitmap image) ressource file.
        /// </summary>
        public ResourceDto sprite { get; set; }

        /// <summary>
        /// type of way to display the image.
        /// </summary>
        public ImageType type { get; set; }
    }
}