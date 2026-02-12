using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMainWidget : MonoBehaviour
{
    // 타이틀
    // 시작, 나가기 버튼
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _exitButton;

    void Start()
    {
        // 함수 바인딩
        _startButton.onClick.AddListener(OnClickedStartBtn);
        _exitButton.onClick.AddListener(OnClickedQuitBtn);
    }

    // 버튼 이벤트 함수
    void OnClickedStartBtn()
    {
        SceneManager.LoadScene("InGame");
    }

    void OnClickedQuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();   
#endif
    }
}
