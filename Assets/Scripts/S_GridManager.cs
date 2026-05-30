using System;
using UnityEngine;

public class S_GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_GridUI _gridUI;
    [SerializeField] private S_GridLogic _gridLogic;

    public int GridSize => _gridUI.GridSize;
    public int LastMergeValue => _gridLogic.LastMergeValue;
    public event Action OnTileSpawned
    {
        add => _gridLogic.OnTileSpawned += value;
        remove => _gridLogic.OnTileSpawned -= value;
    }
    public event Action OnTileMerge
    {
        add => _gridLogic.OnTileMerge += value;
        remove => _gridLogic.OnTileMerge -= value;
    }

    public void StartNewGame()
    {
        _gridLogic.StartNewGame();
    }

    public bool TryMove(Direction direction)
    {
        return _gridLogic.TryMove(direction);
    }

    public bool HasAvailableMoves()
    {
        return _gridLogic.HasAvailableMoves();
    }
}
