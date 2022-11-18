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

public class BuildHelper : EditorWindow
{
    const string _scriptableFolderPath = "EXCLUDED";
    const string scriptablePathNoExt = "Assets/" + _scriptableFolderPath + "/BuildHelperData";
    const string scriptablePath = scriptablePathNoExt + ".asset";
    //static string scriptablePath => Application.dataPath + _scriptablePath;

    BuildHelperData _data;

    public string old => BrowserVersion.Version;
    public int major = 1;
    public int minor = 2;
    public int count = 3;
    public DateTime _date;
    public string date => _date.ToString("yyMMdd");

    class VersionRegex {
        Regex reg;
        string pattern;
        Func<string> replacement;

        public VersionRegex(string part1, string part2, Func<object> content)
        {
            pattern = part1 + "(.*)" + part2;
            replacement = () =>
            {
                var c = content();
                return part1 + c.ToString() + part2;
            };
            reg = new Regex(pattern);
        }

        public string Replace(string text)
        {
            return Regex.Replace(text, pattern, replacement());
        }
    }

    public string newVersion => $"{major}.{minor}.{count}.{date}";


    const string browserVersionPath = @"\Project\Scripts\BrowserVersion.cs";

    VersionRegex patternMajor ;
    VersionRegex patternMinor ;
    VersionRegex patternCount ;
    VersionRegex patternDate ;


    string patternOutputDir = @"OutputDir=(.*)..^OutputBaseFilename=(.*).^Compression=";

    string CommitMessage => $"Build v{newVersion}";
    string CompatibleUmi3dVersion = $"Compatible with UMI3D SDK {UMI3DVersion.version}";



    string info = "";
    Vector2 ScrollPos;
    bool isBuilding = false;
    //\StandardAssets\Changelog

    // Add menu named "My Window" to the Window menu
    [MenuItem("Browser/Build")]
    static void Open()
    {

        // \Assets\Scripts\UI\UXML\
        // Get existing open window or if none, make a new one :
        BuildHelper window = (BuildHelper)EditorWindow.GetWindow(typeof(BuildHelper));
        window.Init();
        window.Show();
    }

    void Init()
    {
        Debug.Log("Init");
         patternMajor = new VersionRegex("string major = \"", "\";", () => major);
         patternMinor = new VersionRegex("string minor = \"", "\";", () => minor);
         patternCount = new VersionRegex("string buildCount = \"", "\";", () => count);
         patternDate = new VersionRegex("string date = \"", "\";", () => date);

        ResetVersion();

        _data = GetScriptable();
        GetEditor();
    }

