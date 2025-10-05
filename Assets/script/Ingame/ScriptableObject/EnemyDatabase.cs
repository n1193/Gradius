using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Fan, Rugal, Garun, Jumper, Dee_01, Ducker, Hatches, Rush, BigCore }

[System.Serializable]
public class EnemyEntry
{
    public EnemyType enemyType;
    public string prefabPath; // 例: "Enemies/EnemyBasic"
}

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "GameData/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] private List<EnemyEntry> enemyEntries;

    private Dictionary<EnemyType, Enemy> _cache = new();

    public Enemy GetPrefab(EnemyType type)
    {
        // キャッシュ済みなら即返す
        if (_cache.TryGetValue(type, out var cached))
            return cached;

        var entry = enemyEntries.Find(e => e.enemyType == type);
        if (entry == null)
        {
            Debug.LogWarning($"[EnemyDatabase] No entry for {type}");
            return null;
        }

        var prefab = Resources.Load<Enemy>(entry.prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[EnemyDatabase] Failed to load prefab at path: {entry.prefabPath}");
            return null;
        }

        _cache[type] = prefab;
        return prefab;
    }
}
