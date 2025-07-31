using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    // 保持为空，也可以加对CanvasGroup的暴露（便维护）
    [HideInInspector] public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowResultPanel(bool win)
    {
        gameObject.SetActive(true);           // 显示整个Panel
        canvasGroup.alpha = 1f;               // 确保CanvasGroup的透明度为1
        winPanel.SetActive(win);              // 显示胜利内容/隐藏失败内容
        losePanel.SetActive(!win);            // 显示失败内容/隐藏胜利内容
    }

    public void Hide()
    {
        gameObject.SetActive(false);          // 需要时可以隐藏Panel
    }
}
