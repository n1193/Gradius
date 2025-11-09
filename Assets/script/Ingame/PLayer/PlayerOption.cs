using UnityEngine;
using System.Collections.Generic;
using System;

public enum OptionIndex
{
    FIRST,
    SECOND
}

public class PlayerOption : MonoBehaviour
{
    [SerializeField] Transform player;
    int delayFramesPerOption = 16;//16フレーム分で考える
    private PlayerTrail playerTrail;
    private int optionIndex;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] SpriteRenderer spriteRenderer;

    private List<BulletPool> mainShotBulletPool; // トリガ（メイン＋各オプションが購読）
    private BulletPool subShotBulletPool; // トリガ（メイン＋各オプションが購読）


    int spriteIndex = 0; // スプライトのインデックス
    int spriteChangeInterval = 30; // スプライトを変更する間隔

    void OnEnable()
    {
        if (player != null)
        {
            PlayerTrail.Prefill(player.position);
            transform.position = player.position;
        }
    }

    public void Initialize(PlayerTrail playerTrail1, Transform transform, OptionIndex index)
    {
        mainShotBulletPool = new List<BulletPool>(); // ← 初期化！
        mainShotBulletPool.Clear();
        this.playerTrail = playerTrail1;
        optionIndex = (int)index;
        player = transform;
        transform.position = player.position;
    }
    public void UpdatePosition()
    {
        if (player == null) return;
        int need = delayFramesPerOption * (optionIndex + 2);
        if (playerTrail.TryGetDelayed(need, out var target))
            transform.position = target;
        else
            transform.position = player.position;
    }
    void Update()
    {
        UpdateSprite();
    }
    void UpdateSprite()
    {
        spriteIndex++;
        if (spriteIndex >= sprites.Count * spriteChangeInterval)
        {
            spriteIndex = 0; // スプライトのインデックスをリセット
        }
        spriteRenderer.sprite = sprites[spriteIndex / spriteChangeInterval]; // スプライトを更新
    }
    public void FireMain()
    {
        foreach (var pool in mainShotBulletPool)
        {
            pool.Fire(transform.position, BulletOwner.Player);
        }
    }
    public void FireSub() => subShotBulletPool?.Fire(transform.position, BulletOwner.Player);
    public void CleanMainShot()
    {
        foreach (BulletPool obj in mainShotBulletPool)
        {
            obj.Dead();
            Destroy(obj);
        }
        mainShotBulletPool.Clear();
    }
    public void AddMainShot(BulletPool bulletPool)
    {
        mainShotBulletPool.Add(bulletPool);
    }
    public void AddSubShot(BulletPool bulletPool)
    {
        subShotBulletPool = bulletPool;
    }
    public void Reset()
    {
        CleanMainShot();
        Destroy(subShotBulletPool);
    }

}