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

using inetum.unityUtils;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using umi3d;
using UnityEngine;
using UnityEngine.UIElements;

public class BugReporter : SingleBehaviour<BugReporter>
{
    #region Fields

    public readonly List<string> unityLogFiles = new List<string> { "Player.log", "Player-prev.log" };

    [SerializeField]
    VisualTreeAsset popUpTemplate;

    VisualElement popUp;

    TextField nameInput;
    TextField titleInput;
    TextField reproStepsInput;

    #endregion

    #region Methods

    private void Start()
    {
        popUp = popUpTemplate.CloneTree().Children().First();

        nameInput = popUp.Q<TextField>("name-input");
        titleInput = popUp.Q<TextField>("title-input");
        reproStepsInput = popUp.Q<TextField>("repro-steps-input");

        popUp.Q<Button>("close-bug-report-pop-up").clickable.clicked += () => ClosePopUp(false);
        popUp.Q<Button>("generate-report-btn").clickable.clicked += () => ClosePopUp(true);

        var popUpElement = popUp.Q<VisualElement>("bug-report-pop-up");

        ConnectionMenu.Instance.uiDocument.rootVisualElement.RegisterCallback<GeometryChangedEvent>(e =>
        {
            if (ConnectionMenu.Instance.uiDocument.rootVisualElement.resolvedStyle.height < 700)
            {
                popUpElement.style.height = new StyleLength(new Length (90, LengthUnit.Percent));
            }
            else
            {
                popUpElement.style.height = 680;
            }
        });
    }


    public void DisplayPopUp()
    {
        ConnectionMenu.Instance.uiDocument.rootVisualElement.Add(popUp);
        popUp.BringToFront();

        umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free);
    }

    public void ClosePopUp(bool generateReport)
    {
        popUp.RemoveFromHierarchy();

        if (generateReport)
            GenerateBugReport(nameInput.value, titleInput.value, reproStepsInput.value);

        nameInput.SetValueWithoutNotify(string.Empty);
        reproStepsInput.SetValueWithoutNotify(string.Empty);
        titleInput.SetValueWithoutNotify(string.Empty);

        umi3d.baseBrowser.Controller.BaseCursor.SetMovement(this, umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Center);
    }

    private void GenerateBugReport(string name, string title, string reproSteps)
    {
        DateTime now = DateTime.Now;

        string tmpFolder = System.IO.Path.Combine(Application.persistentDataPath, "tmp-bug-reporter");
        string reportPath = System.IO.Path.Combine(tmpFolder, "bug-report.log");

        try
        {
            //1. Create temporary folder
            if (!Directory.Exists(tmpFolder))
                Directory.CreateDirectory(tmpFolder);

            //2. Take a screenshot
            string screenshotPath = System.IO.Path.Combine(tmpFolder, "screenshot.png");
            ScreenCapture.CaptureScreenshot(screenshotPath);

            //3. Write log file
            if (!File.Exists(reportPath))
            {
                using (StreamWriter sw = File.CreateText(reportPath))
                {
                    sw.WriteLine("--------- BUG REPORT ---------");

                    sw.WriteLine();
                    sw.WriteLine("# Date " + now.ToString());
                    sw.WriteLine();

                    sw.WriteLine("# Versions");
                    sw.WriteLine("UMI3DVersion : " + UMI3DVersion.version);
                    sw.WriteLine("Software version : " + BrowserDesktop.BrowserVersion.Version);

                    sw.WriteLine();
                    sw.WriteLine("# BUG Description");
                    sw.WriteLine();

                    sw.WriteLine("Title : " + title);

                    sw.WriteLine();
                    sw.WriteLine("Repro steps : ");
                    sw.WriteLine(reproSteps);
                    sw.WriteLine();

                    sw.WriteLine("Author : " + name);

                    sw.WriteLine();
                    sw.WriteLine("# Hardware Description");
                    sw.WriteLine(GetSystemDetails());
                    sw.WriteLine();
                }
            }

            //4.Copy log files
            foreach (var logFile in unityLogFiles)
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath, logFile);
                if (File.Exists(path))
                {
                    File.Copy(path, System.IO.Path.Combine(tmpFolder, logFile));
                }
            }

            StartCoroutine(CreateZipFile(screenshotPath, tmpFolder, now));
        } catch (Exception e)
        {
            Debug.LogError("Impossible to create bug report file : " + e.Message);
            DeleteDirectory(tmpFolder);
        }
    }

    public String GetSystemDetails()
    {
        string res = "OS : " + SystemInfo.operatingSystem + "\n";
        res +=  "CPU : " + SystemInfo.processorType + "\n";
        res += "GPU : " + SystemInfo.graphicsDeviceName + "\n";
        res += "Memory : " + SystemInfo.systemMemorySize + "mo \n";

        return res;
    }

    IEnumerator CreateZipFile(string screenShotFile, string tmpFolder, DateTime now)
    {
        while (!File.Exists(screenShotFile))
            yield return null;

        try
        {
            string defaultName = "UMI3D-Desktop-Browser-BUG-REPORT-" + now.Month + "-" + now.Day + "-" + now.Year + "-" + now.Hour + "-" + now.Minute + ".zip";

            string zipPath = StandaloneFileBrowser.SaveFilePanel("Save File", "", defaultName, "zip");

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            //5.Create zip
            ZipFile.CreateFromDirectory(tmpFolder, zipPath);

            DeleteDirectory(tmpFolder);

            //Open directory
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo(System.IO.Path.GetDirectoryName(zipPath));
            p.Start();

        } catch (Exception e)
        {
            Debug.LogError("Impossible to create bug report file : " + e.Message);
            DeleteDirectory(tmpFolder);
        }
    }

    public void DeleteDirectory(string dirToDelete)
    {
        string[] files = Directory.GetFiles(dirToDelete);
        string[] dirs = Directory.GetDirectories(dirToDelete);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(dirToDelete, false);
    }

    #endregion
}
