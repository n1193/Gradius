using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Search;

public class DropManager : MonoBehaviour
{
    [Header("落とす物（共通）")]
    [SerializeField] GameObject itemDropPrefab;
    //[SerializeField] int dropGroupTotal = 3; // 落とすグループの総数
    [SerializeField] Transform parent; // 落とすグループの総数

    private List<DropGroup> groups = new();

    [Inject] DiContainer _container;

    List<GameObject> Items = new();
    /*
    public DropGroup CreateGroup(Transform anchor, int totalCount)
        => new DropGroup(this, anchor, totalCount);
    */
    public void SpawnDrop(Vector3 pos)
    {
        if (!itemDropPrefab) return;
        GameObject item = _container.InstantiatePrefab(itemDropPrefab, pos, Quaternion.identity, parent);
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
