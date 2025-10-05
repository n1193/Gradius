using UnityEngine;
using Zenject;


public class RespawnManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ScrollDirector scroll;


    [Inject]
    public void Construct(PlayerController player, ScrollDirector scroll)
    {
        this.player = player;
        this.scroll = scroll;
    }
    // GameManagerから呼ばれる
    public void OnPlayerDeath()
    {
        //再表示
        player.gameObject.SetActive(true);
        player.Initialize();
        scroll.SetScrollX();
        //player.ResetAfterRespawn();
    }
}
