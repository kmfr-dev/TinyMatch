using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMainWidget : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mTimeText = null;

    [SerializeField]
    private TextMeshProUGUI mScoreText = null;

    [SerializeField]
    private Button mReplayBtn = null;

    [SerializeField]
    private Button mToTitleBtn = null;

    private void Start()
    {
        mReplayBtn.onClick.AddListener(OnClickedReplayBtn);
        mToTitleBtn.onClick.AddListener(OnClickedToTitleBtn);

        GameManager gameInst = GameManager.mInstance;
        if (null != gameInst)
            gameInst.OnChangedTime += UpdateTimeText;

        ScoreManager scoreInst = ScoreManager.mInstance;
        if (null != scoreInst)
            scoreInst.OnScoreChanged += UpdateScoreText;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void UpdateTimeText(float _newTime)
    {
        mTimeText.text = ((int)_newTime).ToString();
    }

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
        SceneManager.LoadScene("Title");
    }
}
