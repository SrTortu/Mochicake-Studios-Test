using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_GameManager : MonoBehaviour
{
    [SerializeField] private S_InputManager _inputManager;
    [SerializeField] private S_GridManager _gridManager;
    [SerializeField] private S_ScoreManager _scoreManager;
    
    [SerializeField] private GameObject _gameOverPanel;

    private bool _gameOver = false;

    private void Start()
    {
        _inputManager.OnMove += HandleMove;
        _inputManager.OnNewGame += StartNewGame;
        _gridManager.OnTileSpawned += HandleTileSpawned;
        StartNewGame();
    }

    public void StartNewGame()
    {
        _gameOver = false;
        _scoreManager.ResetScore();
        _gridManager.StartNewGame();

        if (_gameOverPanel != null)
        {
            _gameOverPanel.GetComponent<CanvasGroup>().alpha = 0f;
            _gameOverPanel.SetActive(false);
        }
    }

    private void HandleMove(Direction direction)
    {
        // Si el juego ya terminó, no hacer nada
        if (_gameOver)
            return;

        if (_gridManager.TryMove(direction))
        {
            int mergeValue = _gridManager.LastMergeValue;
            if (mergeValue > 0)
                _scoreManager.AddScore(mergeValue);
        }
        else
        {
            // Verificar GameOver si movimiento falla
            if (!_gridManager.HasAvailableMoves())
            {
                _scoreManager.UpdateRecord();
                _gameOver = true;
                ShowGameOverPanel();
            }
        }
    }

    private void HandleTileSpawned()
    {
        // Verificar GameOver después de spawnear un nuevo tile
        if (!_gameOver && !_gridManager.HasAvailableMoves())
        {
            _scoreManager.UpdateRecord();
            _gameOver = true;
            ShowGameOverPanel();
        }
    }

    private void ShowGameOverPanel()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            StartCoroutine(GameOverPanelAnim());
        }
    }
    
    private IEnumerator GameOverPanelAnim()
    {
        CanvasGroup canvasGroup = _gameOverPanel.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

}