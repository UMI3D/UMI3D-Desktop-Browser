using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class LocalizationCSVWindow : EditorWindow
{
    const string k_path = "./Localization/";
    LocalisationTable table;
    LocalisationTable tableLoad;
    string pathLoad;

    [MenuItem("UMI3D/Localization/CSV")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LocalizationCSVWindow));
    }

    private void OnGUI()
    {
        var title = new GUIStyle();
        title.fontStyle = FontStyle.Bold;
        title.fontSize = 15;
        title.normal.textColor = Color.white;
        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Save", title);
        table = EditorGUILayout.ObjectField("Table", table, typeof(LocalisationTable), true) as LocalisationTable;
        if (GUILayout.Button("Save to CSV"))
        {
            if (table == null) return;

            string t = "Key,English,French,Spanish";
            foreach (var item in table.Items)
            {
                t += $"\n{item.Key},{item.English},{item.French},{item.Spanish}";
            }

            using (StreamWriter sw = File.CreateText(k_path + table.name + ".csv"))
            {
                sw.Write(t);
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(15);

        GUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Load", title);
        tableLoad = EditorGUILayout.ObjectField("Table", tableLoad, typeof(LocalisationTable), true) as LocalisationTable;
        pathLoad = EditorGUILayout.TextField("Path", pathLoad) as string;
        if (GUILayout.Button("Load from CSV"))
        {
            if (table == null) return;
            if (!File.Exists(pathLoad)) return;

            EditorUtility.SetDirty(table);
            table.Items = new List<LocalisationTableItem>();
            var lines = File.ReadLines(pathLoad).ToList();
            for (int i = 1; i < lines.Count; i++)
            {
                var element = lines[i].Split(",");
                table.Items.Add(new LocalisationTableItem()
                {
                    Key = element[0],
                    English = element[1],
                    French = element[2],
                    Spanish = element[3]
                });
            }
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndVertical();
    }
}
