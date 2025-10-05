[System.Serializable]
public class SpawnPointData
{
    public EnemyType enemyType;
    public float x;
    public float y;
    public int count;
    public float interval = 0.25f;
    public bool drop = false;
    public string spawnPos = "CustomPoint"; // 必要なら SpawnPosition に変換
    public float delay = 0f;
}
