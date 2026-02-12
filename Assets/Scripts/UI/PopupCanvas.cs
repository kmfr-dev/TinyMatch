using UnityEngine;
using UnityEngine.UI;


public class PopupCanvas : MonoBehaviour
{
    // 화면 전체를 깔고있는 블러 이미지
    [SerializeField]
    private Image mBackBlur = null;

    private void Awake()
    {
        // 비활성화 처리
        gameObject.SetActive(false);
        mBackBlur.gameObject.SetActive(false);
    }

    // 블러이미지 비활/활성화 함수
    public void SetActiveBackBlur(bool _active)
    {
        mBackBlur.gameObject.SetActive(_active);
    }
}
