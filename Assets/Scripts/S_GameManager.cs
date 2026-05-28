using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    [SerializeField] private S_InputManager _inputManager;
    [SerializeField] private S_GridManager _gridManager;
    [SerializeField] private S_ScoreManager _scoreManager;

    private bool _gameOver = false;

    private void Start()
    {
        _inputManager.OnMove += HandleMove;
        _inputManager.OnNewGame += StartNewGame;

        StartNewGame();
    }

    private void StartNewGame()
    {
        _gameOver = false;
        _scoreManager.ResetScore();
        _gridManager.StartNewGame();
    }

    private void HandleMove(Direction direction)
    {
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
                _scoreManager.ShowGameOver();
                _gameOver = true;
            }
        }
    }

}