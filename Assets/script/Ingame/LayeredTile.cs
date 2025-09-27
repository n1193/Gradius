using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom/LayeredTile")]
public class LayeredTile : Tile
{
    public GameObject solidPrefab;
    public GameObject ghostPrefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
{
    tileData.sprite = this.sprite;
    tileData.color = Color.white;
    tileData.colliderType = this.colliderType;

    // ここでPrefabを条件分岐で設定
    tileData.gameObject = (position.x % 2 == 0) ? solidPrefab : ghostPrefab;
}
}
