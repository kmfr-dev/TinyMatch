using System.Collections.Generic;
using UnityEngine;

public class MatchFinder
{
    private bool[,] mVisited = null;
    private Queue<Vector2Int> mQueue = null;

    // 방향 배열
    private readonly Vector2Int[] mDir =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
    public void Init(BoardConfig _boardConfig)
    {
        if (null == _boardConfig)
            return;

        mVisited = new bool[_boardConfig.Width, _boardConfig.Height];
        mQueue = new Queue<Vector2Int>();
    }

    // 지금 매치된 모든 블럭들을 얻어오는 함수
    public List<List<Block>> FindAllMatches(in Block[,] _blocks, int _minMatch)
    {
        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);

        List<List<Block>> allMatches = new List<List<Block>>();

        // 방문여부 배열 초기화
        System.Array.Clear(mVisited, 0, mVisited.Length);

        // 전체 블럭 순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 방문하지 않았고, 블럭이 있다면
                if (false == mVisited[x, y] && null != _blocks[x, y])
                {
                    // 연결된 블럭들을 반환받는다.
                    List<Block> connected = FindConnectedBlocks(_blocks, x, y);
                    
                    // 블럭수가 최소 매치수보다 크다면 매치된 블럭조합을 추가
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
    private List<Block> FindConnectedBlocks(in Block[,] _blocks, int _startX, int _startY)
    {
        // 블럭들을 담을 리스트
        List<Block> connected = new List<Block>();

        // 시작위치의 블럭 타입을 얻어온다.
        string targetType = _blocks[_startX, _startY].mBlockData.BlockType;

        // BFS를 위한 큐 초기화
        mQueue.Clear();

        // 시작 좌표 삽입
        mQueue.Enqueue(new Vector2Int(_startX, _startY));

        // 해당위치 방문 처리
        mVisited[_startX, _startY] = true;

        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);

        // BFS 시작
        while (mQueue.Count > 0)
        {
            Vector2Int curr = mQueue.Dequeue();
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
                if (mVisited[nextX, nextY])
                    continue;

                // 다음좌표의 블럭이 비어있다면 건너뜀
                if (null == _blocks[nextX, nextY])
                    continue;

                // 다음좌표의 블럭이 시작위치의 블럭타입과 맞지않다면 건너뜀
                if (targetType != _blocks[nextX, nextY].mBlockData.BlockType)
                    continue;

                // 방문처리 및 다음좌표 삽입
                mVisited[nextX, nextY] = true;
                mQueue.Enqueue(new Vector2Int(nextX, nextY));
            }
        }

        return connected;
    }

    // 현재 플레이어가 움직여서 매치를 만들 수 있는지 확인하는 함수
    public bool HasPossibleMoves(in Block[,] _blocks, int _minMatch)
    {
        int width = _blocks.GetLength(0);
        int height = _blocks.GetLength(1);
        
        // 블럭 전체순회
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 블럭 없으면 건너뜀
                if (null == _blocks[x, y]) 
                    continue;

                // origin : 원래 위치
                // goal : 이동할 위치 (실제로 이동하지는 않음)
                Vector2Int origin = new Vector2Int(x, y);
                Vector2Int goal = new Vector2Int(x + 1, y);

                // 오른쪽 블록과 교환 시뮬레이션
                if (x < width - 1 && WouldMatchAfterSwap(_blocks, origin, goal, _minMatch)) 
                    return true;

                // goal 위치 변경
                goal.x = x;
                goal.y = y + 1;

                // 위쪽 블록과 교환 시뮬레이션
                if (y < height - 1 && WouldMatchAfterSwap(_blocks, origin, goal, _minMatch)) 
                    return true;
            }
        }
        return false;
    }

    // 블록 스왑 시뮬레이션 중, 매치가 일어나는지 확인하는 함수
    private bool WouldMatchAfterSwap(in Block[,] _blocks, in Vector2Int _p1, in Vector2Int _p2, int _minMatch)
    {
        // 시뮬레이션용 카운트 (실제 배열을 바꾸지 않고 타입만 체크)
        // 만약 시뮬을 돌렸을 때 최소 매치수 보다 크다면 매치가 가능하다는 뜻
        if (CountConnectedSim(_blocks, _p1, _blocks[_p2.x, _p2.y].mBlockData.BlockType, _p1, _p2) >= _minMatch) 
            return true;
        if (CountConnectedSim(_blocks, _p2, _blocks[_p1.x, _p1.y].mBlockData.BlockType, _p1, _p2) >= _minMatch) 
            return true;

        return false;
    }

    // 가로, 세로 블럭을 합산하여 최대값을 반환하는 함수
    private int CountConnectedSim(in Block[,] _blocks, in Vector2Int _startPos, string _type, in Vector2Int _p1, in Vector2Int _p2)
    {
        // 가로 합산 (좌, 우 , 자신)
        int horizanCount = 1 + CountInDir(_blocks, _startPos, Vector2Int.left, _type, _p1, _p2) +
                                CountInDir(_blocks, _startPos, Vector2Int.right, _type, _p1, _p2);
        
        // 세로 합산 (상, 하, 자신)
        int verticalCount = 1 + CountInDir(_blocks, _startPos, Vector2Int.up, _type, _p1, _p2)
                            + CountInDir(_blocks, _startPos, Vector2Int.down, _type, _p1, _p2);

        return Mathf.Max(horizanCount, verticalCount);
    }

    // 인자로 방향을 넘겨 해당방향으로 블럭이 얼마나 있는지 확인 하는 함수
    private int CountInDir(in Block[,] _blocks, in Vector2Int _curPos, in Vector2Int _dir, string _type, in Vector2Int _p1, in Vector2Int _p2)
    {
        int width = _blocks.GetLength(0); 
        int height = _blocks.GetLength(1);

        // 인자로 넘어온 방향으로 몇개의 블럭이 있는지 체크하기위한 카운트
        int count = 0;
        // 인자로 넘어온 방향으로 다음좌표 계산
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

            // 여기까지 왔으면 현재 인자로 넘어온 타입과 같으므로 카운트 증가
            ++count;
            // 다음좌표 갱신
            next += _dir;
        }


        return count;
    }
}
