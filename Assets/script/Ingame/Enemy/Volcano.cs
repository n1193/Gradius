using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class Volcano : Enemy
{
    Action Shot; // 通常弾やレーザー弾の発射アクションのイベント
    const float shotIntervalTime = 0.3f; // 弾を発射する間隔
    float shotInterval;
    ScrollDirector scroll;
    [SerializeField] private GameObject[] ShotPosition;
    [SerializeField] string signalId = "volcano";
    float lifetime = 16.0f; // 存在時間    
    float WaitTime = 7f; // 待機時間
    [Inject]
    void Construct(ScrollDirector scroll)
    {
        this.scroll = scroll;
    }

    void Start()
    {
        shotInterval = shotIntervalTime;
        foreach (var pos in ShotPosition)
        {
            pos.transform.parent = transform; // 親子関係を解除してワールド座標にする
            Shot += () =>
            {
                weaponManager.Fire(
                WeaponType.volcano,
                pos.transform.position,
                Vector3.left,
                Tags.EnemyBullet,
                BulletOwner.Enemy);
            };
        }
        Shot += () => shotInterval = shotIntervalTime; // インターバルをリセット
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
            if (Shot != null)
                Shot?.Invoke(); // 弾を発射
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
