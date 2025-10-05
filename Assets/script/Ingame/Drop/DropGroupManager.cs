using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class DropGroupManager : MonoBehaviour
{
    private DropManager dropManager;

    private List<DropGroup> groups = new();
    [Inject]
    public void Construct(DropManager dropManager)
    {
        this.dropManager = dropManager;
    }
    public DropGroup CreateGroup(int count)
    {
        DropGroup group = new DropGroup();
        groups.Add(group);
        return group;
    }
    public void NotifyDeath(Enemy enemy)
    {
        Debug.Log($"[DropGroupManager] NotifyDeath() 呼び出し確認 {enemy.name}");
        foreach (var group in groups)
        {
            if (group.Contains(enemy))
            {
                group.Remove(enemy);
                if (group.IsAllDead())
                {
                    dropManager.SpawnDrop(enemy.transform.position);
                    groups.Remove(group);
                }
                return;
            }
        }
    }
}
