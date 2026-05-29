using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_ScoreManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;
    
    private bool _isGameOver = false;

    private int score = 0;
    private int highScore = 0;

    public int Score => score;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreDisplay();
        _isGameOver = false;
    }

    public void ResetScore()
    {
        score = 0;
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

        _isGameOver = true;
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Puntuación: {score}";
        recordText.text = $"Mejor: {highScore}"; 
        
    }
    
}