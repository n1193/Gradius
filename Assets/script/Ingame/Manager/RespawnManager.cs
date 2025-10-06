using Unity.VisualScripting;
using UnityEngine;
using Zenject;


public class RespawnManager : MonoBehaviour
{
     private PlayerController player;
     private ScrollDirector scroll;
     private EnemyManager enemyManager;
     private EnemySpawner enemySpawner;
    private Upgrade upgrade;
    DropManager dropManager;
    [Inject]
    public void Construct(PlayerController player, ScrollDirector scroll, EnemyManager enemyManager, EnemySpawner enemySpawner, Upgrade upgrade, DropManager dropManager)
    {
        this.player = player;
        this.scroll = scroll;
        this.enemyManager = enemyManager;
        this.enemySpawner = enemySpawner;
        this.upgrade = upgrade;
        this.dropManager = dropManager;
 
    }
    // GameManagerから呼ばれる
    public void OnPlayerDeath()
    {
        //再表示
        player.gameObject.SetActive(true);
        player.Initialize();
        float X = scroll.SetScrollX();
        enemyManager.ClearAll();
        enemySpawner.ResetSpawner(X);
        upgrade.Reset();
        //player.ResetAfterRespawn();
        dropManager.Reset();
    }
}
