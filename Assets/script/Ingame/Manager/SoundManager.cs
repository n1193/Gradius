using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public enum BGMType { None = -1, Title, Ingame1, Ingame2, Boss }
public enum SEType { None = -1,Title,PlayerShot, Damage, EnemyDamage, Upgrade, BossDmage, EnemyDestroy, BossDestroy, GetUpgradeItem, PlayerDead,Laser }
[Serializable] public class BGMEntry { public BGMType type; public AudioClip clip; }
[Serializable] public class SEEntry  { public SEType  type; public AudioClip clip; }

public enum RespawnMusicPolicy {
    ResumeFromTimestamp,   // 直前の位置から続き再生
    RestartCurrentTrack,   // 現在曲の頭から
    StartFromSpecificTrack // 任意の曲（例：2曲目=Ingame2）から
}

public struct BgmPointer {
    public BGMType type;
    public int timeSamples;        // サンプル数で保持（精度◎）
    public BgmPointer(BGMType t, int s) { type = t; timeSamples = s; }
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] List<BGMEntry> bgmclip = new();
    [SerializeField] List<SEEntry> seclip = new();
    AudioSource[] bgmAudioSource = new AudioSource[2];
    AudioSource seAudioSource;

    [Header("Mixer (optional)")]
    [SerializeField] private AudioMixer _mixer;               // 無くても動く
    [SerializeField] private string _musicVolumeParam = "MusicVol"; // Exposed Param(例:-80〜0dB)

    [Header("Settings")]
    [SerializeField] private float _defaultFadeSeconds = 1.5f;
    [SerializeField] private bool _logarithmicFade = true;    // dBでフェード（推奨）
    [SerializeField] private bool _stopPrevWhenSilent = true; // フェード完了で完全停止
    private Coroutine _currentFade;
    private Coroutine _ingameRoutine;
    public bool _isPaused;
    [SerializeField] private BGMType[] _sequence = { BGMType.Ingame1, BGMType.Ingame2, BGMType.Boss };
    private int _seqIndex = 0;
    private Coroutine _seqRoutine;

    void Start()
    {
        bgmAudioSource[0] = gameObject.AddComponent<AudioSource>();
        bgmAudioSource[1] = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();
        foreach (var s in bgmAudioSource)
        {
            s.playOnAwake = false;
            s.loop = true;
            s.spatialBlend = 0f;
            s.volume = 1f;
        }
        seAudioSource.playOnAwake = false;
        seAudioSource.spatialBlend = 0f;

        if (SceneManager.GetActiveScene().name == SceneType.IngameScene.ToString())
        {
            // ★ 巡回ハンドルを保持するように変更
            _ingameRoutine = StartCoroutine(InGameBGMPlay());
        }
    }
    public void BGMPlay(BGMType bgmtype, bool loop = true, float fadeSeconds = -1f, bool restartIfSame = false)
    {
        Debug.Log("bgmtype" + bgmtype.ToString());
        BGMPlayAt(bgmtype, startTimeSamples: 0, loop: loop, fadeSeconds: fadeSeconds, restartIfSame: restartIfSame);
    }

    public void BGMPlayAt(BGMType bgmtype, int startTimeSamples, bool loop = true, float fadeSeconds = -1f, bool restartIfSame = false)
    {
        var entry = bgmclip.Find(b => b.type == bgmtype && b.clip != null);
        if (entry == null) { Debug.LogWarning($"[BGM] {bgmtype} が未設定/未割当"); return; }
        if (fadeSeconds < 0f) fadeSeconds = 0; //_defaultFadeSeconds;

        var a = bgmAudioSource[0];
        var b = bgmAudioSource[1];
        var next = a.isPlaying ? b : a;
        var prev = a.isPlaying ? a : (b.isPlaying ? b : null);

        // 同一曲最適化
        if (!restartIfSame && prev != null && prev.clip == entry.clip && prev.isPlaying) return;

        if (_currentFade != null) StopCoroutine(_currentFade);

        next.clip = entry.clip;
        next.loop = loop;
        next.pitch = 1f;
        next.volume = 0f;
        // ★ここが追加：再生開始位置（サンプル）
        next.timeSamples = Mathf.Clamp(startTimeSamples, 0, entry.clip.samples - 1);
        next.Play();

        _currentFade = StartCoroutine(CrossFadeRoutine(prev, next, fadeSeconds));
    }
    public void SEPlay(SEType seType, float vloume = 1f)
    {
        var entry = seclip.Find(e => e.type == seType && e.clip != null);
        if (entry == null) { Debug.LogWarning($"[SE] {seType} が未設定/未割当"); return; }
        seAudioSource.PlayOneShot(entry.clip, Mathf.Clamp01(vloume));
    }
    public void ResumeAutoSequenceInIngame()
    {
        if (_ingameRoutine == null)
        {
            _ingameRoutine = StartCoroutine(InGameBGMPlay());
        }
    }
    public void BGMStop()
    {
        if (bgmAudioSource[0].isPlaying) bgmAudioSource[0].Stop();
        if (bgmAudioSource[1].isPlaying) bgmAudioSource[1].Stop();
    }
    public bool IsBgmPlayingNow() =>
    (bgmAudioSource[0] != null && bgmAudioSource[0].isPlaying) ||
    (bgmAudioSource[1] != null && bgmAudioSource[1].isPlaying);

    // いま再生中のBGMソースを返す（なければ null）
    // いま再生中のBGMソース取得
    private AudioSource GetCurrentBgmSource()
    {
        if (bgmAudioSource[0] != null && bgmAudioSource[0].isPlaying) return bgmAudioSource[0];
        if (bgmAudioSource[1] != null && bgmAudioSource[1].isPlaying) return bgmAudioSource[1];
        return null;
    }


    /// <summary>
    /// 指定BGMを再生し、曲末でフェードアウトして停止するまで待つ。
    /// ・loop=false 固定。ポーズ中は時間が進まない（＝ゲーム時間同期）
    /// ・フェードは unscaledDeltaTime で進むが、_isPaused 中は停止させたいなら
    ///   CrossFadeRoutine/FadeOutThenStop 内に `while(_isPaused) yield return null;` を入れておくこと
    /// // 指定曲を再生 → 終盤でフェードアウト → 停止まで待機（ポーズ対応）
    /// </summary>
    public IEnumerator PlayAndWaitForEnd(BGMType type, float fadeOutSeconds = 1.2f)
    {
        // 即切替したくないなら fadeSeconds を好みで設定
        BGMPlay(type, loop: false, fadeSeconds: 0f);

        // 再生ソースが掴めるまで1フレーム待つ
        AudioSource src = null;
        while (src == null)
        {
            src = GetCurrentBgmSource();
            if (src != null && src.clip != null) break;
            yield return null;
        }
        src.loop = false;

        // 曲末監視（あなたの AutoStopAtClipEnd を使う手もある）
        bool fadeStarted = false;
        var clip = src.clip;

        while (src != null && src.clip == clip)
        {
            while (_isPaused) yield return null;

            int remainSamples = clip.samples - src.timeSamples;
            float remainSec = (float)remainSamples / clip.frequency;

            if (!fadeStarted && remainSec <= fadeOutSeconds)
            {
                fadeStarted = true;
                StartCoroutine(FadeOutThenStop(src, Mathf.Max(0.01f, remainSec)));
            }
            if (remainSec <= 0.01f) break;
            yield return null;
        }
    }
    IEnumerator InGameBGMPlay()
    {
        // 1曲目：曲末で自然フェード→停止まで待ってから次へ
        yield return PlayAndWaitForEnd(BGMType.Ingame1, fadeOutSeconds: 1.2f);

        // 2曲目
        yield return PlayAndWaitForEnd(BGMType.Ingame2, fadeOutSeconds: 1.2f);

        // 3曲目（ボスはループ）
        BGMPlay(BGMType.Boss, loop: true, fadeSeconds: 1.0f);
    }

    private IEnumerator CrossFadeRoutine(AudioSource from, AudioSource to, float seconds)
    {
        float t = 0f;

        // 対数（dB）フェード：最後まで自然。線形にしたいなら _logarithmicFade=false
        float fromStart = from != null ? from.volume : 0f;
        float toStart = to.volume;

        while (t < seconds)
        {
            while (_isPaused) yield return null;   // ★追加：ポーズ中はフェード進行を止める
            t += Time.unscaledDeltaTime; // ポーズ中も進む
            float p = Mathf.Clamp01(t / seconds);

            if (_logarithmicFade)
            {
                if (from != null)
                {
                    float db = Mathf.Lerp(LinToDb(fromStart), -80f, p); // 0dB相当→-80dB
                    from.volume = DbToLin(db);
                }
                float dbTo = Mathf.Lerp(LinToDb(toStart), 0f, p);       // -∞〜小音量→0dB
                to.volume = DbToLin(dbTo);
            }
            else
            {
                if (from != null) from.volume = Mathf.Lerp(fromStart, 0f, p);
                to.volume = Mathf.Lerp(toStart, 1f, p);
            }
            yield return null;
        }

        if (from != null)
        {
            from.volume = 0f;
            if (_stopPrevWhenSilent)
            {
                from.Stop();
                from.clip = null;
            }
        }
        to.volume = 1f;
        _currentFade = null;
    }

    // フェードアウト停止（従来の即Stopをやめて自然に止める）
    public void BGMStop(float fadeSeconds = -1f)
    {
        if (fadeSeconds < 0f) fadeSeconds = _defaultFadeSeconds;

        if (_currentFade != null) StopCoroutine(_currentFade);

        // どちらか鳴ってる方を探す
        AudioSource playing = null;
        if (bgmAudioSource[0].isPlaying) playing = bgmAudioSource[0];
        else if (bgmAudioSource[1].isPlaying) playing = bgmAudioSource[1];

        if (playing != null) _currentFade = StartCoroutine(FadeOutThenStop(playing, fadeSeconds));
    }

    private IEnumerator FadeOutThenStop(AudioSource src, float seconds)
    {
        float start = src.volume;
        float t = 0f;
        while (t < seconds)
        {
            while (_isPaused) yield return null;   // ★追加
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / seconds);

            if (_logarithmicFade)
            {
                float db = Mathf.Lerp(LinToDb(start), -80f, p);
                src.volume = DbToLin(db);
            }
            else
            {
                src.volume = Mathf.Lerp(start, 0f, p);
            }
            yield return null;
        }
        src.Stop();
        src.clip = null;
        src.volume = 1f;
        _currentFade = null;
    }

    // dB/線形 変換
    private static float DbToLin(float db) => Mathf.Pow(10f, db / 20f);
    private static float LinToDb(float lin) => lin <= 0f ? -80f : 20f * Mathf.Log10(lin);
    /// <summary>
    /// 指定BGMを再生し、曲の「時間が来たら」フェードアウトして停止する。
    /// ・ポーズ中（AudioSource.Pause / AudioListener.pause）も誤作動しない
    /// ・フェードは unscaledDeltaTime で進む = ポーズ中もフェード進行させたいなら true のままでOK
    /// </summary>
    public void BGMPlayAutoStop(BGMType bgmtype, float fadeOutSeconds = 1.0f, bool pauseAware = true)
    {
        // 既存のBGMPlay（クロスフェード）で流し始める
        BGMPlay(bgmtype, loop: false, fadeSeconds: 0.0f); // ここでは即切替。フェード開始は終盤で行う

        // 今鳴っている方を取得（あなたの実装に合わせて）
        AudioSource playing = null;
        if (bgmAudioSource[0].isPlaying) playing = bgmAudioSource[0];
        else if (bgmAudioSource[1].isPlaying) playing = bgmAudioSource[1];

        if (playing == null || playing.clip == null) return;

        // 念のためループは切る（「時間で終わる」運用なので）
        playing.loop = false;

        // 進行中の自動停止監視を止めたい場合は、ここで専用Coroutine参照を管理して止める
        StartCoroutine(AutoStopAtClipEnd(playing, fadeOutSeconds, pauseAware));
    }
    private IEnumerator AutoStopAtClipEnd(AudioSource src, float fadeOutSeconds, bool pauseAware)
    {
        var clip = src.clip;
        if (clip == null) yield break;

        // フェード秒が曲長を超えていたら安全にクランプ
        fadeOutSeconds = Mathf.Clamp(fadeOutSeconds, 0f, Mathf.Max(0.0f, clip.length - 0.01f));

        bool fadeStarted = false;

        while (src != null && clip != null)
        {
            while (_isPaused) yield return null;   // ★追加（timeSamples も止まるが明示で安全）
            // 再生位置（サンプル）と残り秒
            int timeSamples = src.timeSamples;                   // Pause中は進まない
            int total = clip.samples;
            int remainSmp = Mathf.Max(0, total - timeSamples);
            float remainSec = (float)remainSmp / clip.frequency;
            // 残りがフェード時間以内に入ったらフェード開始（1回だけ）
            if (!fadeStarted && remainSec <= fadeOutSeconds)
            {
                fadeStarted = true;

                // フェードは unscaledDeltaTime で進める（＝ポーズ中も減衰させたい挙動を維持）
                // 「ポーズ中はフェードも止めたい」なら FadeOutThenStop を DeltaTime 版に差し替えるだけ
                StartCoroutine(FadeOutThenStop(src, Mathf.Max(0.01f, remainSec)));
            }

            // 極短クリップ対策：0.01秒以下は即ブレイク
            if (remainSec <= 0.01f) break;

            // 監視更新
            // pauseAware が false の場合でも、timeSamples はPauseで止まるため、
            // 「ポーズ中もカウントダウンしたい」ならDSP時計版を別途用意（下にサンプルあり）
            yield return null;
        }
    }

    // BGMだけを一括 Pause/UnPause するAPI
    public void SetPaused(bool paused)
    {
        _isPaused = paused;
        foreach (var s in bgmAudioSource)
        {
            if (s == null) continue;
            if (paused)
            {
                if (s.isPlaying) s.Pause();   // 位置保持で停止
            }
            else
            {
                if (s.clip != null) s.UnPause(); // 続きから再生
            }
        }
        // ※SEを止めたいなら seAudioSource も同様に処理
    }
    // ★ 追加：外部から 1/2 どちらからでも開始可能（最終的に Boss へ）
    public void StartSequenceFrom(BGMType startType, float fadeSeconds = 1.0f)
    {
        // 既存巡回が生きていたら止める
        StopIngameSequenceIfAny();

        if (startType == BGMType.Ingame1)
        {
            // 既存の 1→2→Boss 巡回をそのまま使う
            _ingameRoutine = StartCoroutine(InGameBGMPlay());
        }
        else if (startType == BGMType.Ingame2)
        {
            // 2 から始めて、自然に Boss へ繋ぐ
            _ingameRoutine = StartCoroutine(InGameBGMPlayFrom2());
        }
        else if (startType == BGMType.Boss)
        {
            // いきなり Boss 常駐（必要なら）
            BGMPlay(BGMType.Boss, loop: true, fadeSeconds: fadeSeconds, restartIfSame: true);
            _ingameRoutine = null; // ループ常駐なので巡回は無し
        }
        else
        {
            // デフォルトは 1 から
            _ingameRoutine = StartCoroutine(InGameBGMPlay());
        }
    }
    // ★ 追加：Ingame2 の末尾まで再生 → Boss へ切替（Boss はループ）
    private IEnumerator InGameBGMPlayFrom2()
    {
        // 2曲目：曲末で自然フェード→停止まで待ってから次へ
        yield return PlayAndWaitForEnd(BGMType.Ingame2, fadeOutSeconds: 1.2f);

        // 3曲目（= Boss）はループで常駐
        BGMPlay(BGMType.Boss, loop: true, fadeSeconds: 1.0f, restartIfSame: true);

        // Boss で居座るのでループ巡回不要
        _ingameRoutine = null;
    }
    // ★ 追加：巡回中なら止める
    private void StopIngameSequenceIfAny()
    {
        if (_ingameRoutine != null)
        {
            StopCoroutine(_ingameRoutine);
            _ingameRoutine = null;
        }
    }
}