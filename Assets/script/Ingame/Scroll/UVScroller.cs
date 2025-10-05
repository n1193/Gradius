using UnityEngine;

/// <summary>
/// Quad に貼ったテクスチャを UV オフセットで横スクロールさせる。
/// マテリアルは共有のまま、MaterialPropertyBlock で個別オフセットだけ変えるので軽い。
/// URP/Built-in 両対応（_BaseMap / _MainTex）
/// </summary>
[RequireComponent(typeof(Renderer))]
public class UVScroller : MonoBehaviour
{
    [Tooltip("UVオフセット速度（右→左に流したいなら x>0）")]
    public Vector2 speed = new Vector2(0.25f, 0f);

    [Tooltip("UVの繰り返し倍率（1で原寸）。星密度を変えたい時に調整")]
    public Vector2 tiling = Vector2.one;

    [Tooltip("Time.unscaledDeltaTime を使う（ポーズ中も動かしたい等）")]
    public bool useUnscaledTime = false;

    // 画面にフィットさせたい場合に使うオプション：
    [Header("Fit (任意)")]
    public bool fitToCameraHeight = false;
    public Camera targetCamera;     // 未指定なら Camera.main
    public float worldHeight = 10f; // このQuadの縦サイズ（ワールド単位）を高さに合わせる

    Renderer _rend;
    MaterialPropertyBlock _mpb;
    int _stMain = Shader.PropertyToID("_MainTex_ST");
    int _stBase = Shader.PropertyToID("_BaseMap_ST");
    Vector2 _ofs;
    bool isstop;

    void Awake()
    {
        _rend = GetComponent<Renderer>();
        _mpb  = new MaterialPropertyBlock();
        if (targetCamera == null) targetCamera = Camera.main;
    }
    void Start()
    {
        if (fitToCameraHeight && targetCamera != null)
        {
            // カメラの高さにQuadをフィット（正射影/透視どちらでも可：奥行きZに注意）
            float h = worldHeight;
            float w = h * targetCamera.aspect;
            transform.localScale = new Vector3(w, h, 1f);
        }

        // 初期の ST（Tiling/Offset）を書き込む
        ApplyST();
        SetStop(false);
    }
    public void SetStop(bool isstop)
    {
        this.isstop = isstop;
    }
    void LateUpdate()
    {
        if (!isstop)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _ofs += speed * dt;

            // 0..1 に丸めて無限に回す
            _ofs.x = Mathf.Repeat(_ofs.x, 1f);
            _ofs.y = Mathf.Repeat(_ofs.y, 1f);

            ApplyST();
        }
    }

    void ApplyST()
    {
        _rend.GetPropertyBlock(_mpb);
        // ST = (tiling.x, tiling.y, offset.x, offset.y)
        var st = new Vector4(tiling.x, tiling.y, _ofs.x, _ofs.y);

        // URP は _BaseMap_ST、Built-in は _MainTex_ST
        if (_rend.sharedMaterial != null && _rend.sharedMaterial.HasProperty(_stBase))
            _mpb.SetVector(_stBase, st);
        else
            _mpb.SetVector(_stMain, st);

        _rend.SetPropertyBlock(_mpb);
    }
}
