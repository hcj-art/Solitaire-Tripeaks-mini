using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI addScoreText;

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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
