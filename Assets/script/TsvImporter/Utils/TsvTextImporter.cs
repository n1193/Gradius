// Assets/Editor/TsvTextImporter.cs
using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "tsv")]
public class TsvTextImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // 必要ならEncoding変更
        var text = File.ReadAllText(ctx.assetPath, Encoding.UTF8);

        var ta = new TextAsset(text);
        ta.name = Path.GetFileNameWithoutExtension(ctx.assetPath);

        ctx.AddObjectToAsset("Text", ta);
        ctx.SetMainObject(ta);
    }
}
