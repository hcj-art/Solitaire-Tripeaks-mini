using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIView : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text remainCardText;
    public TMP_Text levelText;

    public void SetScore(int score)
    {
        scoreText.text = "分数： " + score.ToString();
    }
    public void SetRemainCard(int remain)
    {
        remainCardText.text = "剩余： " + remain.ToString();
    }
    public void SetLevel(int level)
    {
        levelText.text = "关卡： " + level.ToString();
    }
}
