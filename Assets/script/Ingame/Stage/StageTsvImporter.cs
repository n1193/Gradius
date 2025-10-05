/*
// Editor/StageTsvImporter.cs
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class StageTsvImporter : EditorWindow
{
    TextAsset tsv;
    string savePath = "Assets/StageDataRuntime.asset";

    [MenuItem("Tools/Stage/Import TSV -> SO")]
    static void Open() => GetWindow<StageTsvImporter>("TSV -> SO");

    void OnGUI(){
        tsv = (TextAsset)EditorGUILayout.ObjectField("TSV", tsv, typeof(TextAsset), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        if (GUILayout.Button("Import")){
            if (!tsv){ Debug.LogError("TSV未指定"); return; }
            var so = ScriptableObject.CreateInstance<StageDataRuntime>();
            LoadFromTSV(tsv.text, so);
            AssetDatabase.CreateAsset(so, savePath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = so;
            EditorGUIUtility.PingObject(so);
            Debug.Log($"Imported TSV -> {savePath}");
        }
    }

    static void LoadFromTSV(string text, StageDataRuntime dst){
        dst.events.Clear(); dst.checkpoints.Clear();
        using var sr = new StringReader(text);
        string line; int lineNo = 0;
        while ((line = sr.ReadLine()) != null){
            lineNo++;
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var cols = line.Split('\t');
            string kind = cols[0].Trim().ToLowerInvariant();
            try{
                if (kind == "event"){
                    // kind xPos prefabId y count interval drop note
                    var e = new SpawnEvent{
                        xPos     = ParseF(cols,1,0f),
                        prefabId = Get(cols,2,""),
                        y        = ParseF(cols,3,0f),
                        count    = ParseI(cols,4,1),
                        interval = ParseF(cols,5,0f),
                        drop     = ParseI(cols,6,0) != 0,
                    };
                    dst.events.Add(e);
                }else if (kind == "cp"){
                    // kind x spawnIndex weaponKind powerLevels optionCount note
                    var cp = new Checkpoint{
                        x           = ParseF(cols,1,0f),
                        spawnIndex  = ParseI(cols,2,0),
                        weaponKind  = ParseI(cols,3,0),
                        powerLevels = ParseI(cols,4,0),
                        optionCount = ParseI(cols,5,0),
                    };
                    dst.checkpoints.Add(cp);
                }
            }catch(Exception ex){
                Debug.LogError($"TSV parse error L{lineNo}: {ex.Message}\n{line}");
            }
        }
        dst.events.Sort((a,b)=>a.xPos.CompareTo(b.xPos));
        dst.checkpoints.Sort((a,b)=>a.x.CompareTo(b.x));
    }
    static string Get(string[] a,int i,string d)=> (i<a.Length)?a[i].Trim():d;
    static int ParseI(string[] a,int i,int d){ int v; return int.TryParse(Get(a,i,""), out v)?v:d; }
    static float ParseF(string[] a,int i,float d){ float v; return float.TryParse(Get(a,i,""), out v)?v:d; }
}
*/