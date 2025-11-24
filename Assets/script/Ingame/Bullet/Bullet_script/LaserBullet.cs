using System;
using UnityEngine;

public class LaserBullet : Bullet
{
    protected override void Awake()
    {
        dir = Vector2.right;
        speed = 40f; // レーザー弾の速度
        base.Awake();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.Ground))
        {
            if (bulletPool != null & !isDead)
                gameObject.SetActive(false);
        }
    }

}