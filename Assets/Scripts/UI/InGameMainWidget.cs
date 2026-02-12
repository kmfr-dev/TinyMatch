using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMainWidget : MonoBehaviour
{
    // 제한시간 텍스트 -> 에디터에서 설정
    [SerializeField]
    private TextMeshProUGUI mTimeText = null;

    // 점수 텍스트 -> 에디터에서 설정
    [SerializeField]
    private TextMeshProUGUI mScoreText = null;

    // 재시작 버튼 -> 에디터에서 설정
    [SerializeField]
    private Button mReplayBtn = null;

    // To타이틀 버튼 -> 에디터에서 설정
    [SerializeField]
    private Button mToTitleBtn = null;

    private void Start()
    {
        mReplayBtn.onClick.AddListener(OnClickedReplayBtn);
        mToTitleBtn.onClick.AddListener(OnClickedToTitleBtn);

        // 함수 바인딩
        GameManager gameInst = GameManager.mInstance;
        if (null != gameInst)
            gameInst.OnChangedTime += UpdateTimeText;

        ScoreManager scoreInst = ScoreManager.mInstance;
        if (null != scoreInst)
            scoreInst.OnScoreChanged += UpdateScoreText;
    }

    // 제한시간 텍스트 설정
    public void UpdateTimeText(float _newTime)
    {
        mTimeText.text = ((int)_newTime).ToString();
    }

    // 점수 텍스트 설정
    public void UpdateScoreText(int _newScore)
    {
        mScoreText.text = _newScore.ToString();
    }

    //  재시작/타이틀로 버튼 클릭 이벤트
    private void OnClickedReplayBtn()
    {
        GameManager.mInstance?.ResetGame();
    }
    private void OnClickedToTitleBtn()
    {
        GameManager.mInstance?.LoadToTitle();
    }
}
