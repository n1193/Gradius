using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class DropManager : MonoBehaviour
{
    [Header("落とす物（共通）")]
    [SerializeField] GameObject itemDropPrefab;
    //[SerializeField] int dropGroupTotal = 3; // 落とすグループの総数
    [SerializeField] Transform parent; // 落とすグループの総数

    private List<DropGroup> groups = new();

    [Inject] DiContainer _container;



    /*public DropGroup CreateGroup(Transform anchor, int totalCount)
        => new DropGroup(this, anchor, totalCount);
*/
    public void SpawnDrop(Vector3 pos)
    {
        if (!itemDropPrefab) return;
        _container.InstantiatePrefab(itemDropPrefab, pos, Quaternion.identity, parent);
    }
}
