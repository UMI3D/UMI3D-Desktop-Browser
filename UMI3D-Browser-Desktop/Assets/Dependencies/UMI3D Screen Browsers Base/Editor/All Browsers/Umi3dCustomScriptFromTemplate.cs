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
using UnityEditor;

namespace umi3d.browserEditor.utils
{
    /// <summary>
    /// Static class that provide methods to create a script from a template.
    /// </summary>
    public static class Umi3dCustomScriptFromTemplate
    {
        public static string path;
        public static string partialPath;

        static Umi3dCustomScriptFromTemplate()
        {
            string fileName = $"{nameof(Umi3dCustomScriptFromTemplate)}";
            var assets = AssetDatabase.FindAssets($"t:Script {fileName}");

            path = AssetDatabase.GUIDToAssetPath(assets[0]);
            partialPath = path.Substring(0, path.Length - fileName.Length - 3);
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Class", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3dClassTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile
            (
                $"{partialPath}/Umi3dClassTemplate.txt",
                "Umi3dClass.cs"
            );
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Struct", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DStructTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile
            (
                $"{partialPath}/Umi3dStructTemplate.txt",
                "Umi3dStruct.cs"
            );
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Enum", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DEnumTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile
            (
                $"{partialPath}/Umi3dEnumTemplate.txt",
                "Umi3dEnum.cs"
            );
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Interface", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DInterfaceTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile
            (
                $"{partialPath}/Umi3dInterfaceTemplate.txt",
                "Umi3dInterface.cs"
            );
        }
    }
}
