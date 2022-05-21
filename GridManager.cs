using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public int width;
    public int height;
    public Cell[,] gridArray;
    private float cellSize;

    private GameObject tile;

    public GridManager(int width, int height, float cellSize, GameObject tile, int seed, float scale, int octaves, float presistancy, float lan, Vector2 offset, Color[] colors)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.tile = tile;

        gridArray = new Cell[width, height];

        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, scale, octaves, presistancy, lan, offset);

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Cell cell = new Cell();

                gridArray[x, y] = cell;
                cell.cellLocation = new Vector2Int(x, y);

                GameObject Tile = GameObject.Instantiate(tile, GetWorldPos(x, y), Quaternion.identity);
                cell.tile = tile;

                Tile.transform.localScale = new Vector3(cellSize * 0.06439041f, cellSize * 0.069f, 1);

                AssignColors(Tile, noiseMap[x, y], cell, colors);

                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.black, 100f);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.black, 100f);
            }
        }

        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.black, 100f);
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.black, 100f);
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    public void GetXY(Vector3 pos, out int x, out int y)
    {
        x = Mathf.FloorToInt(pos.x / cellSize);
        y = Mathf.FloorToInt(pos.y / cellSize);
    }

    public Cell GetCellXY(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / cellSize);
        int y = Mathf.FloorToInt(pos.y / cellSize);

        return gridArray[x, y];

    }

    public List<Cell> GetCellNeighborList(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        if(cell.cellLocation.x > 0)
        {
            neighbors.Add(gridArray[cell.cellLocation.x - 1, cell.cellLocation.y]);
        }

        if (cell.cellLocation.x < width)
        {
            neighbors.Add(gridArray[cell.cellLocation.x + 1, cell.cellLocation.y]);
        }

        if (cell.cellLocation.y > 0)
        {
            neighbors.Add(gridArray[cell.cellLocation.x, cell.cellLocation.y - 1]);
        }

        if(cell.cellLocation.y < height)
        {
            neighbors.Add(gridArray[cell.cellLocation.x, cell.cellLocation.y + 1]);
        }

        return neighbors;
    }

    public void AssignColors(GameObject tile, float value, Cell cell, Color[] colors)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

        if (value < 0.4f)
        {
            spriteRenderer.color = colors[0];
            cell.celltype = "Deep Ocean";
        }
        else if (value >= 0.4f && value < 0.55f)
        {
            spriteRenderer.color = colors[1];
            cell.celltype = "Shallow Ocean";
        }
        else if (value >= 0.55f && value < 0.6f)
        {
            spriteRenderer.color = colors[2];
            cell.celltype = "Sand";
        }
        else if (value >= 0.6f && value < 0.7f)
        {
            spriteRenderer.color = colors[3];
            cell.celltype = "LightGrass";
        }
        else if (value >= 0.7f && value < 0.79f)
        {
            spriteRenderer.color = colors[4];
            cell.celltype = "Dark Grass";
        }
        else if (value >= 0.79f && value < 0.9f)
        {
            spriteRenderer.color = colors[5];
            cell.celltype = "Rocks";
        }
        else
        {
            spriteRenderer.color = colors[6];
            cell.celltype = "Snow";
        }

    }
}