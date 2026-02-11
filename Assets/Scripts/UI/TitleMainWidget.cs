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

    void OnClickedStartBtn()
    {
        SceneManager.LoadScene("InGame");
    }

    void OnClickedOptionBtn()
    {

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
