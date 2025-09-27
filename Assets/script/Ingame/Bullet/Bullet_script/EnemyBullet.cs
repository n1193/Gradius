using UnityEngine;
using Zenject;
using System;

public class EnemyBullet : Bullet
{
    [Inject]
    void Construct(PlayerController playerController)
    {
        dir = playerController.transform.position - transform.position;
        dir.Normalize();

    }
}