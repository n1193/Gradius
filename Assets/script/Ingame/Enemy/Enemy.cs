using UnityEngine;
using Zenject;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private bool dropOnDeath = true;
    [SerializeField] private int groupID = -1; // -1なら単独
    public bool DropFlag { get; private set; }
    public static event Action<int> OnEnemyDeathInGroup;
    public PlayerController playerController;
    public WeaponManager weaponManager;
    bool Active = false;
    public ScoreManager scoreManager;
    public int score = 100;
    protected DropGroup _dropGroup;
    DropGroupManager dropGroupManager;
    public void SetDropGroup(DropGroup g) => _dropGroup = g;
    private bool isDead;

    private SoundManager soundManager;
    [SerializeField] protected GameObject explosionEffectPrefab;

    public void Initialize(EnemySpawnData data)
    {
        DropFlag = data.dropFlag;
    }
    public void AssignDropGroup(DropGroup group)
    {
        _dropGroup = group;
    }
    private void OnDestroy()
    {
        _dropGroup?.Remove(this);
    }

    int hp;
    [Inject]
    void Construct(PlayerController playerController, WeaponManager weaponManager, ScoreManager scoreManager, SoundManager soundManager, DropGroupManager dropGroupManager)
    {
        this.weaponManager = weaponManager;
        this.playerController = playerController;
        this.scoreManager = scoreManager;
        this.soundManager = soundManager;
        this.dropGroupManager = dropGroupManager;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            hp--;
            if (hp <= 0)
            {
                Die(true);
                soundManager.SEPlay(SEType.EnemyDestroy, 0.3f);
            }
        }
    }
    private void Die(bool byPlayer)
    {
        //ScoreManager.Instance?.AddScore(score);
        if (byPlayer)
        {
            scoreManager?.AddScore(score);
            //_dropGroup?.NotifyMemberDead(byPlayer, transform.position);
            Debug.Log($"[Enemy] Die by Player score={score}");
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation, this.gameObject.transform.parent);
            // DropGroupManagerに通知
            if (_dropGroup != null)
            {
                dropGroupManager.NotifyDeath(this);
            }
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