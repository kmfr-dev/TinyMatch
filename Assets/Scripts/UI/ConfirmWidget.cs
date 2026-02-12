using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmWidget : MonoBehaviour
{
    // 머리말 텍스트 ->에디터에서 설정
    [SerializeField]
    private TextMeshProUGUI mTitleText = null;
    
    // 본문 텍스트 -> 에디터에서 설정
    [SerializeField]
    private TextMeshProUGUI mContentText = null;

    // 버튼 -> 에디터에서 설정
    [SerializeField]
    private Button mAcceptBtn = null;
    [SerializeField]
    private Button mCancelBtn = null;

    // 머리말, 본문 텍스트 설정
    public void SetTitleText(string _titleText)
    {
        mTitleText?.SetText(_titleText);
    }
    public void SetContentText(string _contentText)
    {
        mContentText?.SetText(_contentText);
    }

    // 버튼 이벤트 설정
    public void SetAcceptBtnEvent(Action _action)
    {
        mAcceptBtn.onClick.RemoveAllListeners();
        mAcceptBtn.onClick.AddListener(() => _action());
    }
    public void SetCloseBtnEvent(Action _action)
    {
        mCancelBtn.onClick.RemoveAllListeners();
        mCancelBtn.onClick.AddListener(() => _action());
    }
}
