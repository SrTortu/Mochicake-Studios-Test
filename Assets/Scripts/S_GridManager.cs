using System.Collections.Generic;
using UnityEngine;

public class S_GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private S_Tile tilePrefab;

    [SerializeField] private int gridSize = 4;
    [SerializeField] private float cellSize = 100f;
    [SerializeField] private float spacing = 10f;

    private S_Tile[] tiles;
    private List<int> emptyIndices;

    public int LastMergeValue { get; private set; }

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
        int totalCells = gridSize * gridSize;
        tiles = new S_Tile[totalCells];
        emptyIndices = new List<int>(totalCells);

        for (int i = 0; i < totalCells; i++)
        {
            emptyIndices.Add(i);
        }
    }

    public void StartNewGame()
    {
        ClearGrid();
        SpawnTile();
        SpawnTile();
    }

    public bool TryMove(Direction direction)
    {
        LastMergeValue = 0;
        bool moved = MoveInDirection(direction);

        if (moved)
        {
            UpdateEmptyIndices();
        }

        return moved;
    }

    private bool MoveInDirection(Direction direction)
    {
        bool moved = false;

        for (int line = 0; line < gridSize; line++)
        {
            ProcessLineInDirection(line, direction, ref moved);
        }

        return moved;
    }

    private void ProcessLineInDirection(int line, Direction direction, ref bool moved)
    {
        // Extraer las fichas de la línea según la dirección
        List<S_Tile> tilesInLine = ExtractTilesInDirection(line, direction);

        // Hacer merges
        List<S_Tile> mergedTiles = MergeTiles(tilesInLine, ref moved);

        // Colocar las fichas en su nueva posición
        PlaceTilesInDirection(line, direction, mergedTiles, ref moved);
    }

    private List<S_Tile> ExtractTilesInDirection(int line, Direction direction)
    {
        List<S_Tile> tilesInLine = new List<S_Tile>(gridSize);

        for (int pos = 0; pos < gridSize; pos++)
        {
            int index = GetIndexInDirection(line, pos, direction);

            if (tiles[index] != null)
            {
                tilesInLine.Add(tiles[index]);
            }
        }

        return tilesInLine;
    }

    private void PlaceTilesInDirection(int line, Direction direction, List<S_Tile> mergedTiles, ref bool moved)
    {
        for (int pos = 0; pos < gridSize; pos++)
        {
            int index = GetIndexInDirection(line, pos, direction);
            S_Tile newTile = (pos < mergedTiles.Count) ? mergedTiles[pos] : null;

            if (newTile == null)
            {
                if (tiles[index] != null)
                {
                    Destroy(tiles[index].gameObject);
                    tiles[index] = null;
                    moved = true;
                }
            }
            else
            {
                if (tiles[index] != newTile)
                {
                    tiles[index] = newTile;
                    moved = true;
                }
                UpdateTilePosition(newTile);
            }
        }
    }

    private int GetIndexInDirection(int line, int pos, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:  return XYToIndex(pos, line);
            case Direction.Right: return XYToIndex(gridSize - 1 - pos, line);
            case Direction.Up:    return XYToIndex(line, pos);
            case Direction.Down:  return XYToIndex(line, gridSize - 1 - pos);
            default: return 0;
        }
    }

    private List<S_Tile> MergeTiles(List<S_Tile> tilesInLine, ref bool moved)
    {
        List<S_Tile> result = new List<S_Tile>(gridSize);

        for (int i = 0; i < tilesInLine.Count; i++)
        {
            if (i + 1 < tilesInLine.Count && tilesInLine[i].Value == tilesInLine[i + 1].Value)
            {
                int newValue = tilesInLine[i].Value * 2;
                tilesInLine[i].Init(newValue);
                LastMergeValue = newValue;

                Destroy(tilesInLine[i + 1].gameObject);
                result.Add(tilesInLine[i]);
                i++;
                moved = true;
            }
            else
            {
                result.Add(tilesInLine[i]);
            }
        }
        return result;
    }


    private void UpdateEmptyIndices()
    {
        emptyIndices.Clear();
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null)
                emptyIndices.Add(i);
        }
    }

    public void SpawnTile()
    {
        if (emptyIndices.Count == 0) return;

        int randomIndex = Random.Range(0, emptyIndices.Count);
        int spawnIndex = emptyIndices[randomIndex];
        emptyIndices.RemoveAt(randomIndex);

        int value = Random.value > 0.9f ? 4 : 2;
        SpawnTileAt(spawnIndex, value);
    }

    private void SpawnTileAt(int index, int value)
    {
        (int x, int y) = IndexToXY(index);

        S_Tile newTile = Instantiate(tilePrefab);
        RectTransform tileRT = newTile.GetComponent<RectTransform>();
        tileRT.SetParent(gridContainer, false);
        tileRT.sizeDelta = new Vector2(cellSize, cellSize);
        tileRT.anchoredPosition = GetCellAnchoredPosition(x, y);
        newTile.Init(value);

        tiles[index] = newTile;
    }

    private void UpdateTilePosition(S_Tile tile)
    {
        if (tile == null) return;

        RectTransform rt = tile.GetComponent<RectTransform>();
        int index = System.Array.IndexOf(tiles, tile);
        if (index == -1) return;

        (int x, int y) = IndexToXY(index);
        Vector2 targetPos = GetCellAnchoredPosition(x, y);

        if (rt.anchoredPosition != targetPos)
        {
            rt.anchoredPosition = targetPos;
        }
    }

    public bool HasAvailableMoves()
    {
        if (emptyIndices.Count > 0) return true;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;

            int current = tiles[i].Value;
            (int x, int y) = IndexToXY(i);

            if (x < gridSize - 1 && tiles[XYToIndex(x + 1, y)]?.Value == current) return true;
            if (y < gridSize - 1 && tiles[XYToIndex(x, y + 1)]?.Value == current) return true;
        }
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

    private int XYToIndex(int x, int y)
    {
        return y * gridSize + x;
    }

    private (int x, int y) IndexToXY(int index)
    {
        return (index % gridSize, index / gridSize);
    }

    private Vector2 GetCellAnchoredPosition(int x, int y)
    {
        float totalWidth = gridContainer != null ? gridContainer.rect.width : gridSize * cellSize + (gridSize - 1) * spacing;
        float totalHeight = gridContainer != null ? gridContainer.rect.height : gridSize * cellSize + (gridSize - 1) * spacing;

        float startX = -totalWidth / 2f + cellSize / 2f;
        float startY = totalHeight / 2f - cellSize / 2f;

        float posX = startX + x * (cellSize + spacing);
        float posY = startY - y * (cellSize + spacing);

        return new Vector2(posX, posY);
    }
    
}