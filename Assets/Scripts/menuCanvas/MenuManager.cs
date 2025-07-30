using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public CanvasGroup mainMenuPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup helpPanel;
    public CanvasGroup menuCanvas;
    public CanvasGroup gameCanvas;
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
            menuCanvas.interactable = menuCanvas.blocksRaycasts = false;
            menuCanvas.gameObject.SetActive(false);

            gameCanvas.gameObject.SetActive(true);
            gameCanvas.alpha = 0;
            gameCanvas.interactable = gameCanvas.blocksRaycasts = true;
            gameCanvas.DOFade(1, fadeDuration);
        });
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
    void SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
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
