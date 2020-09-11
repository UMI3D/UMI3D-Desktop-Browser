using System;
using System.Collections.Generic;
using System.Linq;
using umi3d.cdk;
using UnityEngine;

public class DisplayLibraries : MonoBehaviour
{
    public Transform displayerPrefab;
    List<ApplicationLibraryDisplayer> displayers = new List<ApplicationLibraryDisplayer>();

    private void Start()
    {
        UpdateLibs();
    }

    void UpdateLibs()
    {
        foreach (var display in displayers)
            GameObject.Destroy(display.gameObject);
        displayers.Clear();

        Dictionary<string, List<UMI3DResourcesManager.DataFile>> libs = new Dictionary<string, List<UMI3DResourcesManager.DataFile>>();
        foreach (var lib in UMI3DResourcesManager.Libraries)
        {
            if(lib.applications != null)
                foreach(var app in lib.applications)
                {
                    if (!libs.ContainsKey(app)) libs[app] = new List<UMI3DResourcesManager.DataFile>();
                    libs[app].Add(lib);
                }
        }
        foreach (var app in libs)
        {
            var t = Instantiate(displayerPrefab, transform);
            var d = t.GetComponent<ApplicationLibraryDisplayer>();
            Action deleteLib = () =>
            {
                foreach (var lib in app.Value)
                {
                    lib.applications.Remove(app.Key);
                    if (lib.applications.Count <= 0)
                        UMI3DResourcesManager.RemoveLibrary(lib.key);
                }
                UpdateLibs();
            };
            d.set(app.Key, app.Value.Select(l => l.key).ToList(), deleteLib);
            displayers.Add(d);
        }

        //foreach (var lib in UMI3DResourcesManager.Libraries)
        //{
        //    var t = Instantiate(displayerPrefab, transform);
        //    var d = t.GetComponent<LibraryDisplayer>();
        //    Action deleteLib = () =>
        //    {
        //        UMI3DResourcesManager.RemoveLibrary(lib.key);
        //        UpdateLibs();
        //    };
        //    d.set(lib.key, lib.date, deleteLib);
        //    displayers.Add(d);
        //}
    }


}
