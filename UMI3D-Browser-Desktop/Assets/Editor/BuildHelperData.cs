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
using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//[CreateAssetMenu(fileName = "BuildHelperData", menuName = "Build Helper/Build Helper Data", order = 1)]
public class BuildHelperData : ScriptableObject
{
    public string BuildFolderPath;
    public string InstallerFilePath;
    public string LicenseFilePath;
    public bool display;
}

[CustomEditor(typeof(BuildHelperData))]
public class BuildHelperDataEditor : Editor
{

    BuildHelperData data;
    static bool showTileEditor = false;

    public void OnEnable()
    {
        data = (BuildHelperData)target;
    }

    public override void OnInspectorGUI()
    {
        data.display = EditorGUILayout.BeginFoldoutHeaderGroup(data.display,"Paths");
        if (data.display)
        {
            bool changed = false;
            EditorGUILayout.LabelField("Build Folder");
            EditorGUILayout.BeginHorizontal();
            data.BuildFolderPath = TextField(data.BuildFolderPath, ref changed);
            if (GUILayout.Button("Browse"))
            {
                data.BuildFolderPath = EditorUtility.OpenFolderPanel("Build folder", data.BuildFolderPath, "");
                changed = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Installer");
            EditorGUILayout.BeginHorizontal();
            data.InstallerFilePath = TextField(data.InstallerFilePath, ref changed);
            if (GUILayout.Button("Browse"))
            {
                var dir = string.IsNullOrEmpty(data.InstallerFilePath) ? "" : System.IO.Path.GetDirectoryName(data.InstallerFilePath);
                data.InstallerFilePath = EditorUtility.OpenFilePanel("Installer", dir, "iss");
                changed = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("License");
            EditorGUILayout.BeginHorizontal();
            data.LicenseFilePath = TextField(data.LicenseFilePath, ref changed);
            if (GUILayout.Button("Browse"))
            {
                var dir = string.IsNullOrEmpty(data.LicenseFilePath) ? "" : System.IO.Path.GetDirectoryName(data.LicenseFilePath);
                data.LicenseFilePath = EditorUtility.OpenFilePanel("License", dir, "txt");
                changed = true;
            }
            EditorGUILayout.EndHorizontal();
            if (changed)
                EditorUtility.SetDirty(data);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    string TextField(string text,ref bool changed) {
        var _text = EditorGUILayout.TextField(text);
        if(text != _text)
            changed = true;
        return _text;
    }

}

#endif