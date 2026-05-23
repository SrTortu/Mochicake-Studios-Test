using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private int score = 0;
    private int highScore = 0;

    public int Score => score;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreDisplay();
    }

    public void ResetScore()
    {
        score = 0;
        gameOverText.text = "";
        UpdateScoreDisplay();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreDisplay();
    }

    public void ShowGameOver()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        gameOverText.text = $"¡GAME OVER!\nPuntuación: {score}\nMejor: {highScore}";
    }

    public void ShowVictory()
    {
        gameOverText.text = "¡GANASTE!\n¡Alcanzaste 2048!";
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Puntuación: {score}";
    }
}