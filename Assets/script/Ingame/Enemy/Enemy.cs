using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class Enemy : MonoBehaviour
{
    public PlayerController playerController;
    public WeaponManager weaponManager;
    bool Active = false;
    public ScoreManager scoreManager;
    public int score = 100;
    DropGroup _dropGroup;
    public void SetDropGroup(DropGroup g) => _dropGroup = g;
    private SoundManager soundManager;
    [SerializeField]private  GameObject explosionEffectPrefab;

    [Inject]
    void Construct(PlayerController playerController, WeaponManager weaponManager, ScoreManager scoreManager, SoundManager soundManager)
    {
        this.weaponManager = weaponManager;
        this.playerController = playerController;
        this.scoreManager = scoreManager;
        this.soundManager = soundManager;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Die(true);
            soundManager.SEPlay(SEType.EnemyDestroy, 0.3f);
        }
    }

    // ▼ 変更：死因の引数を追加（最小差分）
    private void Die(bool byPlayer)
    {
        //ScoreManager.Instance?.AddScore(score);
        if (byPlayer)
        {
            scoreManager?.AddScore(score);
            _dropGroup?.NotifyMemberDead(byPlayer, transform.position);
            Debug.Log($"[Enemy] Die by Player score={score}");
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation, this.gameObject.transform.parent); 
        }
        // ▼ 追加：Drop 通知（AllKilled用）
        gameObject.SetActive(false); // プール返却でもOK
        Destroy(gameObject);         // プールを使わないなら破棄
    }

    void OnBecameVisible()
    {
        Active = true;
    }

    void OnBecameInvisible()
    {
        if (Active)
        {
            // ▼ 変更：逃亡扱いで通知してから破棄
            Die(false);
        }
    }
}