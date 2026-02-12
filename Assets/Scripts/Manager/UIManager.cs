using System;
using UnityEngine;
using UnityEngine.UI;

// 기본 확인창에 들어갈 문자열
public struct ConfirmContent
{
    // 머릿말 Warning, GameFinished 등
    public string Title;
    // 본문에 들어갈 텍스트
    public string Content;
    public ConfirmContent(string _title, string _content)
    {
        Title = _title;
        Content = _content;
    }
}

public class UIManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static UIManager mInstance { get; private set; } = null;

    [SerializeField]
    private PopupCanvas mPopupCanvas = null;
 
    // 확인창 위젯 - 에디터에서 설정
    [SerializeField]
    private ConfirmWidget mConfirmWidget = null;

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

    // 팝업 텍스트 및 버튼 눌렀을때 이벤트를 설정
    public void OpenConfrim(ConfirmContent _confirmContent, Action _acceptAction, Action _closeAction)
    {
        mPopupCanvas?.gameObject.SetActive(true);
        mPopupCanvas.SetActiveBackBlur(true);

        mConfirmWidget?.SetTitleText(_confirmContent.Title);
        mConfirmWidget?.SetContentText(_confirmContent.Content);
        mConfirmWidget?.SetAcceptBtnEvent(_acceptAction);
        mConfirmWidget?.SetCloseBtnEvent(_closeAction);  
    }

    public void CloseConfirm()
    {
        mPopupCanvas?.gameObject.SetActive(false);
        mPopupCanvas.SetActiveBackBlur(false);
    }
}
