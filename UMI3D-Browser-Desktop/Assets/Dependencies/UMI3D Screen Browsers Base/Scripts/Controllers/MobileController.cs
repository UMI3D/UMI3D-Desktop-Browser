/*
Copyright 2019 - 2022 Inetum

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
using System.Collections.Generic;
using umi3d.baseBrowser.Controller;
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.mobileBrowser.Controller
{
    public class MobileController : IConcreteController
    {
        public List<AbstractUMI3DInput> Inputs
        {
            get {
                List<AbstractUMI3DInput> list = new List<AbstractUMI3DInput>();
                //list.AddRange(ManipulationInputs);
                return list;
            }
        }

        public void Awake()
        {
            //interactions.MainMobileAction mainAction = GetComponentInChildren<interactions.MainMobileAction>();
            //EventInputs.Add(mainAction);
            //mainAction.Init(this);
            //mainAction.bone = interactionBoneType;
            //mainAction.Menu = ObjectMenu.menu;
        }
        public void Start()
        {

        }
        public void Update()
        {

        }

        //public AbstractUMI3DInput FindInput(ManipulationDto manip, DofGroupDto dof, bool unused = true)
        //{
        //    Debug.Log("TODO : Find input for manipulation dto");
        //    //ManipulationGroup group = ManipulationInputs.Find(i => i.IsAvailableFor(manip));
        //    //if (group == null)
        //    //{
        //    //    group = ManipulationGroup.Instanciate(this, ManipulationActionInput, dofGroups, transform);
        //    //    if (group == null)
        //    //    {
        //    //        Debug.LogWarning("find manip input FAILED");
        //    //        return null;
        //    //    }
        //    //    group.bone = interactionBoneType;
        //    //    ManipulationInputs.Add(group);
        //    //}
        //    return null;
        //}

        public AbstractUMI3DInput FindInput(EventDto evt, bool unused = true, bool tryToFindInputForHoldableEvent = false)
            => null;

        ///// <summary>
        ///// <inheritdoc/>
        ///// </summary>
        ///// <param name="tool"></param>
        ///// <param name="releasable"></param>
        ///// <param name="reason"></param>
        ///// <param name="hoveredObjectId"></param>
        //public override void Project(AbstractTool tool, bool releasable, InteractionMappingReason reason, ulong hoveredObjectId)
        //{
        //    if (reason is RequestedByEnvironment)
        //    {
        //        interactions.MainMobileAction mainAction = EventInputs.Find(i => i is umi3d.mobileBrowser.interactions.MainMobileAction) as umi3d.mobileBrowser.interactions.MainMobileAction;
        //        if (mainAction != null) mainAction.ForceDissociate();
        //    }
        //    base.Project(tool, releasable, reason, hoveredObjectId);
        //}
    }
}