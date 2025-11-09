using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Search;

public class DropManager : MonoBehaviour
{
    [Header("落とす物（共通）")]
    [SerializeField] GameObject itemDropPrefab;
    [SerializeField] GameObject enemyClerarEffectPrefab;
    [SerializeField] Transform parent; // 落とすグループの総数
    int dropcount = 0;
    private List<DropGroup> groups = new();
    private const int enemyClearEffectCount = 13;
    [Inject] DiContainer _container;

    List<GameObject> Items = new();
    /*
    public DropGroup CreateGroup(Transform anchor, int totalCount)
        => new DropGroup(this, anchor, totalCount);
    */
    public void SpawnDrop(Vector3 pos)
    {
        if (!itemDropPrefab) return;
        dropcount++;
        dropcount %= enemyClearEffectCount;
        GameObject item;
        Debug.Log($"[DropManager] SpawnDrop() 呼び出し確認 dropcount={dropcount}");
        if (dropcount == 0)
        {
            item = _container.InstantiatePrefab(enemyClerarEffectPrefab, pos, Quaternion.identity, parent);
            Debug.Log("[DropManager] 敵全消しアイテム生成");
        }
        else
        {
            item = _container.InstantiatePrefab(itemDropPrefab, pos, Quaternion.identity, parent);
            Debug.Log("[DropManager] アイテムドロップ生成");
        }
        Items.Add(item);
    }
    public void Reset()
    {
        foreach (var g in Items)
        {
            if (g != null)
                Destroy(g);
        }
        Items.Clear();
    }
}
