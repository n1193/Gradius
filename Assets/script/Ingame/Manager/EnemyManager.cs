using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyDatabase database;  // ScriptableObject参照
    private readonly List<Enemy> enemies = new();
    DiContainer _container;
    /// <summary>
    /// 指定タイプの敵を生成して登録
    /// </summary>
    public Enemy SpawnEnemy(EnemyType type, Vector3 position, DropGroup dropGroup=null,Transform parent = null)
    {
        Enemy prefab = database.GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogWarning($"[EnemyManager] Prefab not found for type: {type}");
            return null;
        }

        Enemy instance = _container.InstantiatePrefabForComponent<Enemy>(prefab, position, Quaternion.identity, parent);
        if (dropGroup != null)
        {
            instance.AssignDropGroup(dropGroup);
            dropGroup.AddEnemy(instance);
        }
        RegisterEnemy(instance);
        //Debug.Log($"EnemyManager.Spawn: type={type}, position={instance.transform.position}");
        return instance;
    }
    public void ClearAll()
    {
        foreach (var e in enemies)
        {
            if (e != null)
            {
                e.Die(true);
            }
        }
        enemies.Clear();
        Debug.Log("[EnemyManager] 全敵削除");
    }
    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }
    /// <summary>
    /// 生成された敵を登録
    /// </summary>
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    /// <summary>
    /// 個別削除時などに呼ぶ
    /// </summary>
    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// 全削除（リスポーン前など）
    /// </summary>
    public void ClearAllEnemies(bool score = false)
    {
        Debug.Log($"[EnemyManager] Clearing {enemies.Count} enemies.");
        foreach (Enemy e in enemies)
        {
            if (e != null)
             continue;    
            Destroy(e);
        }
        enemies.Clear();
    }
}
