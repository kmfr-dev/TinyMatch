using UnityEngine;
using UnityEngine.UIElements;

public class BlockData
{
    // 좌표
    public int Col = 0;
    public int Row = 0;
    // 블록타입 (매칭 판정용
    public string BlockType = string.Empty;

    public Vector2 TargetPos = Vector2.zero;
}
