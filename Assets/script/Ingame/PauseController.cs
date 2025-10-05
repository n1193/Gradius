using UnityEngine;
using Zenject;

public class PauseController : MonoBehaviour
{
    [SerializeField] GameObject overlay;          // ポーズ中に出すUI
    [SerializeField] SoundManager soundManager;   // ★追加：通知先（未設定ならStartで探す）

    bool _paused;
    float _savedFixed;
    [Inject]
    public void Construct(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }


    void Start()
    {
        _paused = false;
        soundManager._isPaused = _paused;
    }

    void Toggle()
    {
        _paused = !_paused;

        if (_paused)
        {
            _savedFixed = Time.fixedDeltaTime;
            Time.timeScale = 0f;            // 物理/アニメ/WaitForSeconds を停止
            AudioListener.pause = true;     // （全部の音を止めたい場合はこのまま）
            soundManager?.SetPaused(true);  // ★追加：BGMフェード用コルーチンも止める
            overlay?.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = _savedFixed;
            AudioListener.pause = false;
            soundManager?.SetPaused(false); // ★追加：BGMを位置維持で再開、フェードも続きから
            overlay?.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Toggle();
    }
}
