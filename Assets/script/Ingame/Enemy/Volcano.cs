using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class Volcano : Enemy
{
    const float shotIntervalTime = 0.3f; // 弾を発射する間隔
    float shotInterval;
    ScrollDirector scroll;
    [SerializeField] private GameObject[] ShotPosition;
    [SerializeField] string signalId = "volcano";
    float lifetime = 16.0f; // 存在時間    
    float WaitTime = 7f; // 待機時間
    BulletPool bulletPool1;
    BulletPool bulletPool2;

    [Inject]
    void Construct(ScrollDirector scroll)
    {
        this.scroll = scroll;
    }

    void Start()
    {
        shotInterval = shotIntervalTime;
        bulletPool1 = weaponManager.CreateBulletPool(transform);
        bulletPool1.Initialize(BulletOwner.Enemy,WeaponType.Volcano, -1,0.25f,SEType.None,Tags.EnemyBullet.ToString());
        bulletPool2 = weaponManager.CreateBulletPool(transform);
        bulletPool2.Initialize(BulletOwner.Enemy,WeaponType.Volcano, -1,0.25f,SEType.None,Tags.EnemyBullet.ToString());
    }

    void Update()
    {
        if (WaitTime > 0)
        {
            WaitTime -= Time.deltaTime;
            return;
        }
        Attack();
        isDead();
    }
    void Attack()
    {
        shotInterval -= Time.deltaTime;
        if (shotInterval <= 0)
        {
            bulletPool1.Fire(ShotPosition[0].transform.position,BulletOwner.Enemy); // 弾を発射
            bulletPool2.Fire(ShotPosition[1].transform.position,BulletOwner.Enemy); // 弾を発射
        }
    }
    void isDead()
    {
        lifetime -= Time.deltaTime;
        if (lifetime > 0) return;
        scroll.NotifySignal(signalId);
        Destroy(gameObject);
    }
    
}
