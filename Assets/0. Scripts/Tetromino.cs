using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;     //테트라미노 타입
    public Tile tile;               //타일 색
    public Vector2Int[] cells { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[tetromino];
    }
}