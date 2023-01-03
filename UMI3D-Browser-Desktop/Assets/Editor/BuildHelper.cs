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

public class BuildHelper : InitedWindow<BuildHelper>
{
    const string _scriptableFolderPath = "EXCLUDED";
    const string scriptablePathNoExt = "Assets/" + _scriptableFolderPath + "/BuildHelperData";
    const string scriptablePath = scriptablePathNoExt + ".asset";
    //static string scriptablePath => Application.dataPath + _scriptablePath;

    const string owner = "UMI3D";
    const string repo = "UMI3D-Desktop-Browser";

    const string filename = "BuildHelperData";
    ScriptableLoader<BuildHelperData> _data;
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
            ("major",() => BrowserVersion.major),
            ("minor", () => BrowserVersion.minor),
            ("buildCount", () => BrowserVersion.buildCount),
            ("date", () => BrowserVersion.date)
            );

        _data = new ScriptableLoader<BuildHelperData>(filename);
        info = new LogScrollView();
        RefreshBranch();
    }

    protected override void Draw()
    {
        
        GUI.enabled = !isBuilding;

        _data.editor?.OnInspectorGUI();


        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Current Branch");
        EditorGUILayout.LabelField(_data.data.Branch);
        if (GUILayout.Button("Refresh Branch"))
            RefreshBranch();

        EditorGUILayout.EndHorizontal();



        GUILayout.Label("Build Version", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        version.Draw();

        EditorGUILayout.Separator();
        try
        {
            EditorGUILayout.LabelField(CommitMessage);
        }
        catch(Exception e) { UnityEngine.Debug.LogException(e); }
        EditorGUILayout.LabelField(CompatibleUmi3dVersion);
        EditorGUILayout.Separator();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build but not push"))
            CleanComputeBuild(false, false);
        if (GUILayout.Button("Update StandardAsset And Build"))
            CleanComputeBuild(false);
        if (GUILayout.Button("Clean All And Build"))
            CleanComputeBuild(true);
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;

        info.Draw();
    }

    async void RefreshBranch()
    {
        _data.data.Branch = await Git.GetBranchName();
    }

    async void CleanComputeBuild(bool cleanAll, bool comit = true)
    {
        isBuilding = true;
        try
        {
            info.Clear();
            info.NewTitle($"Build Browser");
            version.UpdateVersion();

            CleanAndCopyBuildFolder(cleanAll, _data.data.BuildFolderPath);

            //update Setup
            var outputFile = Iscc.UpdateInstaller(_data.data.InstallerFilePath, version.version, "Setup_UMI3D_Browser_(.*)?", $"Setup_UMI3D_Browser_{version.version}");

            // Build player.
            await Build(_data.data.BuildFolderPath);

            info.NewTitle($"Create Installer");

            await Iscc.ExecuteISCC("C:/Program Files (x86)/Inno Setup 6/ISCC.exe", _data.data.InstallerFilePath, info.NewLine, info.NewError);

            if (comit)
            {
                info.NewTitle($"Commit");

                await Git.CommitAll($"{CommitMessage} {CompatibleUmi3dVersion}", info.NewLine, info.NewError);

                info.NewTitle($"Release");

                ReleaseBrowser.Release(_data.data.Token, version.version, _data.data.Branch, new System.Collections.Generic.List<(string path, string name)> { outputFile }, CompatibleUmi3dVersion, owner, repo);
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
        File.Copy(_data.data.LicenseFilePath, buildFolder + "/license.txt", true);
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