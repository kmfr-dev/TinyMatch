using UnityEngine;

[System.Serializable]
public class ScoreConfig
{
    // 기본 블록 터트리면 주는 점수
    public int BasePoint = 0;
    // 보너스 콤보 갯수 ex) 몇 콤보부터 보너스 점수 줄건지
    public int BonusComboCount = 0;
    // 콤보 보너스 점수
    public int BonusPoint = 0;
}
