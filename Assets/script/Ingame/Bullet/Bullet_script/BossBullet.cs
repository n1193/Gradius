using UnityEngine;
public class BossBullet : Bullet
{
    private void Start()
    {
        dir = Vector2.left;
        speed = 20f; // ダブル弾の速度
    }
}
