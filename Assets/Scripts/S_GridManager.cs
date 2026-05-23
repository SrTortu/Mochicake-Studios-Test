using System.Collections.Generic;
using UnityEngine;

public class S_GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private S_Tile tilePrefab;

    [SerializeField] private int gridSize = 4;
    [SerializeField] private float cellSize = 100f;
    [SerializeField] private float spacing = 10f;

    private S_Tile[,] tiles;
    private int lastMergeValue = 0;

    public int LastMergeValue => lastMergeValue;

    private void Awake()
    {
        InitializeGrid();
    }

    private void OnValidate()
    {
        if (gridContainer != null)
        {
            float total = gridSize * cellSize + (gridSize - 1) * spacing;
            gridContainer.sizeDelta = new Vector2(total, total);
        }
    }

    private void InitializeGrid()
    {
        tiles = new S_Tile[gridSize, gridSize];
    }

    public void StartNewGame()
    {
        ClearGrid();
        SpawnTile();
        SpawnTile();
    }

    public bool TryMove(Direction direction)
    {
        lastMergeValue = 0;
        bool moved = false;

        Debug.Log($"TryMove: {direction}");
        switch (direction)
        {
            case Direction.Left: moved = MoveLeft(); break;
            case Direction.Right: moved = MoveRight(); break;
            case Direction.Up: moved = MoveUp(); break;
            case Direction.Down: moved = MoveDown(); break;
        }
        Debug.Log($"Move result: {moved}");
        return moved;
    }

    private bool MoveLeft()
    {
        bool moved = false;
        for (int y = 0; y < gridSize; y++)
        {
            List<S_Tile> row = new List<S_Tile>();
            for (int x = 0; x < gridSize; x++)
            {
                if (tiles[x, y] != null)
                {
                    row.Add(tiles[x, y]);
                    tiles[x, y] = null;
                }
            }

            List<S_Tile> merged = new List<S_Tile>();
            for (int i = 0; i < row.Count; i++)
            {
                if (i + 1 < row.Count && row[i].Value == row[i + 1].Value)
                {
                    int newValue = row[i].Value * 2;
                    row[i].Init(newValue);
                    lastMergeValue = newValue;
                    Destroy(row[i + 1].gameObject);
                    merged.Add(row[i]);
                    i++;
                    moved = true;
                }
                else
                {
                    merged.Add(row[i]);
                }
            }

            for (int i = 0; i < merged.Count; i++)
            {
                int targetX = i;
                tiles[targetX, y] = merged[i];
                RectTransform rt = merged[i].GetComponent<RectTransform>();
                Vector2 targetPos = GetCellAnchoredPosition(targetX, y);
                if (rt.anchoredPosition != targetPos)
                {
                    rt.anchoredPosition = targetPos;
                    moved = true;
                }
            }
        }
        return moved;
    }

    private bool MoveRight()
    {
        bool moved = false;
        for (int y = 0; y < gridSize; y++)
        {
            List<S_Tile> row = new List<S_Tile>();
            for (int x = gridSize - 1; x >= 0; x--)
            {
                if (tiles[x, y] != null)
                {
                    row.Add(tiles[x, y]);
                    tiles[x, y] = null;
                }
            }

            List<S_Tile> merged = new List<S_Tile>();
            for (int i = 0; i < row.Count; i++)
            {
                if (i + 1 < row.Count && row[i].Value == row[i + 1].Value)
                {
                    int newValue = row[i].Value * 2;
                    row[i].Init(newValue);
                    lastMergeValue = newValue;
                    Destroy(row[i + 1].gameObject);
                    merged.Add(row[i]);
                    i++;
                    moved = true;
                }
                else
                {
                    merged.Add(row[i]);
                }
            }

            for (int i = 0; i < merged.Count; i++)
            {
                int targetX = gridSize - 1 - i;
                tiles[targetX, y] = merged[i];
                RectTransform rt = merged[i].GetComponent<RectTransform>();
                Vector2 targetPos = GetCellAnchoredPosition(targetX, y);
                if (rt.anchoredPosition != targetPos)
                {
                    rt.anchoredPosition = targetPos;
                    moved = true;
                }
            }
        }
        return moved;
    }

    private bool MoveUp()
    {
        bool moved = false;
        for (int x = 0; x < gridSize; x++)
        {
            List<S_Tile> col = new List<S_Tile>();
            for (int y = 0; y < gridSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    col.Add(tiles[x, y]);
                    tiles[x, y] = null;
                }
            }

            List<S_Tile> merged = new List<S_Tile>();
            for (int i = 0; i < col.Count; i++)
            {
                if (i + 1 < col.Count && col[i].Value == col[i + 1].Value)
                {
                    int newValue = col[i].Value * 2;
                    col[i].Init(newValue);
                    lastMergeValue = newValue;
                    Destroy(col[i + 1].gameObject);
                    merged.Add(col[i]);
                    i++;
                    moved = true;
                }
                else
                {
                    merged.Add(col[i]);
                }
            }

            for (int i = 0; i < merged.Count; i++)
            {
                int targetY = i;
                tiles[x, targetY] = merged[i];
                RectTransform rt = merged[i].GetComponent<RectTransform>();
                Vector2 targetPos = GetCellAnchoredPosition(x, targetY);
                if (rt.anchoredPosition != targetPos)
                {
                    rt.anchoredPosition = targetPos;
                    moved = true;
                }
            }
        }
        return moved;
    }

    private bool MoveDown()
    {
        bool moved = false;
        for (int x = 0; x < gridSize; x++)
        {
            List<S_Tile> col = new List<S_Tile>();
            for (int y = gridSize - 1; y >= 0; y--)
            {
                if (tiles[x, y] != null)
                {
                    col.Add(tiles[x, y]);
                    tiles[x, y] = null;
                }
            }

            List<S_Tile> merged = new List<S_Tile>();
            for (int i = 0; i < col.Count; i++)
            {
                if (i + 1 < col.Count && col[i].Value == col[i + 1].Value)
                {
                    int newValue = col[i].Value * 2;
                    col[i].Init(newValue);
                    lastMergeValue = newValue;
                    Destroy(col[i + 1].gameObject);
                    merged.Add(col[i]);
                    i++;
                    moved = true;
                }
                else
                {
                    merged.Add(col[i]);
                }
            }

            for (int i = 0; i < merged.Count; i++)
            {
                int targetY = gridSize - 1 - i;
                tiles[x, targetY] = merged[i];
                RectTransform rt = merged[i].GetComponent<RectTransform>();
                Vector2 targetPos = GetCellAnchoredPosition(x, targetY);
                if (rt.anchoredPosition != targetPos)
                {
                    rt.anchoredPosition = targetPos;
                    moved = true;
                }
            }
        }
        return moved;
    }

    public void SpawnTile()
    {
        if (tiles == null) InitializeGrid();

        List<Vector2Int> emptySpaces = new List<Vector2Int>();
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (tiles[x, y] == null)
                    emptySpaces.Add(new Vector2Int(x, y));

        if (emptySpaces.Count == 0) return;

        Vector2Int spawnPos = emptySpaces[Random.Range(0, emptySpaces.Count)];
        int value = Random.value > 0.9f ? 4 : 2;

        S_Tile newTile = Instantiate(tilePrefab);
        RectTransform tileRT = newTile.GetComponent<RectTransform>();
        tileRT.SetParent(gridContainer, false);
        tileRT.sizeDelta = new Vector2(cellSize, cellSize);
        tileRT.anchoredPosition = GetCellAnchoredPosition(spawnPos.x, spawnPos.y);
        newTile.Init(value);
        tiles[spawnPos.x, spawnPos.y] = newTile;
    }

    public bool HasAvailableMoves()
    {
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (tiles[x, y] == null)
                    return true;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (tiles[x, y] == null) continue;
                int current = tiles[x, y].Value;
                if (x < gridSize - 1 && tiles[x + 1, y] != null && current == tiles[x + 1, y].Value) return true;
                if (y < gridSize - 1 && tiles[x, y + 1] != null && current == tiles[x, y + 1].Value) return true;
            }
        }
        return false;
    }

    public bool HasTileValue(int value)
    {
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (tiles[x, y] != null && tiles[x, y].Value == value) return true;
        return false;
    }

    private void ClearGrid()
    {
        if (gridContainer != null)
        {
            for (int i = gridContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(gridContainer.GetChild(i).gameObject);
            }
        }
        InitializeGrid();
    }

    private Vector2 GetCellAnchoredPosition(int x, int y)
    {
        if (gridContainer == null)
        {
            float total = gridSize * cellSize + (gridSize - 1) * spacing;
            float startX = -total / 2f + cellSize / 2f;
            float startY = total / 2f - cellSize / 2f;
            float posX = startX + x * (cellSize + spacing);
            float posY = startY - y * (cellSize + spacing);
            return new Vector2(posX, posY);
        }

        // Use actual rect size to be robust with anchors
        float totalWidth = gridContainer.rect.width;
        float totalHeight = gridContainer.rect.height;
        float startX2 = -totalWidth / 2f + cellSize / 2f;
        float startY2 = totalHeight / 2f - cellSize / 2f;
        float posX2 = startX2 + x * (cellSize + spacing);
        float posY2 = startY2 - y * (cellSize + spacing);
        return new Vector2(posX2, posY2);
    }

    [ContextMenu("LogGridState")]
    public void LogGridState()
    {
        if (tiles == null) { Debug.Log("tiles array is null"); return; }
        string s = "Grid state:\n";
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                s += tiles[x, y] != null ? tiles[x, y].Value.ToString().PadLeft(4) : "   .";
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}