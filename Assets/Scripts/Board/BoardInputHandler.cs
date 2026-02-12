using UnityEngine;
using UnityEngine.InputSystem;

public class BoardInputHandler : MonoBehaviour
{

    #region Variable
    // 보드
    public Board mBoard { private get; set; } = null;

    // 선택된 블록
    private Block mSelectedBlock = null;
    // 터치 관련 변수
    private Vector2 mFirstTouchPos = Vector2.zero;
    private Vector2 mFinalTouchPos = Vector2.zero;
    // 스와이프 감지 최소 거리
    private const float mMinSwipeDist = 0.5f;

    #endregion

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 지금 매치 처리중이면 return
        if (mBoard.mIsProcessing || false == GameManager.mInstance?.mIsCountDown)
            return;

        if (Pointer.current.press.wasPressedThisFrame)
        {
            OnMouseDownHandler();
        }
        else if (Pointer.current.press.wasReleasedThisFrame)
        {
            OnMouseUpHandler();
        }
    }

    // 마우스 다운 처리
    private void OnMouseDownHandler()
    {
        mFirstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 클릭한 위치의 블록 찾기
        mSelectedBlock = mBoard.GetBlockAtPos(mFirstTouchPos);
    }

    // 마우스 업 처리
    private void OnMouseUpHandler()
    {
        if (null == mSelectedBlock)
            return;

        mFinalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 스와이프 거리 체크
        float distance = Vector2.Distance(mFirstTouchPos, mFinalTouchPos);
        if (distance < mMinSwipeDist)
        {
            mSelectedBlock = null;
            return;
        }

        // 스와이프 방향 계산 및 블록 이동
        ProcessSwipe();

        mSelectedBlock = null;
    }

    // 스와이프 처리
    private void ProcessSwipe()
    {
        Vector2 swipeDir = mFinalTouchPos - mFirstTouchPos;
        float swipeAngle = Mathf.Atan2(swipeDir.y, swipeDir.x) * Mathf.Rad2Deg;

        int targetCol = mSelectedBlock.mBlockData.Col;
        int targetRow = mSelectedBlock.mBlockData.Row;

        // 각도에 따라 이동 방향 결정
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            // 오른쪽
            targetCol += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {
            // 위
            targetRow += 1;
        }
        else if (swipeAngle > 135 || swipeAngle <= -135)
        {
            // 왼쪽
            targetCol -= 1;
        }
        else if (swipeAngle > -135 && swipeAngle <= -45)
        {
            // 아래
            targetRow -= 1;
        }


        Vector2Int selectedBlockPos = new Vector2Int(mSelectedBlock.mBlockData.Col, mSelectedBlock.mBlockData.Row);
        Vector2Int targetPos = new Vector2Int(targetCol, targetRow);

        // 블록 교환 시도
        mBoard.TrySwapBlocks(selectedBlockPos, targetPos);
    }
}
