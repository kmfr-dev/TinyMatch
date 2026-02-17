using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BoardManager mInstance { get; private set; } = null;

    // 매치여부 확인 기능을 담당하는 객체
    public MatchFinder mMatchFinder { get; private set; } = null;

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
        Init();
    }

    // 보드 초기화 함수
    private void Init()
    {
        if (null == mBoardObj)
        {
            // 매치 파인더 초기화
            mMatchFinder = new MatchFinder();
            mMatchFinder.Init(mBoardConfig);

            // 보드 초기화
            mBoardObj = new GameObject("Board");
            Board BoardComp = mBoardObj.AddComponent<Board>();
            BoardComp?.Init(mBoardConfig);

            // 입력 설정
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
        }
    }

    // 보드 리셋 함수
    public void ResetBoard()
    {
        DestroyBoard();
        Init();
    }
}
