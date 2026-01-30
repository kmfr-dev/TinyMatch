using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMainWidget : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

    void Start()
    {
        _startButton.onClick.AddListener(OnClickedStartBtn);
        _optionButton.onClick.AddListener(OnClickedOptionBtn);
        _exitButton.onClick.AddListener(OnClickedQuitBtn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClickedStartBtn()
    {
        SceneManager.LoadScene("InGame");
        Debug.Log("게임 시작 버튼 클릭");
    }

    void OnClickedOptionBtn()
    {
        Debug.Log("옵션 버튼 클릭!");
    }

    void OnClickedQuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();   
#endif
        Debug.Log("나가기 버튼 클릭!");
    }
}
