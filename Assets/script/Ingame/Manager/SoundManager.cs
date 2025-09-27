using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum BGMType { None = -1, Title, Ingame1, Ingame2, Boss }
public enum SEType { None = -1,Title,PlayerShot, Damage, EnemyDamage, Upgrade, BossDmage, EnemyDestroy, BossDestroy, GetUpgradeItem, PlayerDead,Laser }
[Serializable] public class BGMEntry { public BGMType type; public AudioClip clip; }
[Serializable] public class SEEntry  { public SEType  type; public AudioClip clip; }

public class SoundManager : MonoBehaviour
{
    [SerializeField] List<BGMEntry> bgmclip = new();
    [SerializeField] List<SEEntry> seclip = new();

    AudioSource[] bgmAudioSource = new AudioSource[2];
    AudioSource seAudioSource;

    void Start()
    {
        bgmAudioSource[0] = gameObject.AddComponent<AudioSource>();
        bgmAudioSource[1] = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();
        //BGMPlay(BGMType.Ingame1, false);
        //SceneRotar.GetActiveScene().name == "InGame";
        if (SceneManager.GetActiveScene().name == SceneType.IngameScene.ToString())
        {
            StartCoroutine(InGameBGMPlay());
        }
        
    }

    public void BGMPlay(BGMType bgmtype, bool loop = true)
    {
        var entry = bgmclip.Find(b => b.type == bgmtype && b.clip != null);
        if (entry == null) { Debug.LogWarning($"[BGM] {bgmtype} が未設定/未割当"); return; }

        var a = bgmAudioSource[0];
        var b = bgmAudioSource[1];
        var next = a.isPlaying ? b : a;
        var prev = a.isPlaying ? a : (b.isPlaying ? b : null);

        if (prev) prev.Stop();

        next.clip = entry.clip;
        next.volume = 1f;
        next.loop = loop;
        next.Play();
    }

    public void SEPlay(SEType seType, float vloume = 1f)
    {
        var entry = seclip.Find(e => e.type == seType && e.clip != null);
        if (entry == null) { Debug.LogWarning($"[SE] {seType} が未設定/未割当"); return; }
        seAudioSource.PlayOneShot(entry.clip, Mathf.Clamp01(vloume));
    }

    public void BGMStop()
    {
        if (bgmAudioSource[0].isPlaying) bgmAudioSource[0].Stop();
        if (bgmAudioSource[1].isPlaying) bgmAudioSource[1].Stop();
    }
    public bool IsBgmPlayingNow() =>
    (bgmAudioSource[0] != null && bgmAudioSource[0].isPlaying) ||
    (bgmAudioSource[1] != null && bgmAudioSource[1].isPlaying);

    IEnumerator InGameBGMPlay()
    {
        // 1曲目

        BGMPlay(BGMType.Ingame1, loop:false);
        yield return new WaitWhile(() => IsBgmPlayingNow());

        // 2曲目
        BGMPlay(BGMType.Ingame2, loop:false);
        yield return new WaitWhile(() => IsBgmPlayingNow());

        // 3曲目（ループ）
        BGMPlay(BGMType.Boss, loop:true); 
    }

    /*
#if UNITY_EDITOR
    void OnValidate()
    {
        NormalizeBgm();
        NormalizeSe();
    }
    void NormalizeBgm()
    {
        if (bgmclip == null) return;

        // 1) 確定行(clipあり & type!=None)の使用済みtypeを収集
        var used = new HashSet<BGMType>();
        foreach (var e in bgmclip)
            if (e != null && e.clip != null && e.type != BGMType.None) used.Add(e.type);

        // 2) 未確定行(clip==null)は必ず None にする／確定行で None のままなら未使用typeを自動割当
        foreach (var e in bgmclip)
        {
            if (e == null) continue;
            if (e.clip == null)
            {
                e.type = BGMType.None; // 追加直後の行を消さないため
            }
            else if (e.type == BGMType.None)
            {
                e.type = FirstUnused(used);
                used.Add(e.type);
            }
        }

        // 3) 確定行どうしの type 重複は後ろを削除
        var seen = new HashSet<BGMType>();
        for (int i = bgmclip.Count - 1; i >= 0; --i)
        {
            var e = bgmclip[i];
            if (e == null) { bgmclip.RemoveAt(i); continue; }

            if (e.clip != null && e.type != BGMType.None)
            {
                if (!seen.Add(e.type))
                {
                    Debug.LogError($"[SoundManager] BGM 重複: {e.type} → 後の要素を削除", this);
                    bgmclip.RemoveAt(i);
                }
            }
            // clip==null は残す
        }
    }

    void NormalizeSe()
    {
        if (seclip == null) return;

        var used = new HashSet<SEType>();
        foreach (var e in seclip)
            if (e != null && e.clip != null && e.type != SEType.None) used.Add(e.type);

        foreach (var e in seclip)
        {
            if (e == null) continue;
            if (e.clip == null)
            {
                e.type = SEType.None;
            }
            else if (e.type == SEType.None)
            {
                e.type = FirstUnused(used);
                used.Add(e.type);
            }
        }

        var seen = new HashSet<SEType>();
        for (int i = seclip.Count - 1; i >= 0; --i)
        {
            var e = seclip[i];
            if (e == null) { seclip.RemoveAt(i); continue; }

            if (e.clip != null && e.type != SEType.None)
            {
                if (!seen.Add(e.type))
                {
                    Debug.LogError($"[SoundManager] SE 重複: {e.type} → 後の要素を削除", this);
                    seclip.RemoveAt(i);
                }
            }
        }
    }

    // 未使用Enum値を返す（全部埋まってたら最初の値）
    BGMType FirstUnused(HashSet<BGMType> used)
    {
        foreach (BGMType v in Enum.GetValues(typeof(BGMType)))
            if (v != BGMType.None && !used.Contains(v)) return v;
        return BGMType.Title;
    }
    SEType FirstUnused(HashSet<SEType> used)
    {
        foreach (SEType v in Enum.GetValues(typeof(SEType)))
            if (v != SEType.None && !used.Contains(v)) return v;
        return SEType.PlayerShot;
    }
#endif
*/
}
