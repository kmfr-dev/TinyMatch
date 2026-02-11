using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class Board : MonoBehaviour
{
    #region Variable

    // 실제 보드 데이터
    private BoardData mBoardData = null;
    // 보드 config
    private BoardConfig mBoardConfig = null;
    // 현재 매치 검사 진행중인지 판별하는 변수
    public bool mIsProcessing { get; private set; } = false;

    #endregion

    #region Setup
    // 초기화 함수
    public void Init(BoardConfig _boardData)
    {
        if (null == _boardData)
            return;

        mBoardConfig = _boardData;

        // 데이터 생성
        mBoardData = new BoardData();

        int width = mBoardConfig.Width;
        int height = mBoardConfig.Height;

        // 가로, 세로값 설정
        mBoardData.mWidth = width;
        mBoardData.mHeight = height;

        // 타일, 블럭 생성
        mBoardData.mTiles = new BackGroundTile[width, height];
        mBoardData.mBlocks = new Block[width, height];

        SpriteRenderer spriteRenderer = _boardData.TilePrefab.GetComponent<SpriteRenderer>();
        if(null == spriteRenderer) 
            return;
        
        mBoardData.mTileSize =   spriteRenderer.bounds.size;

        // 오프셋 계산
        mBoardData.mOffsetX = (width - 1) * mBoardData.mTileSize.x / 2f;
        mBoardData.mOffsetY = (height - 1) * mBoardData.mTileSize.y / 2f;


        for(int x = 0; x < width; ++x)
        {
            for(int y = 0; y < height; ++y)
                CreateTileAndBlock(x, y);
        }

        AdjustCamera(mBoardData.mTileSize);

        // 초기 매치된 보드 구성 방지
        FixInitialMatches();
    }

    // 제거 함수
    public void Clear()
    {
        if (null == mBoardData)
            return;

        // 블록 제거
        foreach (Block block in mBoardData.mBlocks)
        {
            if (null != block)
                Destroy(block.gameObject);
        }

        // 타일 제거
        foreach (BackGroundTile tile in mBoardData.mTiles)
        { 
            if (null != tile)
                tile?.DestroyTile();
        }
    }

    #endregion

    #region Block Operations

    // 블록 생성 함수
    private Block CreateBlock(int _x, int _y)
    {
        string coordName = "(" + _x + ", " + _y + ")";
        Vector2 pos = GetWorldPos(_x, _y);

        // 랜덤 블록 생성
        int BlockToUse = Random.Range(0, mBoardConfig.mBlockPrefabs.Length);
        GameObject blockObj = Instantiate(mBoardConfig.mBlockPrefabs[BlockToUse], pos, Quaternion.identity);
        blockObj.name = "Block_" + coordName;

        // Block 컴포넌트 설정
        Block block = blockObj.GetComponent<Block>();
        if (null != block)
        {
            block.mBlockData.Col = _x;
            block.mBlockData.Row = _y;
            block.mBlockData.BlockType = mBoardConfig.mBlockPrefabs[BlockToUse].name;
        }

        // 블럭 반환
        return block;
    }

    private void SwapBlock(in Vector2Int _startPos, in Vector2Int _targetPos)
    {
        // 범위 체크
        if (!IsValidPos(_startPos.x, _startPos.y) || !IsValidPos(_targetPos.x, _targetPos.y))
            return;

        Block curBlock = mBoardData?.mBlocks[_startPos.x, _startPos.y];
        Block targetBlock = mBoardData?.mBlocks[_targetPos.x, _targetPos.y];

        if (null == curBlock || null == targetBlock)
            return;

        // 배열에서 교환
        mBoardData.mBlocks[_startPos.x, _startPos.y] = targetBlock;
        mBoardData.mBlocks[_targetPos.x, _targetPos.y] = curBlock;

        // Block 컴포넌트의 좌표 업데이트
        UpdateBlockPos(curBlock, _targetPos.x, _targetPos.y);
        UpdateBlockPos(targetBlock, _startPos.x, _startPos.y);
    }

    // 타일과 블록 생성
    private void CreateTileAndBlock(int _x, int _y)
    {
        string coordName = "(" + _x + ", " + _y + ")";
        Vector2 pos = GetWorldPos(_x, _y);

        // 타일 생성
        GameObject tileObj = Instantiate(mBoardConfig.TilePrefab, pos, Quaternion.identity);
        tileObj.name = "Tile_" + coordName;
        mBoardData.mTiles[_x, _y] = tileObj.GetComponent<BackGroundTile>();

        // 블록 생성
        Block block = CreateBlock(_x, _y);
        if (null != block)
            mBoardData.mBlocks[_x, _y] = block;
    }

    // 블록 교환 시도
    public void TrySwapBlocks(in Vector2Int _startPos, in Vector2Int _targetPos)
    {
        // 처리 진행중이면 무시
        if (mIsProcessing)
            return;

        StartCoroutine(SwapSequence(_startPos, _targetPos));
    }

    private IEnumerator SwapSequence(Vector2Int _startPos, Vector2Int _targetPos)
    {
        mIsProcessing = true;

        // 실제 위치 바꾸기
        SwapBlock(_startPos, _targetPos);

        // 애니메이션 시간 만큼 대기
        yield return new WaitForSeconds(0.3f);

        // 매치 판정 시작
        List<List<Block>> allMatches = mBoardData.mBlocks.FindAllMatches(mBoardConfig.MinMatchCount);
        
        // 매치된게있다면 터트리기
        if (allMatches.Count > 0)
        {
            yield return StartCoroutine(PostMatchProcessRoutine());

        }
        // 매치가 없으면 원복
        else
        {
            SwapBlock(_targetPos, _startPos);
            yield return new WaitForSeconds(0.3f);
            mIsProcessing = false;
        }
    }

    // 기존 블록 아래로 내리기
    private IEnumerator DropDownBlocksRoutine()
    {
        for(int x = 0; x < mBoardData.mWidth; ++x)
        {
            for(int y = 0; y < mBoardData.mHeight; ++y)
            {
                // 현재 칸이 비어있다면 블록을 찾아 내림
                if(null == mBoardData.mBlocks[x, y])
                {
                    // 빈칸을 찾았으면 그 바로 위 부터 맨위까지 뒤져서 블록을 찾음
                    for(int nextY = y + 1; nextY < mBoardData.mHeight; ++nextY)
                    {
                        // 만약 그 좌표가 null이 아니라면 블럭을 아래로 이동
                        if(null != mBoardData.mBlocks[x, nextY])
                        {
                            Block block = mBoardData.mBlocks[x, nextY];
                            mBoardData.mBlocks[x, y] = block;
                            mBoardData.mBlocks[x, nextY] = null;

                            UpdateBlockPos(block, x, y);
                            break;

                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.3f);
    }

    // 맨위 빈칸에 새 블록 생성
    private IEnumerator RefillBoardRoutine()
    {
        for(int x = 0; x < mBoardData.mWidth; ++x)
        {
            for(int y = 0; y < mBoardData.mHeight; ++y)
            {
                if (null == mBoardData.mBlocks[x, y])
                {
                    // 새 블록 생성
                    Block newBlock = CreateBlock(x, y);

                    // 먼저 보드 위쪽으로 블럭 위치를 잡아줌
                    Vector2 startPos = GetWorldPos(x, mBoardData.mHeight);
                    newBlock.transform.position = startPos;

                    // 원래 위치로 위치 업데이트
                    mBoardData.mBlocks[x, y] = newBlock;
                    UpdateBlockPos(newBlock, x, y);

                }
            }
        }

        yield return null;
    }

    public Block GetBlockAtPos(in Vector2 _worldPos)
    {
        if (null == mBoardData)
            return null;

        // 월드 좌표를 그리드 좌표로 변환
        int col = Mathf.RoundToInt((_worldPos.x + mBoardData.mOffsetX) / 
            mBoardData.mTileSize.x);

        int row = Mathf.RoundToInt((_worldPos.y + mBoardData.mOffsetY) /
            mBoardData.mTileSize.y);

        Block blockObj = GetBlock(col, row);
        if (blockObj != null)
        {
            return blockObj;
        }

        return null;
    }

    // 블럭 가져오기
    public Block GetBlock(int x, int y)
    {
        if (null == mBoardData)
            return null;

        if (IsValidPos(x, y))
        {
            return mBoardData.mBlocks[x, y];
        }

        return null;
    }

    #endregion

    #region Match Logic

    // 초기 매치 방지 및 무브락 방지 함수
    private void FixInitialMatches()
    {
        // 1. 매치가 발견되는 동안 계속 반복
        while (true)
        {
            // MatchFinder를 통해 현재 매치된 리스트를 가져옴
            List<List<Block>> allMatches = mBoardData.mBlocks.FindAllMatches(mBoardConfig.MinMatchCount);

            // 매치가 하나도 없으면 루프 탈출! 성공!
            if (allMatches.Count <= 0)
                break;

            foreach (var matchGroup in allMatches)
            {
                // 각 매치 그룹의 첫 번째 블록만 색상을 바꿔도 매치는 깨진다
                ChangeRandomBlock(matchGroup[0]);
            }
        }

        // 2. 플레이어가 한번 이동 시 매치를 만들 수 있는지 체크
        bool hasPossibleMatch = mBoardData.mBlocks.HasPossibleMoves(mBoardConfig.MinMatchCount);

        // 만약 만들 수 없다면 셔플
        if (false == hasPossibleMatch)
        {
            StartCoroutine(ShuffleBoardRoutine());
        }

        GameManager.mInstance?.GameStart();
    }

    private IEnumerator ShuffleBoardRoutine()
    {
        mIsProcessing = true;

        int width = mBoardData.mWidth;
        int height = mBoardData.mHeight;

        bool hasMove = false;
        int Count = 0;

        while (!hasMove)
        {
            // 1. 블록들을 리스트에 수집
            List<Block> blockList = new List<Block>();
            foreach (Block block in mBoardData.mBlocks)
            {
                if (null != block)
                    blockList.Add(block);
            }

            // 2. 셔플
            for (int i = blockList.Count - 1; i > 0; --i)
            {
                int randomIndex = Random.Range(0, i + 1);

                Block temp = blockList[i];
                blockList[i] = blockList[randomIndex];
                blockList[randomIndex] = temp;
            }

            // 3. 셔플된 블록을 배열에 다시 배치
            int index = 0;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    mBoardData.mBlocks[x, y] = blockList[index++];
                    UpdateBlockPos(mBoardData.mBlocks[x, y], x, y);
                }
            }

            // 셔플 이후 검증

            // 1. 매치되는게 하나라도 있는지 확인
            bool IsMatch = mBoardData.mBlocks.FindAllMatches(mBoardConfig.MinMatchCount).Count > 0;
            // 2. 움직일 수 있는지 확인
            bool canMove = mBoardData.mBlocks.HasPossibleMoves(mBoardConfig.MinMatchCount);
            // 반영
            hasMove = !IsMatch && canMove;

            // 100번 넘게 섞었고 아직 못움직인다면 강제로 보정
            if (!hasMove && Count >= 100)
            {
                CreateFourMatch();
            }

            ++Count;

            yield return null;

        }

        yield return new WaitForSeconds(0.3f);
        mIsProcessing = false;
    }

    private void CreateFourMatch()
    {
        int randomBlockIndex = Random.Range(0, mBoardConfig.mBlockPrefabs.Length - 1);

        GameObject blockPrefab = mBoardConfig.mBlockPrefabs[randomBlockIndex];
        if (null == blockPrefab)
            return;

        string blocktype = blockPrefab.name;
        Sprite img = blockPrefab.GetComponent<SpriteRenderer>().sprite;

        for (int i = 0; i < mBoardConfig.MinMatchCount; ++i)
        {
            Block block = mBoardData.mBlocks[i, 0];
            block.mBlockData.BlockType = blocktype;
            block.GetComponent<SpriteRenderer>().sprite = img;
        }
    }

    private IEnumerator PostMatchProcessRoutine()
    {
        mIsProcessing = true;
        int comboCount = 0;

        while(mIsProcessing)
        {
            // 1. 매치 검사
            List<List<Block>> allMatches = mBoardData.mBlocks.FindAllMatches(mBoardConfig.MinMatchCount);

            // 더이상 매치가 없으면 종료
            if (0 >= allMatches.Count)
                break;

            // 2. 블록 제거
            HashSet<Block> blocksToDestroy = new HashSet<Block>();
            foreach(List<Block> blocks in allMatches)
            {
                foreach (Block block in blocks)
                    blocksToDestroy.Add(block);
            }

            // 점수 계산
            ++comboCount;
            int totalDestroyedCount = blocksToDestroy.Count;
            ScoreManager.mInstance?.AddScore(totalDestroyedCount, comboCount);
            // =========

            DestroyBlocks(blocksToDestroy);

            yield return new WaitForSeconds(0.2f);  

            // 3. 블록 낙하
            yield return StartCoroutine(DropDownBlocksRoutine());

            // 4. 빈칸 채우기
            yield return StartCoroutine(RefillBoardRoutine());    

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.3f);

        // 5. 터트린 후 매치가 생기는지 확인
        bool hasPossibleMatch = mBoardData.mBlocks.HasPossibleMoves(mBoardConfig.MinMatchCount);
        // 매치가 생기지 않으면 셔플
        if (false == hasPossibleMatch)
            yield return StartCoroutine(ShuffleBoardRoutine());

        mIsProcessing = false;
    }

    // 특정 블록의 타입을 랜덤하게 교체하는 헬퍼 함수
    private void ChangeRandomBlock(Block _targetBlock)
    {
        string currentType = _targetBlock.mBlockData.BlockType;
        int randomIndex;

        // 현재와 다른 타입이 나올 때까지 랜덤 돌리기
        do
        {
            randomIndex = Random.Range(0, mBoardConfig.mBlockPrefabs.Length);
        }
        while (mBoardConfig.mBlockPrefabs[randomIndex].name == currentType);

        // 데이터 갱신
        string blockName = mBoardConfig.mBlockPrefabs[randomIndex].name;
        _targetBlock.mBlockData.BlockType = blockName;

        // 시각적 갱신 (스프라이트 교체)
        SpriteRenderer sr = _targetBlock.GetComponent<SpriteRenderer>();
        SpriteRenderer newSr = mBoardConfig.mBlockPrefabs[randomIndex].GetComponent<SpriteRenderer>();
        if (sr != null && newSr != null)
        {
            sr.sprite = newSr.sprite;
        }

        mBoardData.mBlocks[_targetBlock.mBlockData.Col, _targetBlock.mBlockData.Row] = _targetBlock;
    }

    // 인자로 넘어온 블럭들 제거함수
    private void DestroyBlocks(HashSet<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (null == block)
                continue;

            mBoardData.mBlocks[block.mBlockData.Col, block.mBlockData.Row] = null;
            StartCoroutine(block.SMmoothDestroyBlock());
        }
    }

    #endregion

    #region Utility
    // 유효위치인지 체크
    private bool IsValidPos(int _col, int _row)
    {
        if (null == mBoardData)
            return false;

        return _col >= 0 && _col < mBoardData.mWidth 
                && _row >= 0 && _row < mBoardData.mHeight;
    }

    // 그리드 좌표를 월드 좌표로 변환
    private Vector2 GetWorldPos(int col, int row)
    {    
        float x = col * mBoardData.mTileSize.x - mBoardData.mOffsetX;
        float y = row * mBoardData.mTileSize.y - mBoardData.mOffsetY;
        return new Vector2(x, y);
    }

    // 블록 좌표 및 위치 업데이트
    private void UpdateBlockPos(Block _block, int _col, int _row)
    {
        _block.mBlockData.Col = _col;
        _block.mBlockData.Row = _row;
        _block.mBlockData.TargetPos = GetWorldPos(_col, _row);
    }

    // 카메라 조정
    private void AdjustCamera(in Vector2 _tileSize)
    {
        Camera mainCamera = Camera.main;
        if (null == mainCamera)
            return;

        // 보드 사이즈
        float boardWidth = mBoardData.mWidth * _tileSize.x;
        float boardHeight = mBoardData.mHeight * _tileSize.y;

        // 화면비
        float screenRatio = (float)Screen.width / Screen.height;
        float boardRatio = boardWidth / boardHeight;

        if(screenRatio >= boardRatio)
        {
            // 세로 기준
            mainCamera.orthographicSize = boardHeight / 2f + 0.5f;
        }
        else
        {
            // 가로 기준
            mainCamera.orthographicSize = boardWidth / screenRatio / 2f + 0.5f;
        }
    }

    #endregion
}
