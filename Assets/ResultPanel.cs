using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;

    public void ShowResultPanel(bool win)
    {
        gameObject.SetActive(true);           // 显示整个Panel
        winPanel.SetActive(win);              // 显示胜利内容/隐藏失败内容
        losePanel.SetActive(!win);            // 显示失败内容/隐藏胜利内容
    }

    public void Hide()
    {
        gameObject.SetActive(false);          // 需要时可以隐藏Panel
    }
}
