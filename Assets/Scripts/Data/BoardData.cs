using UnityEngine;

public class BoardData
{
    public int mWidth = 0;
    public int mHeight = 0;

    // 보드에 깔릴 타일과 블럭
    public BackGroundTile[,] mTiles = null;
    public Block[,] mBlocks = null;

    // 타일 사이즈
    public Vector2 mTileSize = Vector2.zero;

    // 오프셋
    public float mOffsetX = 0;
    public float mOffsetY = 0;
}