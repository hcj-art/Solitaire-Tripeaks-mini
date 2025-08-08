using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //这是我的分支测试，为了体验代码拉取合并的过程，test以注释的形式去查看合并后的代码
    public CanvasGroup mainMenuPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup helpPanel;
    public CanvasGroup menuCanvas;
    public CanvasGroup gameCanvas;
    public GameManager gameManager;
    public float fadeDuration = 0.5f;
    void Start()
    {
        ShowPanel(mainMenuPanel);
    }

    public void OnStartClicked()
    {
        // 先让 menuCanvas 渐隐、隐藏，再让 gameCanvas 渐现
        menuCanvas.DOFade(0, fadeDuration).OnComplete(() =>
        {
            //menuCanvas.interactable = menuCanvas.blocksRaycasts = false;
            menuCanvas.gameObject.SetActive(false);

            gameCanvas.gameObject.SetActive(true);
            gameCanvas.alpha = 0;
            gameCanvas.interactable = gameCanvas.blocksRaycasts = true;
            gameCanvas.DOFade(1, fadeDuration);
        });
        gameManager.RestartGame(); // 重置游戏状态
    }
    public void OnSettingClicked()
    {
        SwitchPanel(mainMenuPanel, settingsPanel);
    }
    public void OnHelpClicked()
    {
        SwitchPanel(mainMenuPanel, helpPanel);
    }
    public void OnBackFromSettingClicked()
    {
        SwitchPanel(settingsPanel, mainMenuPanel);
    }
    public void OnBackFromHelpClicked()
    {
        SwitchPanel(helpPanel, mainMenuPanel);
    }
    void ShowPanel(CanvasGroup panel)
    {
        mainMenuPanel.gameObject.SetActive(panel == mainMenuPanel);
        settingsPanel.gameObject.SetActive(panel == settingsPanel);
        helpPanel.gameObject.SetActive(panel == helpPanel);

        mainMenuPanel.alpha = (panel == mainMenuPanel) ? 1 : 0;
        settingsPanel.alpha = (panel == settingsPanel) ? 1 : 0;
        helpPanel.alpha = (panel == helpPanel) ? 1 : 0;

        mainMenuPanel.interactable = mainMenuPanel.blocksRaycasts = (panel == mainMenuPanel);
        settingsPanel.interactable = settingsPanel.blocksRaycasts = (panel == settingsPanel);
        helpPanel.interactable = helpPanel.blocksRaycasts = (panel == helpPanel);

    }
    //这里的from和to在隐藏和显示时会自己变换alpha的数据，不像上面的OnStartClicked()自己变不回去
    //导致上面OnStartClicked()中点击start后，他的alpha自己变不回来
    //有必要写一个方法专门修复这个问题，或者以后只改active状态，不修改alpha
    void SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        //SwitchPanel之所以没问题，是因为它每次都开关配对，逻辑闭环。
        from.DOFade(0, fadeDuration).OnComplete(() =>
        {
            from.interactable = false;
            from.blocksRaycasts = false;
            from.gameObject.SetActive(false);

            to.gameObject.SetActive(true);
            to.alpha = 0;
            to.interactable = to.blocksRaycasts = true;
            to.DOFade(1, fadeDuration);
        });
    }
}
