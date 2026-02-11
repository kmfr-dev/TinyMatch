using UnityEngine;

[System.Serializable]
public class BoardConfig
{
    public int Width = 0;
    public int Height = 0;
    public int MinMatchCount = 0;
    public GameObject TilePrefab = null;
    public GameObject[] mBlockPrefabs = null;
}
