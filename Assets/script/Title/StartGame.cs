using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading;

public class StartGame : MonoBehaviour
{
    [Header("Scene / Sound")]
    [SerializeField] SceneRouter sceneRouter;
    [SerializeField] SoundManager soundManager;
    [SerializeField] SceneType sceneName = SceneType.IngameScene;

    [Header("Slide In (Canvas UI: 左の画面外から)")]
    [SerializeField] RectTransform slideTarget;              // 動かすUI(RectTransform)
    [SerializeField] bool useCurrentAsEnd = true;            // 現在の位置を終点にする
    [SerializeField] Vector2 endPos;                         // 手動指定したい場合
    [SerializeField, Min(0f)] float slideDuration = 5f;    // スライド時間
    [SerializeField] AnimationCurve ease =
        AnimationCurve.EaseInOut(0, 0, 1, 1);               // イージング曲線
    [SerializeField] float extraMargin = 32f;                // どれだけ画面外から出すか(px)
    [SerializeField] KeyCode actionKey = KeyCode.Space;      // スキップ＆決定キー

    CancellationTokenSource _cts;

    [Inject]
    void Construct(SceneRouter sceneRouter, SoundManager soundManager)
    {
        this.sceneRouter = sceneRouter;
        this.soundManager = soundManager;
    }

    async void Start()
    {
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        // 1) タイトルUIを左の画面外からスライドイン（Spaceでスキップ可）
        bool skipped = false;
        if (slideTarget != null)
        {
            if (useCurrentAsEnd) endPos = slideTarget.anchoredPosition;
            skipped = await SlideInFromLeft(slideTarget, endPos, slideDuration, ease, extraMargin, actionKey, ct);
        }

        // 2) スライド完了後：「Spaceをもう一回」で遷移
        //    もしスキップでSpaceが押しっぱなしなら、一度「離されるのを待って」から次の押下を待つ
        if (skipped)
            await UniTask.WaitUntil(() => !Input.GetKey(actionKey), cancellationToken: ct);
        
        await UniTask.WaitUntil(() => Input.GetKeyDown(actionKey), cancellationToken: ct);

        // 3) 遷移（必要ならBGMワンショットなど）
        soundManager?.BGMPlay(BGMType.Title, loop: false, fadeSeconds: 0f, restartIfSame: true);
        sceneRouter.Go(sceneName.ToString()); // フェード＆非同期は Router 側
        enabled = false;
    }

    void OnDestroy() => _cts?.Cancel();

    // ===== RectTransform を左外から endPos へスライド。戻り値：スキップされたら true =====
    async UniTask<bool> SlideInFromLeft(RectTransform target, Vector2 end, float duration,
                                        AnimationCurve curve, float marginPx, KeyCode key,
                                        CancellationToken ct)
    {
        // 親Rectから“左の画面外”開始位置を計算（中央アンカー前提）
        var parent = target.parent as RectTransform;
        float parentW = parent ? parent.rect.width : Screen.width;
        float selfW   = target.rect.width;
        float startX  = end.x - (parentW * 0.5f + selfW * 0.5f + marginPx);
        Vector2 start = new Vector2(startX, end.y);
        target.anchoredPosition = start;
        await UniTask.Yield(); // 初期反映の安定化
        Debug.Log("startX"+startX);
        
        if (duration <= 0f)
        {
            target.anchoredPosition = end;
            return false;
        }

        bool skipped = false;
        float t = 0f;
        while (t < duration && !ct.IsCancellationRequested)
        {
            if (Input.GetKeyDown(key)) { skipped = true; break; } // ← スキップ

            t += Time.deltaTime;                           // timeScale無視。必要なら deltaTime に
            float k = Mathf.Clamp01(t / duration);
            float e = (curve != null) ? curve.Evaluate(k) : k;
            Debug.Log("k" + k);
            target.anchoredPosition = Vector2.LerpUnclamped(start, end, e);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        // 着地を保証
        target.anchoredPosition = end;
        return skipped;
    }

#if UNITY_EDITOR
    // Build Settings に入ってるか警告（編集時だけ）
    void OnValidate()
    {
        if (string.IsNullOrEmpty(sceneName.ToString())) return;
        bool listed = false;
        foreach (var s in UnityEditor.EditorBuildSettings.scenes)
            if (s.enabled && s.path.EndsWith($"/" + sceneName + ".unity")) { listed = true; break; }
        if (!listed) Debug.LogWarning($"[StartGame] '{sceneName}' が Build Settings に入ってないかも", this);
    }
#endif
}
