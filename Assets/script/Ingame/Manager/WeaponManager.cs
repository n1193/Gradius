using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Zenject;

public enum WeaponType { Normal, Laser, Missile, Double, Enemy, Boss, Volcano }

[System.Serializable]
public class WeaponEntry
{
    public WeaponType weaponType;
    public GameObject bulletPrefab;
}

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<WeaponEntry> weapons = new();
    [SerializeField] private BulletPool bulletPool;
    private Dictionary<WeaponType, GameObject> _weaponMap = new();
    private DiContainer diContainer;

    [Inject]
    void Construct(DiContainer diContainer)
    {
        this.diContainer = diContainer;
    }

    void Awake()
    {
        foreach (var weapon in weapons)
        {
            if (weapon != null && weapon.bulletPrefab != null)
            {
                _weaponMap[weapon.weaponType] = weapon.bulletPrefab;
            }
        }
    }

    public Bullet GetBulletPrefab(WeaponType type)
    {
        if (_weaponMap.TryGetValue(type, out GameObject prefab))
        {
            Debug.Log($"[WeaponManager] weaponType: {type}, hasPrefab: {_weaponMap.ContainsKey(type)}");
            return prefab.GetComponent<Bullet>();
        }
        return null;
    }
    public BulletPool CreateBulletPool(Transform transform)
    {
        return diContainer.InstantiatePrefabForComponent<BulletPool>(bulletPool);
    }
}
