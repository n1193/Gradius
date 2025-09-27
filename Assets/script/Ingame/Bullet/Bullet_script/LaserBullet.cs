using System;
using UnityEngine;

public class LaserBullet : Bullet
{
    private void Start()
    {
        speed = 50f; // レーザー弾の速度
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Ground))
        {
            Destroy(gameObject); // 弾を削除
        }
    }
}