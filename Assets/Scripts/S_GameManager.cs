using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    [SerializeField] private S_InputManager inputManager;
    [SerializeField] private S_GridManager gridManager;
    [SerializeField] private S_ScoreManager scoreManager;

    private bool gameOver = false;

    private void Start()
    {
        inputManager.OnMove += HandleMove;
        inputManager.OnNewGame += StartNewGame;

        StartNewGame();
    }

    private void StartNewGame()
    {
        gameOver = false;
        scoreManager.ResetScore();
        gridManager.StartNewGame();
    }

    private void HandleMove(Direction direction)
    {
        if (gameOver)
            return;

        if (gridManager.TryMove(direction))
        {
            int mergeValue = gridManager.LastMergeValue;
            if (mergeValue > 0)
                scoreManager.AddScore(mergeValue);

            gridManager.SpawnTile();

            // Verificar derrota
            if (!gridManager.HasAvailableMoves())
            {
                scoreManager.ShowGameOver();
                gameOver = true;
            }
        }
    }

    private void OnDestroy()
    {
        inputManager.OnMove -= HandleMove;
        inputManager.OnNewGame -= StartNewGame;
    }
}