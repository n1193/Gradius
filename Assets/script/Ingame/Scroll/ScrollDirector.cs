using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

public class ScrollDirector : MonoBehaviour
{
    // 停止の種類
    public enum StopKind { Timed, WaitSignal }
    public Volcano volcanoPrefab;
    [SerializeField]public GameObject volcanoInstancePoint;
    public DiContainer _container;
    [SerializeField]public UVScroller uVScroller;

    SoundManager soundManager;

    private float[] checkpointXs = { 0f, 130f, 250f };
    private int currentCheckpointIndex = -1;
    public float CurrentCheckpointX { get; private set; } = 0f;

    // 停止ポイントの定義
    [System.Serializable]
    public struct Stop
    {
        public float x;            // この X 以上で停止開始
        public StopKind kind;      // 停止の種類
        public float duration;     // Timed のとき使用（秒）
        public string signalId;    // WaitSignal のとき使用（"Boss1" 等）
    }
    // SpawnEvent: x座標到達で生成する決定論的イベント
    [System.Serializable]
    public struct SpawnEvent
    {
        public float xPos;           // ステージの絶対x（スクロール距離）
        public string prefabId;      // 登録済み辞書キー
        public float y;
        public int count;
        public float interval;
        public bool drop;
    }

    // Checkpoint: 復帰地点
    [System.Serializable]
    public struct Checkpoint
    {
        public float x;              // 復帰先のscrollX
        public int spawnIndex;       // タイムラインの再開インデックス
        public int weaponKind;       // 必要なら
        public int powerLevels;      // 例：ビットマスク/配列等
        public int optionCount;
    }

    [Header("Scroll Settings")]
    [SerializeField] float baseSpeed = 1f;   // 単位/秒（右方向に加算）
    [Tooltip("インスペクタで管理。x は右に進んだ距離（正）。")]
    List<Stop> stops =  new List<Stop> {
        new Stop { x = 260f, kind = StopKind.WaitSignal,signalId="volcano"/*Timed,duration = 16f */},
        new Stop { x = 281, kind = StopKind.WaitSignal,signalId="Boss"}
    };

    [Header("Debug")]
    [SerializeField] bool logPause = true;

    public float X { get; private set; }                 // 進行距離
    public bool IsPaused => _pauseCounter > 0;           // 何かしらの理由で止まっている
    public int PauseCount => _pauseCounter;              // 今のポーズ要因の数
    public int NextStopIndex => _nextStopIndex;          // 次に見る Stop のインデックス（UI表示などに）

    const float Eps = 0.0001f;

    int _pauseCounter = 0;
    int _nextStopIndex = 0;
    string _waitingSignal = null;
    Coroutine _timedPauseCo;

    [Inject]
    void Construct(DiContainer container, SoundManager soundManager)
    {
        _container = container;
        this.soundManager = soundManager;
    }
    void Start()
    {
        X = 0f;
        // 念のためソート（x 昇順）—手で順番を変えたときの事故防止
        stops.Sort((a, b) => a.x.CompareTo(b.x));
    }

    void Update()
    {
        if (!IsPaused)
        {
            X += baseSpeed * Time.deltaTime;
            #if Unirrty_EDITOR
            if (Input.GetKey(KeyCode.I))
            {
                X+=100;
            }
            #endif
        }        
        // すでに停止中なら到達チェックはしない
            if (IsPaused) return;
        // 次の Stop に到達したら開始
        if (_nextStopIndex < stops.Count && X + Eps >= stops[_nextStopIndex].x)
        {
            var s = stops[_nextStopIndex++];
            uVScroller.SetStop(true);
            switch (s.kind)
            {
                case StopKind.Timed:
                    // 時間停止：Add→一定秒→Remove（Realtime）
                    _timedPauseCo = StartCoroutine(PauseForSecondsRealtime(s.duration));
                    break;

                case StopKind.WaitSignal:
                    if (stops[_nextStopIndex - 1].signalId == "volcano")
                    {
                        Debug.Log("火山を作った");
                        _container.InstantiatePrefabForComponent<Volcano>(volcanoPrefab, volcanoInstancePoint.transform.position, Quaternion.identity, volcanoInstancePoint.transform);
                    }
                    AddPause();
                    _waitingSignal = s.signalId;
                    break;
            }
        } // プレイヤーが次のチェックポイントを超えたら更新
        for (int i = 0; i < checkpointXs.Length; i++)
        {
            if (X >= checkpointXs[i] && i > currentCheckpointIndex)
            {
                currentCheckpointIndex = i;
                CurrentCheckpointX = checkpointXs[i];
                Debug.Log($"Checkpoint {i + 1} reached! X={CurrentCheckpointX}");
            }
        }
    }
    
