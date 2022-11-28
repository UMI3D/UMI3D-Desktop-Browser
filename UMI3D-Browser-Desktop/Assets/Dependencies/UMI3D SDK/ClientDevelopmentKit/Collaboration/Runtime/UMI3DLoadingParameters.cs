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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using umi3d.cdk.interaction;
using umi3d.cdk.userCapture;
using umi3d.cdk.volumes;
using umi3d.common;
using umi3d.common.interaction;
using umi3d.common.userCapture;
using umi3d.common.volume;
using UnityEngine;

namespace umi3d.cdk
{
    /// <summary>
    /// Loading helper.
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultLoadingParameters", menuName = "UMI3D/Default Loading Parameters")]
    public class UMI3DLoadingParameters : AbstractUMI3DLoadingParameters
    {

        private const DebugScope scope = DebugScope.CDK | DebugScope.Collaboration | DebugScope.Loading;

        [ConstEnum(typeof(UMI3DAssetFormat), typeof(string))]
        public List<string> supportedformats = new List<string>();
        public float maximumResolution;

        public virtual UMI3DNodeLoader nodeLoader { get; } = new UMI3DNodeLoader();
        public virtual UMI3DAbstractAnchorLoader AnchorLoader { get; protected set; } = null;
        public virtual UMI3DAvatarNodeLoader avatarLoader { get; } = new UMI3DAvatarNodeLoader();

        public NotificationLoader notificationLoader;

        [SerializeField]
        private Material _skyboxMaterial;
        public Material skyboxMaterial { get { if (_skyboxMaterial == null) { _skyboxMaterial = new Material(RenderSettings.skybox); RenderSettings.skybox = _skyboxMaterial; } return _skyboxMaterial; } }

        public List<IResourcesLoader> ResourcesLoaders { get; } = new List<IResourcesLoader>() { new ObjMeshDtoLoader(), new ImageDtoLoader(), new GlTFMeshDtoLoader(), new BundleDtoLoader(), new AudioLoader() };

        public List<AbstractUMI3DMaterialLoader> MaterialLoaders { get; } = new List<AbstractUMI3DMaterialLoader>() { new UMI3DExternalMaterialLoader(), new UMI3DPbrMaterialLoader(), new UMI3DOriginalMaterialLoader() };

        AbstractLoader loader;
        public virtual void Init()
        {
            (loader = new EntityGroupLoader())
            .SetNext(new UMI3DAnimationLoader())
            .SetNext(new PreloadedSceneLoader())
            .SetNext(new UMI3DInteractableLoader())
            .SetNext(new UMI3DGlobalToolLoader())
            .SetNext(new UMI3DMeshNodeLoader())
            .SetNext(new UMI3DLineRendererLoader())
            .SetNext(new UMI3DSubMeshNodeLoader())
            .SetNext(new UMI3DVolumeLoader())
            .SetNext(new UMI3DUINodeLoader())
            .SetNext(avatarLoader)
            .SetNext(new UMI3DHandPoseLoader())
            .SetNext(new UMI3DEmotesConfigLoader())
            .SetNext(new UMI3DEmoteLoader())
            .SetNext(notificationLoader.GetNotificationLoader())
            .SetNext(new UMI3DNodeLoader())
            .SetNext(UMI3DEnvironmentLoader.Instance.nodeLoader)
            ;
        }

        protected AbstractLoader GetLoader()
        {
            if (loader == null)
                Init();
            return loader;
        }

        /// <summary>
        /// Load an UMI3D Object.
        /// </summary>
        public override async Task ReadUMI3DExtension(ReadUMI3DExtensionData data)
        {
            await GetLoader().Handle(data);
            if (AnchorLoader != null)
                await AnchorLoader.Handle(data);
        }

        /// <inheritdoc/>
        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyData data)
        {
            var b = await GetLoader().Handle(data);
            if (AnchorLoader != null)
                await AnchorLoader.Handle(data);
            return b;
        }

        /// <inheritdoc/>
        public override async Task<bool> SetUMI3DProperty(SetUMI3DPropertyContainerData data)
        {
            var b = await GetLoader().Handle(data);
            if (AnchorLoader != null)
                await AnchorLoader.Handle(data);
            return b;
        }

        /// <inheritdoc/>
        public override async Task<bool> ReadUMI3DProperty(ReadUMI3DPropertyData data)
        {
            var b = await GetLoader().Handle(data);
            if (AnchorLoader != null)
                await AnchorLoader.Handle(data);
            return b;
        }

