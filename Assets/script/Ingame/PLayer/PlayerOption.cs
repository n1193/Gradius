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

    public Action<Vector3> mainShot; // トリガ（メイン＋各オプションが購読）
    public Action<Vector3> subShot; // トリガ（メイン＋各オプションが購
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
    public void SetMainShot(Action<Vector3> action)
    {
        Debug.Log($"Option SetMainShot");
        mainShot = action;
    }
    public void SetSubShot(Action<Vector3> action)
    {
        subShot = action;
    }
    public void FireMain() => mainShot?.Invoke(transform.position);
    public void FireSub() => subShot?.Invoke(transform.position);
}