using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager; //游戏管理器
    public Tile tilePrefab; //   tile的预制
    public TileState[] tileStates; //所有的Tile状态配置

    public TileGrid grid; //网格
    private List<Tile> tiles; //当前已经创建的Tile
    private bool waiting; //是否处于等待操作的状态 用来控制

    /// <summary>
    /// 清除所有游戏状态
    /// </summary>
    public void ClearBoard()
    {
        foreach (var cell in grid.cells) cell.tile = null;

        foreach (var tile in tiles) Destroy(tile.gameObject);

        tiles.Clear();
    }

    /// <summary>
    /// 随机生成一个Tile
    /// </summary>
    public void CreateTile()
    {
        var tile = Instantiate(tilePrefab, grid.transform);
        //TODO:这里总是会生成2，可以继续根据规则继续优化
        tile.SetState(tileStates[0]);
        tile.LinkCell(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    public bool CheckForGameOver()
    {
        //Tile数量不够单元格
        if (tiles.Count != grid.size) return false;

        //通过循环判断某个单元格的四个方向，任意一个方向可以合并的话，游戏都未结束
        foreach (var tile in tiles)
        {
            var up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            var down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            var left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            var right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up is not null && CanMerge(tile, up.tile)) return false;

            if (down is not null && CanMerge(tile, down.tile)) return false;

            if (left is not null && CanMerge(tile, left.tile)) return false;

            if (right is not null && CanMerge(tile, right.tile)) return false;
        }

        return true;
    }

    private void Awake()
    {
        tiles = new List<Tile>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Move(Vector2Int.up, 0, 1, 1, 1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector2Int.left, 1, 1, 0, 1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
    }

    /// <summary>
    /// 移动格子
    /// </summary>
    /// <param name="direction">移动的方向</param>
    /// <param name="startX">移动的初始X</param>
    /// <param name="incrementX">移动的X方向步长</param>
    /// <param name="startY">移动的初始Y</param>
    /// <param name="incrementY">移动的Y方向的步长</param>
    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        var changed = false;
        for (var x = startX; x >= 0 && x < grid.width; x += incrementX)
        for (var y = startY; y >= 0 && y < grid.height; y += incrementY)
        {
            var cell = grid.GetCell(x, y);

            if (cell.IsOccupied()) changed |= MoveTile(cell.tile, direction);
        }

        if (changed) StartCoroutine(WaitForChanges());
    }

    private IEnumerator WaitForChanges()
    {
        CreateTile();
        if (CheckForGameOver()) gameManager.GameOver();
        yield return null;
    }

    /// <summary>
    /// 朝指定的方向移动单元格
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        var adjcent = grid.GetAdjacentCell(tile.cell, direction);
        //如果有相邻的Cell
        while (adjcent is not null)
        {
            if (adjcent.IsOccupied()) //相邻的Cell里面有Tile
            {
                //如果正好他们俩可以合并，那就执行合并，本次Tile的移动处理就结束了
                if (CanMerge(tile, adjcent.tile))
                {
                    MergeTiles(tile, adjcent.tile);
                    return true;
                }

                //如果不能合并，也就到此结束了，什么都不用做
                break;
            }

            //执行到这里，说明它相邻的格子为空，那么就把当前相邻的格子作为一个目标，再去判断相邻的格子的相邻格子的情况
            newCell = adjcent;
            adjcent = grid.GetAdjacentCell(adjcent, direction);
        }

        //执行完之后就知道是否有最终的新格子，将Tile移动到最终的格子上去。
        if (newCell is not null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判断两个单元格是否能合并
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="otherTile"></param>
    /// <returns></returns>
    private bool CanMerge(Tile tile, Tile otherTile)
    {
        return tile.state == otherTile.state && !otherTile.locked;
    }

    /// <summary>
    /// 将tile单元格合并到otherTile单元格上去
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="otherTile"></param>
    private void MergeTiles(Tile tile, Tile otherTile)
    {
        tiles.Remove(tile);
        tile.Merge(otherTile.cell);

        //找到合并之后的新状态
        var index = Mathf.Clamp(IndexOf(otherTile.state) + 1, 0, tileStates.Length - 1);
        var newState = tileStates[index];

        //设置状态
        otherTile.SetState(newState);
        otherTile.locked = false;
        //增加分数
        gameManager.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        return Array.IndexOf(tileStates, state);
    }
}