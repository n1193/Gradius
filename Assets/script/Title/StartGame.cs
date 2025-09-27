using UnityEngine;
using Zenject;

public class StartGame : MonoBehaviour
{
    [SerializeField] SceneRouter sceneRouter;
    [SerializeField] SoundManager soundManager;
    [SerializeField] SceneType sceneName;

    [Inject]
    void Construct(SceneRouter sceneRouter,SoundManager soundManager)
    {
        this.sceneRouter = sceneRouter;  
        this.soundManager = soundManager;  
    }
    void Start()
    {
        sceneName = SceneType.IngameScene;
    }   

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.BGMPlay(BGMType.Title, false);
            sceneRouter.Go(sceneName.ToString()); // フェード＆非同期は Router 側
            enabled = false;
        }
    }

#if UNITY_EDITOR
    // Build Settings に入ってるか警告（編集時だけ）
    void OnValidate()
    {
        if (string.IsNullOrEmpty(sceneName.ToString())) return;
        bool listed = false;
        foreach (var s in UnityEditor.EditorBuildSettings.scenes)
            if (s.enabled && s.path.EndsWith($"/{sceneName}.unity")) { listed = true; break; }
        if (!listed) Debug.LogWarning($"[StartGame] '{sceneName}' が Build Settings に入ってないかも", this);
    }
#endif
}
