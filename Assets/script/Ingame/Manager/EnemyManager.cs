using UnityEngine;
using System.Collections.Generic;
using Zenject;
using System;

public enum EnemyType { Fan, Rugal, Garun, Jumper, Dee_01, Ducker, Hatches, Rush, BigCore }

[Serializable]
public class EnemyEntry
{
    public EnemyType enemyType;
    public Enemy enemyPrefab;   // ルートに Enemy を1個
}

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemyEntry> enemyEntry;
    [SerializeField] SpawnManager spawnManager;

    [Inject]
    public void Construct(SpawnManager spawnManager)
    {
        this.spawnManager = spawnManager;
    }

    // Wave生成（Spawnerから呼ばれる）
    public void Instance(
        EnemyType enemyType,
        Vector3 spawnPoint,
        bool drop = false,
        int count = 1,
        float spawnInterval = 0.25f,
        SpawnPosition spawenPosition = SpawnPosition.CustomPoint)
    {
        var entry = enemyEntry.Find(e => e.enemyType == enemyType);
        if (entry == null || entry.enemyPrefab == null)
            return; // spawnPoint(Vector3) の null チェックは不要

        // ★ 変更点：最後の引数に drop を渡す
        spawnManager.Spawn(entry.enemyPrefab, spawnPoint, count, spawnInterval, spawenPosition, drop);
    }
}