    public float GetRespawnX()
    {
        return CurrentCheckpointX;
    }

    public void SetSpeed(float s) => baseSpeed = s;

    public void SetPlayerPause(bool on)
    {
        if (on) AddPause();
        else    RemovePause();
    }

    /// <summary> ボスなどから「倒したよ」の合図。id が一致した待機のみ解除。 </summary>
    public void NotifySignal(string id)
    {
        if (!string.IsNullOrEmpty(_waitingSignal) && _waitingSignal == id)
        {
            _waitingSignal = null;
            uVScroller.SetStop(false);
            RemovePause();
            if (logPause) Debug.Log($"[ScrollDirector] WaitSignal end id={id} (PauseCount={_pauseCounter})");
        }
    }


    public void SkipCurrentStop()
    {
        // Timed を強制解除
        if (_timedPauseCo != null)
        {
            StopCoroutine(_timedPauseCo);
            _timedPauseCo = null;
            // Timed 分の Pause を外す
            RemovePause();
        }
        // Signal 待ちを強制解除
        if (!string.IsNullOrEmpty(_waitingSignal))
        {
            _waitingSignal = null;
            RemovePause();
        }

        // すでに X が次の stop を超えている場合、取りこぼしをまとめて飛ばす
        while (_nextStopIndex < stops.Count && X + Eps >= stops[_nextStopIndex].x)
        {
            _nextStopIndex++;
        }
        if (logPause) Debug.Log($"[ScrollDirector] SkipCurrentStop → next={_nextStopIndex}, X={X:F2}, PauseCount={_pauseCounter}");
    }

    public void ResetAll(float startX = 0f)
    {
        //X = startX;
        X = -97.13f;
        _nextStopIndex = 0;
        _waitingSignal = null;

        if (_timedPauseCo != null)
        {
            StopCoroutine(_timedPauseCo);
            _timedPauseCo = null;
        }

        _pauseCounter = 0;
        if (logPause) Debug.Log($"[ScrollDirector] ResetAll: X={X}, PauseCount={_pauseCounter}");
    }

    void AddPause()
    {
        _pauseCounter++;
        uVScroller.SetStop(true);
        if (logPause) Debug.Log($"[ScrollDirector] Pause++ ({_pauseCounter})");
    }

    void RemovePause()
    {
        _pauseCounter = Mathf.Max(0, _pauseCounter - 1);
        if(_pauseCounter<=0)uVScroller.SetStop(false);

        if (logPause) Debug.Log($"[ScrollDirector] Pause-- ({_pauseCounter})");
    }

    IEnumerator PauseForSecondsRealtime(float seconds)
    {
        AddPause();
        if (logPause) Debug.Log($"[ScrollDirector] Timed pause start {seconds:0.###}s (PauseCount={_pauseCounter})");
        float t = 0f;
        while (t < seconds)
        {
            // Realtime で待つ（他所の timeScale 変更の影響を受けない）
            yield return null;
            t += Time.unscaledDeltaTime;
        }
        if (logPause) Debug.Log($"[ScrollDirector] Timed pause end (PauseCount={_pauseCounter - 1})");
        RemovePause();
        _timedPauseCo = null;
    }
    // ScrollDirector.cs に追記
    public float SetScrollX()
    {
        X = checkpointXs[currentCheckpointIndex];
        // 取りこぼし防止：すでに通過済みのStopをスキップ
        while (_nextStopIndex < stops.Count && X + Eps >= stops[_nextStopIndex].x)
            _nextStopIndex++;
        _waitingSignal = null;
        if (_timedPauseCo != null) { StopCoroutine(_timedPauseCo); _timedPauseCo = null; }
        _pauseCounter = 0;
        if (logPause) Debug.Log($"[ScrollDirector] SetScrollX: X={X}, nextStop={_nextStopIndex}");
        if (currentCheckpointIndex == 0)
        {
            soundManager.StartSequenceFrom(BGMType.Ingame1);
        }
        else if (currentCheckpointIndex == 1)
        {
            soundManager.StartSequenceFrom(BGMType.Ingame2);
        }
        else
        {
            soundManager.StartSequenceFrom(BGMType.Boss);
        }
        return X;
    }

#if UNITY_EDITOR
    // インスペクタ編集時に x を昇順に整える（任意）
    void OnValidate()
    {
        if (stops != null && stops.Count > 1)
        {
            stops.Sort((a, b) => a.x.CompareTo(b.x));
        }
    }
#endif
}
