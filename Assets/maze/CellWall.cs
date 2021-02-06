using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CellWall : IEquatable<CellWall> {
    public GameObject gameObject;
    public Vector2 index;
    public bool isBorder = false;
    float offset = 0.5f;
    public bool isDestroy = false;
    public List<Cell> neighborCells = new List<Cell> ();

    public CellWall (int x, int y, GameObject prefab, GameObject parent, bool isColl, bool isBorder) {
        gameObject = GameObject.Instantiate (prefab, parent.transform);
        gameObject.name = x.ToString () + "," + y.ToString ();
        gameObject.transform.position = isColl? new Vector3 (x - offset, offset, y) : new Vector3 (x, offset, y - offset);
        // 默认墙体是水平的,是列则旋转90°
        if (isColl) {
            gameObject.transform.Rotate (new Vector3 (0, 90f, 0), Space.World);
        }
        this.isBorder = isBorder;
        // 是边界墙体,则染成蓝色
        if (this.isBorder) {
            gameObject.GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 1);
        }
        index = new Vector2 (x, y);
    }
    public void Destroy () {
        isDestroy = true;
        GameObject.Destroy (gameObject);
    }
    public void AddNeighborCells (Cell cell) {
        neighborCells.Add (cell);
    }
    public List<Cell> GetNeighborCells () {
        return neighborCells;
    }
    public bool Equals (CellWall other) {
        if (other == null) return false;
        return (this.index.x == other.index.x && this.index.y == other.index.y);
    }
    public override int GetHashCode () {
        return Tuple.Create (index.x, index.y).GetHashCode ();
    }
}