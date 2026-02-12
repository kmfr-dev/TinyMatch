using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager mInstance { get; private set; } = null;

    // 게임 룰 -> 에디터에서 제어
    [SerializeField]
    private GameRuleConfig mGameConfig = null;

    // 제한시간 관련
    public float mCurTime { get; private set; } = 0f;
    // 현재 카운트다운이 진행중인지
    public bool mIsCountDown { get; private set; } = false;

    // 제한시간 이벤트
    public Action<float> OnChangedTime;
    // 게임 종료 이벤트
    public Action OnGameFinished; 

    private void Awake()
    {
        // 인스턴스가 없다면 
        if (null == mInstance)
        {
            mInstance = this;
        }
        // 있다면 제거
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 제한시간 타이머
        if(mIsCountDown && mCurTime > 0f)
        {
            mCurTime -= Time.deltaTime;

            Debug.Log(mCurTime);

            // 제한시간이 다됐으면 게임 종료 처리
            if(mCurTime <= 0f)
            {
                mIsCountDown = false;
                mCurTime = 0f;

                GameFinish();
            }

            OnChangedTime.Invoke(mCurTime);
        }
    }

    // 제한시간 설정 및 카운트다운 시작
    public void GameStart()
    {
        mIsCountDown = true;
        mCurTime = mGameConfig.TimeRemaining;
    }

    // 게임 종료 후 팝업 활성화
    public void GameFinish()
    {
        int score = ScoreManager.mInstance.mCurScore;

        string titleText = "Game Finished!";
        string contentText = $"당신의 점수는 {score}점 입니다.\n 재시작 or 타이틀로";

        UIManager.mInstance?.OpenConfrim(new ConfirmContent(titleText, contentText), () =>
            {
                ResetGame();
                UIManager.mInstance?.CloseConfirm();
            },
            LoadToTitle
            );
    }

    // 보드판, 점수판 리셋
    public void ResetGame()
    {
        BoardManager.mInstance?.ResetBoard();
        ScoreManager.mInstance?.ResetScore();
    }

    // 타이틀로 씬전환
    public void LoadToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
