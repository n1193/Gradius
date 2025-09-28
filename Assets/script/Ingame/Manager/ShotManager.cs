/*using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ShotManager : MonoBehaviour
{
    DiContainer _container;

    [Inject]
    void Construct(DiContainer container)
    {
        _container = container;
    }

    public Bullet SpawnFromPool(BulletPool pool, Vector3 position, Vector3 direction, string tag, BulletOwner owner)
    {
        pool.Fire(position, direction, this, tag, owner);
        return null; // 必要なら Poolから発射したBulletを返す設計に変更してもOK
    }

    public Bullet SpawnDirect(GameObject prefab, Vector3 position, Vector3 direction, string tag, BulletOwner owner)
    {
        var b = _container.InstantiatePrefabForComponent<Bullet>(prefab, position, Quaternion.identity, this.transform);
        b.Initialize(owner, null, this, direction, tag);
        return b;
    }
}
*/