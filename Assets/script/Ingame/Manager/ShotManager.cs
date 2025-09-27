using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class ShotManager : MonoBehaviour
{
    readonly List<Bullet> bullets = new();
    DiContainer _container;

    [Inject]
    void Construct(DiContainer container)
    {
        _container = container;
    }

    public Bullet Spawn(GameObject prefab, Vector3 position, Vector3 direction, string tag, BulletOwner owner, float? overrideSpeed = null)
    {
        // 戻り値は Bullet コンポーネント
        var b = _container.InstantiatePrefabForComponent<Bullet>(prefab, position, Quaternion.identity, this.transform);
        b.Initialize(owner, this, direction, tag);
        RegisterBullet(b);
        return b;
    }

    public void RegisterBullet(Bullet b)
    {
        if (b && !bullets.Contains(b)) bullets.Add(b);
    }

    public void RemoveBullet(Bullet b)
    {
        if (b) bullets.Remove(b);
    }
}
