using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScoreManager mInstance { get; private set; } = null;

    // 점수 관련 설정 -> 에디터에서 제어
    [SerializeField]
    private ScoreConfig mScoreConfig = null;

    // 현재 점수
    public int mCurScore = 0;

    // 점수 변화시 이벤트
    public System.Action<int> OnScoreChanged;
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
  
    // 점수 추가 함수
    public void AddScore(int _destroyedBlock, int _comboCount)
    {
        // 콤보 수가 Config에서 설정한 숫자보다 크거나 같을경우에 보너스 포인트 계산
        int bonusPoint = (_comboCount >= mScoreConfig.BonusComboCount) ?
            _comboCount * mScoreConfig.BonusPoint : 0;

        int scoreSum = (_destroyedBlock * mScoreConfig.BasePoint) + bonusPoint;

        mCurScore += scoreSum;

        OnScoreChanged?.Invoke(mCurScore);
    }

    // 점수 리셋
    public void ResetScore()
    {
        mCurScore = 0;
        OnScoreChanged?.Invoke(mCurScore);
    }
}
