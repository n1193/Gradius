using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum SceneType
{
    TitleScene,
    IngameScene
}

public class SceneRouter : MonoBehaviour
{
    [SerializeField] CanvasGroup fade;     // 黒Image付き
    [SerializeField] float fadeTime = 0.35f;
    [SerializeField]SoundManager soundManager;

    bool isLoading;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (fade) fade.alpha = 0f;
    }

    public void Go(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(LoadRoutine(sceneName));
    }
    IEnumerator LoadRoutine(string sceneName)
    {
        isLoading = true;

        // 暗転
        yield return Fade(1f);

        // 非同期ロード
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;
        while (soundManager.IsBgmPlayingNow()) yield return null; //再生が終わったら移動する。

        op.allowSceneActivation = true;

        while (!op.isDone) yield return null;
        

        // 不要アセット解放（必要な時だけ）
        // yield return Resources.UnloadUnusedAssets();

        // 明転
        yield return Fade(0f);

        isLoading = false;
    }

    IEnumerator Fade(float to)
    {
        if (!fade) yield break;
        fade.blocksRaycasts = true;
        float from = fade.alpha, t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            fade.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        fade.alpha = to;
        fade.blocksRaycasts = to > 0.99f;
    }
}
