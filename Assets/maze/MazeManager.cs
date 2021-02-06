using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeManager : MonoBehaviour {
    public static MazeManager Instance;
    private void Awake () { Instance = this; }

    public int mapWidth = 5, mapHeight = 5;
    [SerializeField] Cell[, ] cells;
    [SerializeField] CellWall[, ] wallCols, wallRows;
    public GameObject cellPrefab, wallPrefab;
    GameObject cellGroup, wallGroup;
    public string cellsGroupName = "cellGroup", wallsGroupName = "cellWallsGroup";

    public int originX = 0, originY = 0;
    public void PrimMaze () {
        if (originX >= mapWidth || originY >= mapHeight) {
            Debug.LogError ("初始位置不在迷宫内");
            return;
        }
        // 待判断是否需要打通的墙集合
        List<CellWall> _walls = new List<CellWall> ();
        // 初始点
        Cell currCell = cells[originX, originY];
        currCell.SetVisited ();
        _walls.AddRange (currCell.GetNeighborWalls ());
        while (_walls.Count > 0) {
            // 随机选一面墙
            CellWall randomWall = _walls[Random.Range (0, _walls.Count)];
            if (randomWall.isBorder || randomWall.isDestroy) {
                _walls.Remove (randomWall);
            }
            List<Cell> neighborCells = randomWall.GetNeighborCells ();
            bool isAllCellVisited = true;
            foreach (Cell _cell in neighborCells) {
                if (!_cell.isVisited) {
                    isAllCellVisited = false;
                    break;
                }
            }
            // 如果这面墙分隔的两个单元格只有一个单元格被访问过
            if (!isAllCellVisited) {
                // 那就从列表里移除这面墙，即把墙打通，让未访问的单元格成为迷宫的通路
                randomWall.Destroy ();
                // 把这个格子的墙加入列表
                foreach (Cell _cell in neighborCells) {
                    if (_cell.isVisited) continue;
                    _cell.SetVisited ();
                    _walls.AddRange (_cell.GetNeighborWalls ());
                }
                // 去重
                _walls = _walls.Distinct ().ToList ();
            }
            // 移除当前墙
            _walls.Remove (randomWall);
        }
    }
    public void Clear () {
        for (int i = 0; i < cells.GetLength (0); i++) {
            for (int j = 0; j < cells.GetLength (1); j++) {
                cells[i, j].Destroy ();
            }
        }
        for (int i = 0; i < wallCols.GetLength (0); i++) {
            for (int j = 0; j < wallCols.GetLength (1); j++) {
                wallCols[i, j].Destroy ();
            }
        }
        for (int i = 0; i < wallRows.GetLength (0); i++) {
            for (int j = 0; j < wallRows.GetLength (1); j++) {
                wallRows[i, j].Destroy ();
            }
        }
    }
    public void Init () {
        // 创建分组
        cellGroup = GameObject.Find (cellsGroupName);
        if (cellGroup == null) {
            cellGroup = new GameObject (cellsGroupName);
        }
        wallGroup = GameObject.Find (wallsGroupName);
        if (wallGroup == null) {
            wallGroup = new GameObject (wallsGroupName);
        }
        // 声明cell和wall
        cells = new Cell[mapWidth, mapHeight];
        wallCols = new CellWall[mapWidth + 1, mapHeight];
        wallRows = new CellWall[mapWidth, mapHeight + 1];
        CreateCells ();
        CreateWallCols (ref wallCols, true);
        CreateWallCols (ref wallRows, false);
        // 给cell建立邻居cell和wall关系
        foreach (var item in cells) {
            item.InitNeighborCells (cells);
            item.InitNeighborWalls (wallRows, wallCols);
        }
    }
    public void CreateCells () {
        for (int i = 0; i < mapWidth; i++) {
            for (int j = 0; j < mapHeight; j++) {
                cells[i, j] = new Cell (i, j, cellPrefab, cellGroup);
            }
        }
    }
    public void CreateWallCols (ref CellWall[, ] arr, bool isColl) {
        for (int i = 0; i < arr.GetLength (0); i++) {
            for (int j = 0; j < arr.GetLength (1); j++) {
                bool isBorder;
                if (isColl) {
                    isBorder = (i == mapWidth || i == 0);
                } else {
                    isBorder = (j == mapHeight || j == 0);
                }
                arr[i, j] = new CellWall (i, j, wallPrefab, wallGroup, isColl, isBorder);
            }
        }
    }
}