        /// <inheritdoc/>
        public override UMI3DLocalAssetDirectory ChooseVariant(AssetLibraryDto assetLibrary)
        {
            UMI3DLocalAssetDirectory res = null;
            foreach (UMI3DLocalAssetDirectory assetDir in assetLibrary.variants)
            {
                bool ok = res == null;
                if (!ok && !assetDir.formats.Any(f => !supportedformats.Contains(f)))
                {
                    if (res.formats.Any(f => !supportedformats.Contains(f)))
                        ok = true;
                    else
                        ok = Compare(assetDir.metrics.resolution, res.metrics.resolution, maximumResolution);
                }

                if (ok)
                {
                    res = assetDir;
                }
            }
            return res;
        }

        /// <summary>
        /// Is <paramref name="a"/> bigger than <paramref name="b"/> and inferior than <paramref name="max"/>?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="max">maximum, 0 mean no maximum</param>
        /// <returns></returns>
        private bool Compare(float a, float b, float max)
        {
            if (max <= 0) return a > b;
            if (b > max) return b > a;
            if (a < max) return a > b;
            return false;
        }

        /// <inheritdoc/>
        public override FileDto ChooseVariant(List<FileDto> files)
        {
            FileDto res = null;
            if (files != null)
            {
                foreach (FileDto file in files)
                {
                    bool ok = res == null;
                    if (!ok && supportedformats.Contains(file.format))
                    {
                        if (!supportedformats.Contains(res.format))
                            ok = true;
                        else
                            ok = Compare(file.metrics.resolution, res.metrics.resolution, maximumResolution);

                    }
                    if (ok)
                    {
                        res = file;
                    }
                }
            }

            return res;
        }

        /// <inheritdoc/>
        public override IResourcesLoader SelectLoader(string extension)
        {
            foreach (IResourcesLoader loader in ResourcesLoaders)
            {
                if (loader.IsSuitableFor(extension))
                    return loader;
                if (loader.IsToBeIgnored(extension))
                    return null;
            }
            throw new Umi3dException("there is no compatible loader for this extention : " + extension);
        }

        /// <inheritdoc/>
        public override AbstractUMI3DMaterialLoader SelectMaterialLoader(GlTFMaterialDto gltfMatDto)
        {
            foreach (AbstractUMI3DMaterialLoader loader in MaterialLoaders)
            {
                if (loader.IsSuitableFor(gltfMatDto))
                    return loader;
            }
            UMI3DLogger.LogError("there is no compatible material loader for this material.", scope);
            return null;
        }

        /// <inheritdoc/>
        public override async void loadSkybox(ResourceDto skybox)
        {
            try
            {
                FileDto fileToLoad = ChooseVariant(skybox.variants);
                if (fileToLoad == null) return;
                string ext = fileToLoad.extension;
                IResourcesLoader loader = SelectLoader(ext);
                if (loader != null)
                {
                    var o = await UMI3DResourcesManager.LoadFile(UMI3DGlobalID.EnvironementId, fileToLoad, loader);
                    var tex = (Texture2D)o;
                    if (tex != null)
                    {

                        Cubemap cube;
                        Color[] imageColors;

                        //prerequises: 
                        // 1) image is in format
                        //     +y
                        //  -x +z +x -z
                        //     -Y
                        // 2) faces are cubes

                        int size = tex.width / 4;
                        cube = new Cubemap(size, TextureFormat.RGB24, false);

                        //Need to invert y ? Oo
                        var buffer = new Texture2D(tex.width, tex.height);
                        buffer.SetPixels(tex.GetPixels());
                        for (int x = 0; x < tex.width; x++)
                        {
                            for (int y = 0; y < tex.height; y++)
                                tex.SetPixel(x, y, buffer.GetPixel(x, tex.height - 1 - y));
                        }

                        imageColors = tex.GetPixels(size, 0, size, size);
                        cube.SetPixels(imageColors, CubemapFace.PositiveY);

                        imageColors = tex.GetPixels(0, size, size, size);
                        cube.SetPixels(imageColors, CubemapFace.NegativeX);

                        imageColors = tex.GetPixels(size, size, size, size);
                        cube.SetPixels(imageColors, CubemapFace.PositiveZ);

                        imageColors = tex.GetPixels(size * 2, size, size, size);
                        cube.SetPixels(imageColors, CubemapFace.PositiveX);

                        imageColors = tex.GetPixels(size * 3, size, size, size);
                        cube.SetPixels(imageColors, CubemapFace.NegativeZ);

                        imageColors = tex.GetPixels(size, size * 2, size, size);
                        cube.SetPixels(imageColors, CubemapFace.NegativeY);

                        cube.Apply();
                        skyboxMaterial.SetTexture("_Tex", cube);
                        RenderSettings.skybox = skyboxMaterial;
                    }
                    else
                    {
                        UMI3DLogger.LogWarning($"invalid cast from {o.GetType()} to {typeof(Texture2D)}", scope);
                    }
                }
            }
            catch(Exception e)
            {
                UMI3DLogger.LogException(e,scope);
            }
        }

