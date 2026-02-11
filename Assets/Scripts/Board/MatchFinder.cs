using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    private static readonly Vector2Int[] mDir =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    // 지금 매치된 모든 블럭들을 얻어온느 함수
    public static List<List<Block>> FindAllMatches(this Block[,] _blocks, int _minMatch)
    {
        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);

        List<List<Block>> allMatches = new List<List<Block>>();
        
        // 방문여부 배열
        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 방문하지 않았고, 블럭이 있다면
                if (false == visited[x, y] && null != _blocks[x, y])
                {
                    List<Block> connected = FindConnectedBlocks(_blocks, x, y, visited);
                    
                    // 블럭수가 최소 매치수보다 크다면 블럭들을 추가
                    if (connected.Count >= _minMatch)
                    {
                        allMatches.Add(connected);
                    }
                }
            }
        }
        return allMatches;
    }

    // 현재 연결된 블럭반환하는 함수
    private static List<Block> FindConnectedBlocks(Block[,] _blocks, int _startX, int _startY, bool[,] _visited)
    {
        // 블럭들을 담을 리스트
        List<Block> connected = new List<Block>();

        // 시작위치의 블럭 타입을 얻어온다.
        string targetType = _blocks[_startX, _startY].mBlockData.BlockType;

        // BFS를 위한 큐
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // 시작 좌표 삽입
        queue.Enqueue(new Vector2Int(_startX, _startY));

        // 해당위치 방문 처리
        _visited[_startX, _startY] = true;


        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);

        // BFS 시작
        while (queue.Count > 0)
        {
            Vector2Int curr = queue.Dequeue();
            connected.Add(_blocks[curr.x, curr.y]);

            foreach (Vector2Int dir in mDir)
            {
                // 다음좌표 계산
                int nextX = curr.x + dir.x;
                int nextY = curr.y + dir.y;

                // 유효좌표가 아니면 건너뜀
                if (nextX < 0 || nextY < 0 || nextX >= width || nextY >= height)
                    continue;

                // 이미 방문했다면 건너뜀
                if (_visited[nextX, nextY])
                    continue;

                // 다음좌표의 블럭이 비어있다면 건너뜀
                if (null == _blocks[nextX, nextY])
                    continue;

                // 다음좌표의 블럭이 시작위치의 블럭타입과 맞지않다면 건너뜀
                if (targetType != _blocks[nextX, nextY].mBlockData.BlockType)
                    continue;

                // 방문처리 및 다음좌표 삽입
                _visited[nextX, nextY] = true;
                queue.Enqueue(new Vector2Int(nextX, nextY));
            }
        }

        return connected;
    }

    public static bool HasPossibleMoves(this Block[,] _blocks, int _minMatch)
    {
        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (null == _blocks[x, y]) 
                    continue;

                Vector2Int src = new Vector2Int(x, y);
                Vector2Int dest = new Vector2Int(x + 1, y);

                // 오른쪽/위쪽 블록과 교환 시뮬레이션
                if (x < width - 1 && WouldMatchAfterSwap(_blocks, src, dest, _minMatch)) 
                    return true;

                dest.x = x;
                dest.y = y + 1;

                if (y < height - 1 && WouldMatchAfterSwap(_blocks, src, dest, _minMatch)) 
                    return true;
            }
        }
        return false;
    }

    private static bool WouldMatchAfterSwap(Block[,] _blocks, in Vector2Int _p1, in Vector2Int _p2, int _minMatch)
    {
        // 시뮬레이션용 카운트 (실제 배열을 바꾸지 않고 타입만 체크)
        // 만약 시뮬을 돌렸을 때 최소 매치수 보다 크다면 매치가 가능하다는 뜻
        if (CountConnectedSim(_blocks, _p1, _blocks[_p2.x, _p2.y].mBlockData.BlockType, _p1, _p2) >= _minMatch) 
            return true;
        if (CountConnectedSim(_blocks, _p2, _blocks[_p1.x, _p1.y].mBlockData.BlockType, _p1, _p2) >= _minMatch) 
            return true;

        return false;
    }

    private static int CountConnectedSim(Block[,] _blocks, in Vector2Int _startPos, string _type, in Vector2Int _p1, in Vector2Int _p2)
    {
        // 가로 합산 (좌, 우 , 자신)
        int horizanCount = 1 + CountInDir(_blocks, _startPos, Vector2Int.left, _type, _p1, _p2) +
                                CountInDir(_blocks, _startPos, Vector2Int.right, _type, _p1, _p2);
        
        // 세로 합산 (상, 하, 자신)
        int verticalCount = 1 + CountInDir(_blocks, _startPos, Vector2Int.up, _type, _p1, _p2)
                            + CountInDir(_blocks, _startPos, Vector2Int.down, _type, _p1, _p2);

        return Mathf.Max(horizanCount, verticalCount);
    }

    private static int CountInDir(Block[,] _blocks, in Vector2Int _curPos, in Vector2Int _dir, string _type, in Vector2Int _p1, in Vector2Int _p2)
    {
        int width = _blocks.GetLength(0); 
        int height = _blocks.GetLength(1);

        int count = 0;
        Vector2Int next = _curPos + _dir;

        while (true)
        {
            // 유효좌표 확인
            if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                break;

            // 다음좌표 블럭확인
            if (null == _blocks[next.x, next.y])
                break;

            // 가상 타입결정,
            // 다음(next)이 p1이나 p2라면 서로의 타입을 바꾼다.
            string currentType = string.Empty;

            if (next == _p1)
                currentType = _blocks[_p2.x, _p2.y].mBlockData.BlockType;
            else if (next == _p2)
                currentType = _blocks[_p1.x, _p1.y].mBlockData.BlockType;
            else
                currentType = _blocks[next.x, next.y].mBlockData.BlockType;

            // 만약 가상타입이 인자로 넘어온 타입과 다르면 break;
            if (_type != currentType)
                break;

            ++count;
            next += _dir;
        }

        return count;
    }
}
