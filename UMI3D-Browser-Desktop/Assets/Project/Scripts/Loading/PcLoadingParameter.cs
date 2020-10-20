/*
Copyright 2019 Gfi Informatique

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
using umi3d.common;
using UnityEngine;

public class PcLoadingParameter : umi3d.cdk.UMI3DLoadingParameters
{

    /// <see cref="AbstractUMI3DLoadingParameters.ChooseVariant(AssetLibraryDto)"/>
    public override UMI3DLocalAssetDirectory ChooseVariant(AssetLibraryDto assetLibrary)
    {
        UMI3DLocalAssetDirectory res = null;
        foreach (var assetDir in assetLibrary.variants)
        {
            if ((res == null) || (assetDir.metrics.resolution > res.metrics.resolution))
            {
                res = assetDir;
            }
        }
        return res;
    }

    /// <see cref="AbstractUMI3DLoadingParameters.ChooseVariante(List{FileDto})"/>
    public override FileDto ChooseVariante(List<FileDto> files)
    {
        FileDto res = null;
        foreach (var file in files)
        {
            if ((res == null) || (file.metrics.resolution > res.metrics.resolution))
            {
                res = file;
            }
        }
        return res;
    }
}
