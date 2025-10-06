using UnityEngine;

public class DoubleBullet : Bullet
{
    protected override void Awake()
    {
        dir = new Vector2(1, 1);
        speed = 25f; // ダブル弾の速度
        base.Awake();
    }
}