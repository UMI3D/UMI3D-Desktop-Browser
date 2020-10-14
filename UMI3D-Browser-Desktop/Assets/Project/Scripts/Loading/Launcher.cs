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

using BrowserDesktop.Cursor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public const string dataFile = "userInfo";

    public TMP_Text version;
    public TMP_InputField ip;
    public TMP_InputField port;
    public TMP_Dropdown dropdown;
    public Button deleteOption;
    public Button RunButton;
    public string scene;
    public string thisScene;

    [Serializable]
    public class Data
    {
        public string ip;
        public string port;
    }
    [Serializable]
    class DataSet
    {
        public List<Data> datas;
    }

    DataSet data;
    Data current;

    /// <summary>
    /// Write a DataFile in a directory.
    /// </summary>
    /// <param name="data">DataFile to write.</param>
    /// <param name="directory">Directory to write the file into.</param>
    void SetDataSet(DataSet data)
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        FileStream file;
        if (File.Exists(path)) file = File.OpenWrite(path);
        else file = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Read a DataFile in a directory.
    /// </summary>
    /// <returns>A DataFile if the directory containe one, null otherwhise.</returns>
    DataSet GetDataSet()
    {
        string path = umi3d.common.Path.Combine(Application.persistentDataPath, dataFile);
        if (File.Exists(path))
        {
            FileStream file;
            file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            DataSet data = (DataSet)bf.Deserialize(file);
            file.Close();
            return data;
        }
        return new DataSet() { datas = new List<Data>() };
    }

    private void Awake()
    {
        version.text = umi3d.UMI3DVersion.version;
        data = GetDataSet();
        RunButton.onClick.AddListener(Run);
        dropdown.options = data.datas.Select(d => new TMP_Dropdown.OptionData(d.ip)).ToList();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        deleteOption.onClick.AddListener(DeleteOption);
        dropdown.value = data.datas.Count - 1;
        OnDropdownValueChanged(dropdown.value);
    }

    void SetData(Data data)
    {
        current = data;
        ip.text = data?.ip;
        port.text = data?.port;
        Canvas.ForceUpdateCanvases();
    }

    void OnDropdownValueChanged(int i)
    {
        SetData((i < data.datas.Count && i >= 0) ? data.datas[i] : null);
    }

    void DeleteOption()
    {
        int i = dropdown.value;
        if (i < 0 || i >= data.datas.Count)
        {
            SetData(null);
            return;
        }

        data.datas.RemoveAt(i);
        SetDataSet(data);
        dropdown.options = data.datas.Select(d => new TMP_Dropdown.OptionData(d.ip)).ToList();
        i = Math.Min(i, data.datas.Count - 1);
        dropdown.value = i;
        OnDropdownValueChanged(i);
    }

    void Run()
    {
        if (current != null && current.ip == ip.text)
            data.datas.Remove(current);
        current = new Data() { ip = ip.text, port = port.text };
        data.datas.Add(current);
        SetDataSet(data);
        StartCoroutine(WaitReady(current));
        
    }
    IEnumerator WaitReady(Launcher.Data data)
    {
        CursorHandler.Instance.Clear();
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        while (!Connecting.Exists)
            yield return new WaitForEndOfFrame();
        Connecting.Instance.Connect(data);
        SceneManager.UnloadSceneAsync(thisScene);
        //initSocket();
    }

}
