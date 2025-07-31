using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI addScoreText;
    public ResultPanel resultPanel;//在ResultPanel.cs中也获取了CanvasGroup
    public GameSceneController gameController;

    [Header("UI面板CanvasGroup")]
    public CanvasGroup mainMenuPanel;
    public CanvasGroup gamePanel;
    public GameObject menuCanvas;
    public float fadeTime = 0.5f;

    int currentCombo = 0;
    int maxComboScore = 10;
    int totalScore = 0;

    void Awake()
    {
        Instance = this;
    }

    public void OnTargetPileMatch()
    {
        currentCombo = Mathf.Min(currentCombo + 1, maxComboScore);
        int addScore = currentCombo;
        totalScore += addScore;
        UpdateScoreUI(totalScore, addScore, currentCombo);
    }

    public void OnCardPileClick()
    {
        currentCombo = 0;
        UpdateScoreUI(totalScore, 0, currentCombo);
    }

    void UpdateScoreUI(int total, int lastAdd, int combo)
    {
        if (scoreText != null)
            scoreText.text = $"      : {total}";
        if (comboText != null)
            comboText.text = $"         {combo} !";
        if (addScoreText != null)
            addScoreText.text = lastAdd > 0 ? $"+{lastAdd}" : "";
    }

    //返回主菜单的逻辑
    public void BackToMainMenu()
    {
        if (resultPanel != null && resultPanel.gameObject.activeSelf)
        {
            resultPanel.canvasGroup.DOFade(0, fadeTime).OnComplete(() => resultPanel.gameObject.SetActive(false));
        }
        if (gamePanel != null && gamePanel.gameObject.activeSelf)
        {
            gamePanel.DOFade(0, fadeTime).OnComplete(() => gamePanel.gameObject.SetActive(false));
        }
        if (mainMenuPanel != null)
        {
            // Canvas物体必须激活
            menuCanvas.SetActive(true);
            // menuCanvas的CanvasGroup必须是透明状态1
            CanvasGroup cgCanvas = menuCanvas.GetComponent<CanvasGroup>();
            if (cgCanvas != null) cgCanvas.alpha = 1f; //状态也是1

            // mainMenuPanel也要激活
            mainMenuPanel.gameObject.SetActive(true);
            mainMenuPanel.alpha = 0f;
            mainMenuPanel.DOFade(1, fadeTime);
        }
        //RestartGame();在主菜单点击开始时才重新实例化
    }

    public void RestartGame()
    {
        //重置胜负判断
        if (GameEndManager.Instance != null)
            GameEndManager.Instance.ResetGameEnd();

        //隐藏结果面板
        if (resultPanel != null)
            resultPanel.Hide();

        //重置分数
        totalScore = 0;
        currentCombo = 0;
        UpdateScoreUI(0, 0, 0);

        //重置游戏场景
        if (gameController != null)
            gameController.ResetCardGameRoot();
    }
}
