using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BoardManager mInstance { get; private set; }

    // Board
    private GameObject mBoardObj = null;
    // Input 
    private BoardInputHandler mInput = null;

    // 보드 설정 -> 에디터에서 제어
    [SerializeField] 
    private BoardConfig mBoardConfig = null;

    private void Awake()
    {
        // 인스턴스가 없다면 
        if(null == mInstance)
        {
            mInstance = this;
        }
        // 있다면 제거
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitBoard();
    }

    // 보드 초기화 함수
    private void InitBoard()
    {
        if (null == mBoardObj)
        {
            mBoardObj = new GameObject("Board");
            Board BoardComp = mBoardObj.AddComponent<Board>();
            BoardComp?.Init(mBoardConfig);

            mInput = mBoardObj.AddComponent<BoardInputHandler>();
            mInput.mBoard = BoardComp;
        }
        else
        {
            Debug.Log("보드가 이미 존재합니다!");
        }
    }

    // 보드 제거 함수
    public void DestroyBoard()
    {
       // 보드가 있을 때만 제거
       if(null != mBoardObj)
        {
            mBoardObj.GetComponent<Board>()?.Clear();
            Destroy(mBoardObj);
            mBoardObj = null;
        }
    }

    // 보드 리셋 함수
    public void ResetBoard()
    {
        DestroyBoard();
        InitBoard();
    }
}
