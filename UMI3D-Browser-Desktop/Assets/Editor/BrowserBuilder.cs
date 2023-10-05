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

#if UNITY_EDITOR

using BrowserDesktop;
using inetum.unityUtils;
using inetum.unityUtils.editor;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using umi3d;
using umi3d.cdk;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BrowserBuilder : InitedWindow<BrowserBuilder>
{
    const string owner = "UMI3D";
    const string repo = "UMI3D-Desktop-Browser";

    const string filename = "BrowserBuilderData";
    ScriptableLoader<BrowserBuilderData> data;
    VersionGUI version;
    const string browserVersionPath = @"\Project\Scripts\BrowserVersion.cs";

    string CommitMessage => $"Build v{version.version}";
    string CompatibleUmi3dVersion = $"Compatible with UMI3D SDK {UMI3DVersion.version}";

    LogScrollView info;
    bool isBuilding = false;

    [MenuItem("Browser/Build")]
    static void Open()
    {
        OpenWindow();
    }

    protected override void Init()
    {
        version = new VersionGUI(
            Application.dataPath + browserVersionPath,
            "I.I.I.yyMMdd",
            () => BrowserVersion.Version,
            ("major",(s) => BrowserVersion.major),
            ("minor", (s) => BrowserVersion.minor),
            ("buildCount", (s) => BrowserVersion.buildCount),
            ("date", (s) => BrowserVersion.date)
            );

        data = new ScriptableLoader<BrowserBuilderData>(filename);
        info = new LogScrollView(data.data);
        RefreshBranch();
    }

    protected override void Draw()
    {
        GUI.enabled = !isBuilding;
        data?.editor?.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current Branch");
        try
        {
            EditorGUILayout.LabelField(data.data.Branch);
        }
        catch { }
        if (GUILayout.Button("Refresh Branch"))
            RefreshBranch();
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Build Version", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        try
        {
            version.Draw();
        }
        catch { }

        EditorGUILayout.Separator();
        try
        {
            EditorGUILayout.LabelField(CommitMessage);
            EditorGUILayout.LabelField(CompatibleUmi3dVersion);
        }
        catch { }

        EditorGUILayout.Separator();


        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        try
        {
            if (GUILayout.Button("Build but don't push"))
                CleanComputeBuild(false, false);
            if (GUILayout.Button($"Build and push on {data.data.Branch}"))
                CleanComputeBuild(false);
            if (GUILayout.Button($"Clean All, Build and push on {data.data.Branch}"))
                CleanComputeBuild(true);
        }
        catch { }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;

        info.Draw();
    }

    async void RefreshBranch()
    {
        data.data.Branch = await Git.GetBranchName();
    }


    protected override void Reinit()
    {
        base.Reinit();
        bool cleanAll = data.data.cleanAll, comit = data.data.comitAll;
        CleanComputeBuild2(cleanAll, comit);
    }

    async void CleanComputeBuild(bool cleanAll, bool comit = true)
    {
        isBuilding = true;
        try
        {
            info.Clear();
            info.NewTitle($"Build Browser");

            data.data.cleanAll = cleanAll;
            data.data.comitAll = comit;

            SetWaitForReinit();

            await Task.Yield();

            var update = version.UpdateVersion();

            await Task.Yield();
            AssetDatabase.Refresh();

            await Task.Yield();
            await Task.Yield();

            if (EditorApplication.isCompiling)
                return;

            //should not go there when rebuild.
            UnityEngine.Debug.Log("here");

            SetWaitForReinit(false);

            await Task.Yield();

            CleanComputeBuild2(cleanAll, comit);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        isBuilding = false;
    }

    async void CleanComputeBuild2(bool cleanAll, bool comit = true)
    {
        isBuilding = true;
        try
        {
            CleanAndCopyBuildFolder(cleanAll, data.data.BuildFolderPath);

            //update Setup
            var outputFile = Iscc.UpdateInstaller(data.data.InstallerFilePath, version.version, "Setup_UMI3D_Browser_(.*)?", $"Setup_UMI3D_Browser_{version.version}");

            var text = info.text;
            // Build player.
            await Build(data.data.BuildFolderPath);

            ReInit();
            info.text = text;

            info.NewTitle($"Create Installer");
            info.NewLine($"Skipped : todo manualy");
            //await Iscc.ExecuteISCC("C:/Program Files (x86)/Inno Setup 6/ISCC.exe", data.data.InstallerFilePath, info.NewLine, info.NewError);

            if (comit)
            {
                info.NewTitle($"Commit");

                await Git.CommitAll($"{CommitMessage} {CompatibleUmi3dVersion}", info.NewLine, info.NewError);

                info.NewTitle($"Release");

                var url = await ReleaseBrowser.Release(data.data.Token, version.version, data.data.Branch, new System.Collections.Generic.List<(string path, string name)> { outputFile }, CompatibleUmi3dVersion, owner, repo);

                Application.OpenURL(url);
            }
            //Open folder
            Command.OpenFile(outputFile.path);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        isBuilding = false;
    }

    #region BuildUtils

    void CleanAndCopyBuildFolder(bool cleanAll, string buildFolder)
    {
        if (cleanAll)
        {
            if (Directory.Exists(buildFolder))
                Directory.Delete(buildFolder, true);
            Directory.CreateDirectory(buildFolder);
        }
        File.Copy(data.data.LicenseFilePath, buildFolder + "/license.txt", true);
    }

    async Task Build(string buildFolder)
    {
        string[] levels = new string[] { "Assets/Project/Scenes/Connection.unity", "Assets/Project/Scenes/Environment.unity" };
        BuildPipeline.BuildPlayer(levels, buildFolder + "/UMI3D-Browser-Desktop.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

        while (BuildPipeline.isBuildingPlayer)
            await Task.Yield();
    }

    #endregion
}
#endif