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
using umi3d.cdk.interaction;
using UnityEngine;
using static umi3d.baseBrowser.Controller.BaseController;

namespace umi3d.baseBrowser.Controller
{
    public struct CursorData
    {
        public bool ForceProjection, ForceProjectionReleasable;
        public ButtonMenuItem ForceProjectionReleasableButton;

        public Interactable LastProjected, OldHovered, CurrentHovered;
        public ulong LastHoveredId, CurrentHoveredId;
        public Transform CurrentHoveredTransform;

        public Vector3 LastPosition, LastNormal, LastDirection;
        public Vector3 Position, Normal, Direction;
        public Vector3 CenteredWorldPosition, WorldPosition, WorldNormal, WorlDirection;

        public HoverState HoverState;

        public int saveDelay;

        public void Save()
        {
            if (saveDelay > 0) saveDelay--;
            else
            {
                if (saveDelay < 0) saveDelay = 0;
                OldHovered = CurrentHovered;
                LastHoveredId = CurrentHoveredId;
                CurrentHovered = null;
                CurrentHoveredTransform = null;
                CurrentHoveredId = 0;
                LastPosition = Position;
                LastNormal = Normal;
                LastDirection = Direction;
            }
        }

        public bool IsDelaying() => saveDelay > 0;
    }
}
