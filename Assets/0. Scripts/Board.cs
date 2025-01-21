using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap {  get; private set; }
    public Piece activePiece { get; private set; } 
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);   //랜덤으로 테트라미노 선별
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);
        Set(activePiece);
    }

    public void Set(Piece piece)
    {
        for(int i  = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
}