        /// <inheritdoc/>
        public override Task UnknownOperationHandler(AbstractOperationDto operation)
        {
            switch (operation)
            {
                case SwitchToolDto switchTool:
                    AbstractInteractionMapper.Instance.SwitchTools(switchTool.toolId, switchTool.replacedToolId, switchTool.releasable, 0, new interaction.RequestedByEnvironment());
                    break;
                case ProjectToolDto projection:
                    AbstractInteractionMapper.Instance.SelectTool(projection.toolId, projection.releasable, 0, new interaction.RequestedByEnvironment());
                    break;
                case ReleaseToolDto release:
                    AbstractInteractionMapper.Instance.ReleaseTool(release.toolId, new interaction.RequestedByEnvironment());
                    break;
                case SetTrackingTargetFPSDto setTargetFPS:
                    UMI3DClientUserTracking.Instance.SetFPSTarget(setTargetFPS.targetFPS);
                    break;
                case SetStreamedBonesDto streamedBones:
                    UMI3DClientUserTracking.Instance.SetStreamedBones(streamedBones.streamedBones);
                    break;
                case SetSendingCameraPropertiesDto sendingCamera:
                    UMI3DClientUserTracking.Instance.SetCameraPropertiesSending(sendingCamera.activeSending);
                    break;
                case SetSendingTrackingDto sendingTracking:
                    UMI3DClientUserTracking.Instance.SetTrackingSending(sendingTracking.activeSending);
                    break;
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task UnknownOperationHandler(uint operationId, ByteContainer container)
        {
            ulong id;
            bool releasable;

            switch (operationId)
            {
                case UMI3DOperationKeys.SwitchTool:
                    id = UMI3DNetworkingHelper.Read<ulong>(container);
                    ulong oldid = UMI3DNetworkingHelper.Read<ulong>(container);
                    releasable = UMI3DNetworkingHelper.Read<bool>(container);
                    AbstractInteractionMapper.Instance.SwitchTools(id, oldid, releasable, 0, new interaction.RequestedByEnvironment());
                    break;
                case UMI3DOperationKeys.ProjectTool:
                    id = UMI3DNetworkingHelper.Read<ulong>(container);
                    releasable = UMI3DNetworkingHelper.Read<bool>(container);
                    AbstractInteractionMapper.Instance.SelectTool(id, releasable, 0, new interaction.RequestedByEnvironment());
                    break;
                case UMI3DOperationKeys.ReleaseTool:
                    id = UMI3DNetworkingHelper.Read<ulong>(container);
                    AbstractInteractionMapper.Instance.ReleaseTool(id, new interaction.RequestedByEnvironment());
                    break;
                case UMI3DOperationKeys.SetUTSTargetFPS:
                    int target = UMI3DNetworkingHelper.Read<int>(container);
                    UMI3DClientUserTracking.Instance.SetFPSTarget(target);
                    break;
                case UMI3DOperationKeys.SetStreamedBones:
                    List<uint> streamedBones = UMI3DNetworkingHelper.ReadList<uint>(container);
                    UMI3DClientUserTracking.Instance.SetStreamedBones(streamedBones);
                    break;
                case UMI3DOperationKeys.SetSendingCameraProperty:
                    bool sendCamera = UMI3DNetworkingHelper.Read<bool>(container);
                    UMI3DClientUserTracking.Instance.SetCameraPropertiesSending(sendCamera);
                    break;
                case UMI3DOperationKeys.SetSendingTracking:
                    bool sendTracking = UMI3DNetworkingHelper.Read<bool>(container);
                    UMI3DClientUserTracking.Instance.SetTrackingSending(sendTracking);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}