    void OnGUI()
    {
        GUI.enabled = !isBuilding;

        var editor = GetEditor();
        UnityEngine.Debug.Assert(_data != null);
        UnityEngine.Debug.Assert(editor != null);

        editor?.OnInspectorGUI();

        GUILayout.Label("Build Version", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Old version");
        EditorGUILayout.LabelField(old);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("New version");
        int.TryParse(EditorGUILayout.TextField(major.ToString()), out major);
        int.TryParse(EditorGUILayout.TextField(minor.ToString()), out minor);
        int.TryParse(EditorGUILayout.TextField(count.ToString()), out count);
        var _day = EditorGUILayout.TextField(date);
        SetDate(_day);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Version"))
            ResetVersion();
        if (GUILayout.Button("Major +1"))
            major += 1;
        if (GUILayout.Button("Minor +1"))
            minor += 1;
        if (GUILayout.Button("Count +1"))
            count += 1;
        if (GUILayout.Button("Set To Now"))
            _date = DateTime.Now;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField(CommitMessage);
        EditorGUILayout.LabelField(CompatibleUmi3dVersion);
        EditorGUILayout.Separator();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build but not push"))
            CleanComputeBuild(false,false);
        if (GUILayout.Button("Update StandardAsset And Build"))
            CleanComputeBuild(false);
        if (GUILayout.Button("Clean All And Build"))
            CleanComputeBuild(true);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Test"))
            Test();

        GUI.enabled = true;
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
        GUI.enabled = false;
        EditorGUILayout.TextArea(info);
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
    }

    async void Test()
    {

    }

    async void CleanComputeBuild(bool cleanAll, bool comit = true)
    {
        isBuilding = true;
        try
        {
            info = "";
            UpdateVersion();

            CleanAndCopyBuildFolder(cleanAll, _data.BuildFolderPath);

            //update Setup
            var outputFile = UpdateInstaller();

            // Build player.
            await Build(_data.BuildFolderPath);

            await ExecuteISCC(_data.InstallerFilePath);

            if(comit)
            await CommitAll();
            //Open folder
            OpenFile(outputFile);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        isBuilding = false;
    }

    #region OnGUI Utils
    void SetDate(string date)
    {
        if (!DateTime.TryParseExact($"{date}", "yyMMdd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out _date))
        {
            UnityEngine.Debug.Log($"Error in pasing date : {date} with yyMMdd");
        };
    }

    void ResetVersion()
    {
            int.TryParse(BrowserVersion.major, out major);
            int.TryParse(BrowserVersion.minor, out minor);
            int.TryParse(BrowserVersion.buildCount, out count);
            SetDate(BrowserVersion.date);
        
    }

    #endregion

    #region BuildUtils

    async Task CommitAll()
    {
        string gitCommand = "git";
        string gitAddArgument = @"add .";
        string gitCommitArgument = $"commit -m \"{CommitMessage} {CompatibleUmi3dVersion} \"";
        string gitTagArgument = $"tag -a {newVersion} -m \"{CompatibleUmi3dVersion}\"";
        string gitPushArgument = @"push --follow-tags";

        await ExecuteCommand(gitCommand, gitAddArgument, (s) => info += $"\n{s}", (s) => info += $"\nError : {s}");
        await ExecuteCommand(gitCommand, gitCommitArgument, (s) => info += $"\n{s}", (s) => info += $"\nError : {s}");
        await ExecuteCommand(gitCommand, gitTagArgument, (s) => info += $"\n{s}", (s) => info += $"\nError : {s}");
        await ExecuteCommand(gitCommand, gitPushArgument, (s) => info += $"\n{s}", (s) => info += $"\nError : {s}");

    }

    void UpdateVersion()
    {
        UnityEngine.Debug.Log(Application.dataPath + browserVersionPath);
        string text = File.ReadAllText(Application.dataPath + browserVersionPath);
        UnityEngine.Debug.Log(patternMajor != null);
        text = patternMajor.Replace(text);
        text = patternMinor.Replace(text);
        text = patternCount.Replace(text);
        text = patternDate.Replace(text);

        File.WriteAllText(Application.dataPath + browserVersionPath, text);
    }

    void CleanAndCopyBuildFolder(bool cleanAll, string buildFolder)
    {
        //clean folder
        if (cleanAll)
        {
            if (Directory.Exists(buildFolder))
                Directory.Delete(buildFolder, true);
            Directory.CreateDirectory(buildFolder);
        }

        //copy license
        File.Copy(_data.LicenseFilePath, buildFolder + "/license.txt", true);
    }

    private string UpdateInstaller()
    {
        var InstallerPath = _data.InstallerFilePath;
        string setupText = File.ReadAllText(InstallerPath);
        setupText = Regex.Replace(setupText, "#define MyAppVersion \"(.*)?\"", $"#define MyAppVersion \"{newVersion}\"");
        setupText = Regex.Replace(setupText, "OutputBaseFilename=Setup_UMI3D_Browser_(.*)?", $"OutputBaseFilename=Setup_UMI3D_Browser_{newVersion}");
        File.WriteAllText(InstallerPath, setupText);


        Regex DirReg = new Regex(patternOutputDir, RegexOptions.Multiline | RegexOptions.Singleline);
        var md = DirReg.Match(setupText);
        string path = md.Groups[1].Captures[0].Value + "\\" + md.Groups[2].Captures[0].Value + ".exe";
        return path;
    }

    async Task Build(string buildFolder)
    {
        string[] levels = new string[] { "Assets/Project/Scenes/Connection.unity", "Assets/Project/Scenes/Environement.unity" };
        BuildPipeline.BuildPlayer(levels, buildFolder + "/UMI3D-Browser-Desktop.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        while (BuildPipeline.isBuildingPlayer)
            await Task.Yield();
    }

    static void OpenFile(string path)
    {
        path = path.Replace('/', '\\');

        if (File.Exists(path))
        {
            FileAttributes attr = File.GetAttributes(path);
            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                OpenFileWith("explorer.exe", path, "/root,");
            }
            else
            {
                OpenFileWith("explorer.exe", path, "/select,");
            }
        }
        else
            UnityEngine.Debug.LogError("no file at "+path);
    }

    static void OpenFileWith(string exePath, string path, string arguments)
    {
        if (path == null)
            return;

        try
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            if (exePath != null)
            {
                process.StartInfo.FileName = exePath;
                //Pre-post insert quotes for fileNames with spaces.
                process.StartInfo.Arguments = string.Format("{0}\"{1}\"", arguments, path);
            }
            else
            {
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            }
            if (!path.Equals(process.StartInfo.WorkingDirectory))
            {
                process.Start();
            }
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }

    #endregion

    #region Scriptable Handler

    Editor ScriptableEditor;
    Editor GetEditor()
    {
        if (ScriptableEditor == null)
            ScriptableEditor = Editor.CreateEditor(_data);
        return ScriptableEditor;
    }

    static BuildHelperData GetScriptable() => LoadScriptable() ?? CreateScriptable();

    static BuildHelperData CreateScriptable()
    {
        CreateFolder();
        BuildHelperData asset = ScriptableObject.CreateInstance<BuildHelperData>();
        AssetDatabase.CreateAsset(asset, scriptablePath);
        AssetDatabase.SaveAssets();
        return asset;
    }

    static void CreateFolder()
    {
        if (!System.IO.Directory.Exists(Application.dataPath + System.IO.Path.GetDirectoryName(scriptablePath).TrimStart("Assets".ToCharArray())))
        {
            AssetDatabase.CreateFolder("Assets", _scriptableFolderPath);
        }

    }

    static BuildHelperData LoadScriptable()
    {
        var asset = AssetDatabase.LoadAssetAtPath<BuildHelperData>(scriptablePath);
        UnityEngine.Debug.Assert(asset != null, scriptablePath);
        return asset;
    }

    #endregion

    //
    #region Command
    public async Task ExecuteISCC(string command)
    {
        command = command.Replace('/', '\\');
        string exe = "\"C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe\"";
        //ExecuteCommandSync(exe + " " + command);
        await ExecuteCommand(exe, $"\"{command}\"", (s)=>info +=$"\n{s}", (s) => info += $"\nError : {s}");
    }

    public static async Task ExecuteCommand(string command, string args, Action<string> output, Action<string> error)
    {
        var processInfo = new ProcessStartInfo(command, args)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };

        var process = Process.Start(processInfo);

        if (output != null)
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output(e.Data);
        else
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            UnityEngine.Debug.Log("Information while executing command { <i>" + command + " " + args + "</i> } : <b>D>" + e.Data + "</b>");

        process.BeginOutputReadLine();

        if (error != null)
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => error(e.Data);
        else
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            UnityEngine.Debug.Log("Error while executing command { <i>" + command + " " + args + "</i> } : <b>E>" + e.Data + "</b>");

        process.BeginErrorReadLine();

        while (!process.HasExited)
        {
            await Task.Yield();
        }

        process.Close();
    }
    #endregion
}
#endif