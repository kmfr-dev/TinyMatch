using UnityEngine;

[System.Serializable]
public class BoardConfig
{
    // 가로, 세로
    public int Width = 0;
    public int Height = 0;
    // 최소 매치 수 
    public int MinMatchCount = 0;
    // 블럭 및 타일 프리팹
    public GameObject TilePrefab = null;
    public GameObject[] mBlockPrefabs = null;
}
