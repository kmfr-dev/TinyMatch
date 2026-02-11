using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager mInstance { get; private set; } = null;

    [SerializeField]
    private GameRuleConfig mGameConfig = null;

    // 제한시간 관련
    public float mCurTime { get; private set; } = 0f;
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
    private void Start()
    {
        
    }

    private void Update()
    {
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

    public void GameStart()
    {
        mIsCountDown = true;
        mCurTime = mGameConfig.TimeRemaining;
    }

    public void GameFinish()
    {
        int score = ScoreManager.mInstance.mCurScore;
        // Firebase DB에 저장시도
        FireBaseManager.mInstance.SaveScore(score);

        OnGameFinished?.Invoke();
    }

    public void ResetGame()
    {
        BoardManager.mInstance?.ResetBoard();
        ScoreManager.mInstance?.ResetScore();
    }
}
