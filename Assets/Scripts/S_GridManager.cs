using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class S_GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _gridContainer;
    [SerializeField] private SO_TileData[] _tileData;
    [SerializeField] private S_PoolManager _poolManager;

    [Space(10), Header("Grid Settings")]
    [SerializeField] private int _gridSize = 4;
    [SerializeField] private float _cellSize = 100f;
    [SerializeField] private float _spacing = 10f;
    [SerializeField] private Color _cellBackgroundColor = new Color(0.6f, 0.55f, 0.5f);

    public int GridSize { get { return _gridSize; } }

    private S_Tile[] _tiles;
    private List<int> _freeIndexes;
    private int _lastMergeValue;
    private bool[] _mergedThisTurnCache;

    public int LastMergeValue { get { return _lastMergeValue; } }

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        int total = _gridSize * _gridSize;
        _tiles = new S_Tile[total];
        _freeIndexes = new List<int>(total);
        _mergedThisTurnCache = new bool[total];

        CreateCellBackgrounds(total);

        for (int i = 0; i < total; i++)
            _freeIndexes.Add(i);
    }

    private void CreateCellBackgrounds(int total)
    {
        for (int i = 0; i < total; i++)
        {
            GameObject cellBg = new GameObject($"Cell_{i}");
            cellBg.transform.SetParent(_gridContainer, false);

            RectTransform rect = cellBg.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_cellSize, _cellSize);
            rect.anchoredPosition = GetTilePosition(i);

            Image image = cellBg.AddComponent<Image>();
            image.color = _cellBackgroundColor;
        }
    }

    public void StartNewGame()
    {
        ClearGrid();
        SpawnTile();
        SpawnTile();
    }

    private void ClearGrid()
    {
        // Primero devolver todos los tiles al pool
        for (int i = _gridContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _gridContainer.GetChild(i);
            if (child.GetComponent<S_Tile>() != null)
            {
                _poolManager.ReturnToPool(child.gameObject);
            }
        }

        // Luego destruir todos los hijos restantes (celdas de fondo)
        while (_gridContainer.childCount > 0)
        {
            Transform child = _gridContainer.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        InitializeGrid();
    }

    public bool TryMove(Direction direction)
    {
        bool moved = Move(direction);

        if (moved)
            StartCoroutine(SpawnAfterAnimation());

        return moved;
    }

    private IEnumerator SpawnAfterAnimation()
    {
        yield return new WaitForSeconds(0.1f); // Esperar a que terminen las animaciones de movimiento
        SpawnTile();
    }

    private bool Move(Direction direction)
    {
        bool moved = false;
        Array.Clear(_mergedThisTurnCache, 0, _mergedThisTurnCache.Length);
        _lastMergeValue = 0;

        // Primero: mover todos los tiles
        moved = MoveTiles(direction);

        // Segundo: hacer los merges
        bool merged = MergeTiles(direction, _mergedThisTurnCache);

        // Tercero: mover nuevamente los tiles resultantes de los merges
        if (merged)
        {
            moved = MoveTiles(direction) || moved;
        }

        if (moved || merged)
        {
            UpdateFreeIndexes();
            UpdateTiles();
        }

        return moved || merged;
    }

    private bool MoveTiles(Direction direction)
    {
        bool moved = false;
        int startIndex, endIndex, step;

        GetLoopParameters(direction, out startIndex, out endIndex, out step);

        for (int i = startIndex; i != endIndex; i += step)
        {
            if (_tiles[i] != null)
            {
                int targetIndex = i;
                int nextIndex = GetNeighborIndex(targetIndex, direction);

                // Encontrar la posición más lejana posible
                while (nextIndex >= 0 && nextIndex < _tiles.Length && _tiles[nextIndex] == null)
                {
                    targetIndex = nextIndex;
                    nextIndex = GetNeighborIndex(targetIndex, direction);
                }

                if (targetIndex != i)
                {
                    _tiles[targetIndex] = _tiles[i];
                    _tiles[i] = null;
                    moved = true;
                }
            }
        }

        return moved;
    }

    private bool MergeTiles(Direction direction, bool[] _mergedThisTurnCache)
    {
        bool merged = false;
        int startIndex, endIndex, step;
        GetLoopParameters(direction, out startIndex, out endIndex, out step);

        for (int i = startIndex; i != endIndex; i += step)
        {
            if (_tiles[i] != null)
            {
                int neighborIndex = GetNeighborIndex(i, direction);
                if (neighborIndex >= 0 && neighborIndex < _tiles.Length &&
                    _tiles[neighborIndex] != null &&
                    _tiles[neighborIndex].GetValue() == _tiles[i].GetValue() &&
                    !_mergedThisTurnCache[neighborIndex])
                {
                    SO_TileData newData = _tileData[_tiles[i].GetIDataIndex() + 1];
                    _tiles[neighborIndex].UpgradeData(newData);
                    _lastMergeValue += newData.value;
                    _poolManager.ReturnToPool(_tiles[i].gameObject);
                    _tiles[i] = null;
                    _mergedThisTurnCache[neighborIndex] = true;
                    merged = true;
                }
            }
        }

        return merged;
    }

    private void GetLoopParameters(Direction direction, out int startIndex, out int endIndex, out int step)
    {
        switch (direction)
        {
            case Direction.Up:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
            case Direction.Down:
                startIndex = _tiles.Length - 1;
                endIndex = -1;
                step = -1;
                break;
            case Direction.Left:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
            case Direction.Right:
                startIndex = _tiles.Length - 1;
                endIndex = -1;
                step = -1;
                break;
            default:
                startIndex = 0;
                endIndex = _tiles.Length;
                step = 1;
                break;
        }
    }

    private int GetNeighborIndex(int index, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                if (index - _gridSize >= 0)
                    return index - _gridSize;
                break;
            case Direction.Down:
                if (index + _gridSize < _tiles.Length)
                    return index + _gridSize;
                break;
            case Direction.Left:
                if (index % _gridSize != 0)
                    return index - 1;
                break;
            case Direction.Right:
                if ((index + 1) % _gridSize != 0)
                    return index + 1;
                break;
        }
        return -1;
    }

    // ====================== SPAWN ======================

    private void UpdateFreeIndexes()
    {
        _freeIndexes.Clear();
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null)
            {
                _freeIndexes.Add(i);
            }
        }
    }

    public void SpawnTile()
    {
        if (_freeIndexes.Count == 0) return;

        int randomIndex = Random.Range(0, _freeIndexes.Count);
        int tileIndex = _freeIndexes[randomIndex];
        int dataIndex = 0;
        _freeIndexes.RemoveAt(randomIndex);

        if (Random.value > 0.9f)
        {
            dataIndex = 1;
        }

        S_Tile newTile = _poolManager.SpawnFromPool(_gridContainer);
        newTile.transform.SetAsLastSibling(); // Asegurar que esté encima de las celdas de fondo
        RectTransform tileRect = newTile.transform as RectTransform;
        tileRect.sizeDelta = new Vector2(_cellSize, _cellSize);
        tileRect.anchoredPosition = GetTilePosition(tileIndex);
        newTile.Init(_tileData[dataIndex], dataIndex);
        _tiles[tileIndex] = newTile;

        newTile.AnimateSpawn();
    }

    // ====================== VISUALES ======================

    private Vector2 GetTilePosition(int index)
    {
        int row = index / _gridSize;
        int col = index % _gridSize;

        float totalGridSize = _gridSize * _cellSize + (_gridSize - 1) * _spacing;

        float x = col * (_cellSize + _spacing) - totalGridSize / 2 + _cellSize / 2;
        float y = -row * (_cellSize + _spacing) + totalGridSize / 2 - _cellSize / 2;

        return new Vector2(x, y);
    }

    private void UpdateTiles()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] != null && _tiles[i].gameObject.activeInHierarchy)
            {
                _tiles[i].AnimateToPosition(GetTilePosition(i));
            }
        }
    }

    private void OnValidate()
    {
        if (_gridContainer != null)
        {
            float total = _gridSize * _cellSize + (_gridSize - 1) * _spacing;
            float margin = 4f;
            _gridContainer.sizeDelta = new Vector2(total + margin, total + margin);
        }
    }

    public bool HasAvailableMoves()
    {
        // Si hay espacios vacíos, hay movimientos disponibles
        if (_freeIndexes.Count > 0)
            return true;

        // Verificar si hay merges posibles
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] == null) continue;

            int currentValue = _tiles[i].GetValue();

            // Verificar vecino arriba
            int upIndex = GetNeighborIndex(i, Direction.Up);
            if (upIndex >= 0 && _tiles[upIndex] != null && _tiles[upIndex].GetValue() == currentValue)
                return true;

            // Verificar vecino abajo
            int downIndex = GetNeighborIndex(i, Direction.Down);
            if (downIndex >= 0 && _tiles[downIndex] != null && _tiles[downIndex].GetValue() == currentValue)
                return true;

            // Verificar vecino izquierda
            int leftIndex = GetNeighborIndex(i, Direction.Left);
            if (leftIndex >= 0 && _tiles[leftIndex] != null && _tiles[leftIndex].GetValue() == currentValue)
                return true;

            // Verificar vecino derecha
            int rightIndex = GetNeighborIndex(i, Direction.Right);
            if (rightIndex >= 0 && _tiles[rightIndex] != null && _tiles[rightIndex].GetValue() == currentValue)
                return true;
        }

        return false;
    }
}