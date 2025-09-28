using UnityEngine;
using Zenject;
using System;

public class EnemyBullet : Bullet
{
    PlayerController playerController;

    [Inject]
    void Construct(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    public override void refrash()
    {
        dir = playerController.transform.position - transform.position;
        dir.Normalize();
        isDead = false;
        gameObject.SetActive(true);
    }
}