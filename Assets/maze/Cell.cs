using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Cell {
    public GameObject gameObject;
    public Vector2 index;
    public Cell[] neighborCells = new Cell[4];
    public CellWall[] neighborWalls = new CellWall[4];
    public bool isVisited = false;
    public Cell (int x, int y, GameObject prefab, GameObject parent) {
        gameObject = GameObject.Instantiate (prefab, parent.transform);
        gameObject.name = x.ToString () + "," + y.ToString ();
        gameObject.transform.position = new Vector3 (x, 0, y);
        index = new Vector2 (x, y);
    }
    public void Destroy () {
        GameObject.Destroy (gameObject);
    }
    public void InitNeighborCells (Cell[, ] cells) {
        //上下左右  0 1 2 3  这种顺序   但是需要注意的是 最前面的一行地面是没有更前面的元素的  反之亦然  左右亦然
        if (index.y + 1 < cells.GetLength (1)) { // 上
            neighborCells[0] = cells[(int) index.x, (int) index.y + 1];
        }
        if (index.y - 1 >= 0) { // 下
            neighborCells[1] = cells[(int) index.x, (int) index.y - 1];
        }
        if (index.x - 1 >= 0) { // 左
            neighborCells[2] = cells[(int) index.x - 1, (int) index.y];
        }
        if (index.x + 1 < cells.GetLength (0)) { // 右
            neighborCells[3] = cells[(int) index.x + 1, (int) index.y];
        }
    }

    public void InitNeighborWalls (CellWall[, ] row, CellWall[, ] col) {
        neighborWalls[0] = row[(int) index.x, (int) index.y + 1]; // 上
        neighborWalls[0].AddNeighborCells (this);
        neighborWalls[1] = row[(int) index.x, (int) index.y]; // 下
        neighborWalls[1].AddNeighborCells (this);
        neighborWalls[2] = col[(int) index.x, (int) index.y]; // 左
        neighborWalls[2].AddNeighborCells (this);
        neighborWalls[3] = col[(int) index.x + 1, (int) index.y]; // 右
        neighborWalls[3].AddNeighborCells (this);
    }

    public List<CellWall> GetNeighborWalls () {
        List<CellWall> _walls = new List<CellWall> ();
        foreach (var item in neighborWalls) {
            if (item != null && !item.isBorder) _walls.Add (item);
        }
        return _walls;
    }
    public List<Cell> GetNeighborCells () {
        List<Cell> _cells = new List<Cell> ();
        foreach (var item in neighborCells) {
            if (item != null && !item.isVisited) _cells.Add (item);
        }
        return _cells;
    }

    public void SetVisited () {
        this.isVisited = true;
        // 已访问过的格子,上绿色
        // gameObject.GetComponent<MeshRenderer> ().material.color = new Color (0, 1, 1);
    }
}