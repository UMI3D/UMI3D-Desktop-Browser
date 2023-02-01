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

using inetum.unityUtils;
using System.Linq;
using System.Threading.Tasks;
using umi3d.common;
using umi3d.common.userCapture;
using UnityEngine;

namespace umi3d.cdk.userCapture
{
    /// <summary>
    /// Loader called to load <see cref="UMI3DSkeletonNodeDto"/>.
    /// </summary>
    /// It is based upon the <see cref="UMI3DMeshNodeDto"/> as the animation ressources are packaged in a bundle just like in a model.
    public class UM3DSkeletonNodeLoader : UMI3DMeshNodeLoader
    {
        /// <inheritdoc/>
        public override bool CanReadUMI3DExtension(ReadUMI3DExtensionData data)
        {
            return data.dto is UMI3DSkeletonNodeDto && base.CanReadUMI3DExtension(data);
        }

        /// <inheritdoc/>
        public override async Task ReadUMI3DExtension(ReadUMI3DExtensionData data)
        {
            if (data.dto is not UMI3DSkeletonNodeDto nodeDto)
                throw new Umi3dException($"DTO should be an {GetType()}");

            await base.ReadUMI3DExtension(data);

            FileDto fileToLoad = UMI3DEnvironmentLoader.Parameters.ChooseVariant(nodeDto.mesh.variants);  //! mesh = ressource
            IResourcesLoader loader = UMI3DEnvironmentLoader.Parameters.SelectLoader(fileToLoad.extension);
            
            if (loader != null)
            {
                var o = await UMI3DResourcesManager.LoadFile(nodeDto.id, fileToLoad, loader);
                if (o is GameObject go)
                    await Task.Run(() =>
                    {
                        var modelTracker = data.node.GetOrAddComponent<ModelTracker>();

                        SkeletonMapper skeletonMapper = go.GetComponent<SkeletonMapper>();
                        if (skeletonMapper == null) //? hopefully not necessary because it would imply to rebind everything
                        {
                            skeletonMapper = go.GetComponent<SkeletonMapper>();
                            skeletonMapper.BoneAnchor = go.GetComponent<UMI3DClientUserTrackingBone>().boneType;
                        }

                        AnimationSkeleton animationSkeleton = new(skeletonMapper);
                        PersonalSkeleton.Instance.Skeletons.Add(animationSkeleton);

                        go.layer = LayerMask.NameToLayer("Invisible"); // should not see AnimationSkeletons

                        //if (go.TryGetComponent(out Animator animator)) //! may delete that if already done in base class
                        //    modelTracker.animatorsToRebind.Add(animator);
                    });
                else
                    throw (new Umi3dException($"Cast not valid for {o.GetType()} into GameObject or {data.dto.GetType()} into UMI3DSkeletonNodeDto"));
            }
            else
                throw (new Umi3dException($"No loader found for {fileToLoad.extension}"));
        }
    }
}