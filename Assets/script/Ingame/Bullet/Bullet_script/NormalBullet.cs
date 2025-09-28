using UnityEngine;

public class NormalBullet : Bullet
{
    // Start で speed を上書きしない！Initialize の override を殺すから
    protected override void Awake()
    {
        dir = Vector2.right;
        base.Awake();
        speed = 20f;
    }
}
