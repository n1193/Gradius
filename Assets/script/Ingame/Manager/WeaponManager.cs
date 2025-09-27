using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
public enum WeaponType { Normal, Laser, Missile, Double,Enemy,Boss,volcano }

[Serializable]
public class WeaponEntry
{
    public WeaponType weaponType;
    public GameObject weaponPrefab;
}

public class WeaponManager : MonoBehaviour
{
    [SerializeField] List<WeaponEntry> weapons = new();
    [SerializeField] ShotManager shotManager;

    Dictionary<WeaponType, GameObject> _map=new();
    [Inject]
    public void Construct(ShotManager shotManager)
    {
        this.shotManager = shotManager;
    }

    void Awake()
    {
        _map = weapons?.Where(w => w.weaponPrefab)
                       .ToDictionary(w => w.weaponType, w => w.weaponPrefab)
               ?? new Dictionary<WeaponType, GameObject>();
    }

    public void Fire(WeaponType type, Vector3 pos, Vector3 dir,
                     string tag = Tags.PlayerBullet, BulletOwner owner = BulletOwner.Player,
                     float? speed = null)
    {
        if (!_map.TryGetValue(type, out var prefab) || !prefab)
        {
            Debug.LogError($"Weapon prefab not found: {type}");
            return;
        }
        shotManager.Spawn(prefab, pos, dir, tag, owner, speed);
    }

    void OnValidate() => BuildMap();

    void BuildMap()
    {
        _map.Clear();
        if (weapons == null) return;

        var seen = new HashSet<WeaponType>();
        for (int i = 0; i < weapons.Count; i++)
        {
            var w = weapons[i];
            if (w == null) continue;

            if (!seen.Add(w.weaponType))
                Debug.LogWarning($"[WeaponManager] Duplicate entry for {w.weaponType} (index {i}). Last one wins.");

            if (!w.weaponPrefab)
                Debug.LogWarning($"[WeaponManager] Missing prefab for {w.weaponType} (index {i}).");

            if (w.weaponPrefab) _map[w.weaponType] = w.weaponPrefab; // 重複は最後で上書き
        }
    }
}
