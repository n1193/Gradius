using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
    Default,   // 何も指定しないとき
    Power,
    AllKill,
    // 必要に応じて追加
}
public class ItemFactory : MonoBehaviour
{
    [SerializeField] List<Entry> entries;

    Dictionary<ItemType, GameObject> _map;

    void Awake()
    {
        _map = entries.ToDictionary(e => e.type, e => e.prefab);
    }

    public GameObject GetPrefab(ItemType type)
    {
        return _map.TryGetValue(type, out var prefab) ? prefab : null;
    }

    [System.Serializable]
    public struct Entry
    {
        public ItemType type;
        public GameObject prefab;
    }
}
