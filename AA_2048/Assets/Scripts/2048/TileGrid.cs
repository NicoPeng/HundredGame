using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; } //当前的Grid统辖的Row有哪些
    public TileCell[] cells { get; private set; } //网格内的所有Cell单元格
    public int size => cells.Length;

    public int height => rows.Length;
    public int width => size / height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    /// <summary>
    /// @formatter:on
    /// </summary>
    private void Start()
    {
        for (var i = 0; i < rows.Length; i++)
        for (var j = 0; j < rows[i].cells.Length; j++)
            rows[i].cells[j].coordinates = new Vector2Int(j, i);
    }

    /// <summary>
    /// 根据坐标获取单元格
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public TileCell GetCell(Vector2Int coord)
    {
        return GetCell(coord.x, coord.y);
    }

    /// <summary>
    /// 根据坐标获取单元格
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return rows[y].cells[x];
        else
            return null;
    }

    /// <summary>
    /// 获取一个单元格在某个方向的邻居
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        var coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;
        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        var emptyCells = new List<TileCell>();
        foreach (var cell in cells)
            if (cell.IsEmpty())
                emptyCells.Add(cell);

        //如果有空的单元格
        if (emptyCells.Count > 0) return emptyCells[Random.Range(0, emptyCells.Count)];

        return null;
    }
}