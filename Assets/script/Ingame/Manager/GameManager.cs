using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public RespawnManager respawnManager;

    public enum GameState
    {
        Playing,
        Dying,
        Respawning,
        GameOver,
        StageClear
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    SoundManager soundManager;
    ScrollDirector scrollDirector;

    [Inject]
    public void Construct(RespawnManager respawnManager, SoundManager soundManager, ScrollDirector scrollDirector)
    {
        this.respawnManager = respawnManager;
        this.soundManager = soundManager;
        this.scrollDirector = scrollDirector;
    }

    // 💀 プレイヤー死亡通知（PlayerController から呼ばれる）
    public void OnPlayerDeath()
    {
        if (CurrentState != GameState.Playing) return;
        Debug.Log("[GameManager] Player death detected → waiting in PlayerLife coroutine");
        CurrentState = GameState.Dying;
        // ⚠️ この時点ではリスポーン処理しない
        // PlayerLife 側の Die() コルーチンがタイミングを管理する
    }

    // 🌀 PlayerLife から呼ばれる
    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting from checkpoint...");
        CurrentState = GameState.Respawning;

        if (respawnManager != null)
        {
            respawnManager.OnPlayerDeath();  // ← チェックポイント復帰処理
        }
        else
        {
            Debug.LogWarning("RespawnManager is not assigned in GameManager!");
        }

        StartCoroutine(ReturnToPlaying());
    }

    private IEnumerator ReturnToPlaying()
    {
        yield return new WaitForSeconds(0.5f);
        CurrentState = GameState.Playing;
        Debug.Log("[GameManager] Back to Playing state.");
    }

    // 🚫 残機が尽きたら
    public void OnGameOver()
    {
        if (CurrentState == GameState.GameOver) return;
        Debug.Log("[GameManager] GameOver");
        scrollDirector.SetPlayerPause(true);
        StartCoroutine(GameOverAction());
    }

    // 🏁 ステージクリア
    public void OnStageClear()
    {
        CurrentState = GameState.StageClear;
        Debug.Log("[GameManager] Stage Clear!");
        SceneManager.LoadScene(SceneType.TitleScene.ToString());
    }
    IEnumerator GameOverAction()
    {
        soundManager.SEPlay(SEType.GameOver);

        yield return new WaitForSeconds(5f); // ウェーブ間の待機時間
        CurrentState = GameState.GameOver;
        SceneManager.LoadScene(SceneType.TitleScene.ToString());
    }
}
