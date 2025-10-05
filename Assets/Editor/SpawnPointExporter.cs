using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class SpawnPointExporter
{
    [MenuItem("Tools/Export SpawnPoints to TSV")]
    public static void Export()
    {
        // Scene上のSpawnPointを全取得
        var spawnPoints = Object.FindObjectsOfType<SpawnPoint>();

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("[SpawnPointExporter] SpawnPointが見つかりませんでした。");
            return;
        }

        // TSVデータ構築
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("enemyType\tx\ty\tcount\tinterval\tdrop\tspawnPos\tdelay");

        foreach (var sp in spawnPoints)
        {
            string enemyType = sp.enemyType.ToString();
            float x = sp.transform.position.x;
            float y = sp.transform.position.y;
            int count = sp.spawnCount;
            float interval = sp.spawnInterval;
            bool drop = sp.drop;
            string spawnPos = "Right"; // 必要ならsp.SpawnPositionから取得
            float delay = 0f;

            sb.AppendLine($"{enemyType}\t{x:F2}\t{y:F2}\t{count}\t{interval}\t{drop}\t{spawnPos}\t{delay}");
        }

        // 保存場所
        string path = "Assets/Resources/EnemySpawnData_2.tsv";
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"[SpawnPointExporter] {spawnPoints.Length}件のSpawnPointを書き出しました → {path}");
    }
}
