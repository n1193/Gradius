using System.Collections.Generic;
using UnityEngine;

public class DropGroup
{
    private List<Enemy> members = new();
    private int expectedCount;
    private bool isSpawnComplete = false;

    public void AddEnemy(Enemy e)
    {
        members.Add(e);
        e.AssignDropGroup(this);
        expectedCount = members.Count; // ←追加時に更新
        Debug.Log($"[DropGroup] 追加: {e.name}, 現在{expectedCount}体");
    }
    public void MarkSpawnComplete()
    {
        isSpawnComplete = true;
        Debug.Log("[DropGroup] 生成完了フラグを立てた");
    }

    public bool Contains(Enemy e) => members.Contains(e);

    public void Remove(Enemy e)
    {
        members.Remove(e);
        Debug.Log($"[DropGroup] {e.name}死亡 残り={members.Count}/{expectedCount}");
    }

    public bool IsAllDead()
    {
        if (!isSpawnComplete) return false;  // ← 生成が終わるまで判定しない
        bool allDead = members.TrueForAll(m => m == null);
        return allDead;
    }
}
