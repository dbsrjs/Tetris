using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Piece holdPiece { get; private set; }
    public Piece tempPiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;        //스폰 위치
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

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

    /// <summary>
    /// 테트라미노 생성
    /// </summary>
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);   //랜덤으로 테트라미노 선별
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        //생성할 수 있는 위치인지 체크
        if(IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void SpawnPiece(Piece piece)
    {
        activePiece.Initialize(this, spawnPosition, holdPiece.data);

        //생성할 수 있는 위치인지 체크
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    private void GameOver()
    {
        tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    /// <summary>
    /// 이동하기 전 잔상 제거
    /// </summary>
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    /// <summary>
    /// 유효한 위치인지 체크
    /// </summary>
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            //맵을 벗어나지 않았는지 판단
            if(!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            //해당 공간을 차지하고 있는지 판단
            if(tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 삭제 로직
    /// </summary>
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax)
        {
            if(IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    /// <summary>
    /// 줄이 가득 찼는지 체크
    /// </summary>
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if(!tilemap.HasTile(position))  //이 줄이 가득 차지 않았다.
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 줄 삭제
    /// </summary>
    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);    //타일을 null로 변경
        }

        while(row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void HoladChange()
    {
        print(holdPiece);
        if (holdPiece == null) // 홀드가 비어있다면, 현재 블록을 홀드하고 새 블록을 소환
        {
            holdPiece = activePiece; // 새로운 객체 생성 (단순 참조가 아닌 독립적인 복사본)
            print(holdPiece);
            Clear(activePiece); // 기존 블록 지우기
            SpawnPiece(); // 새로운 블록 생성
        }
        else
        {
            tempPiece = activePiece;
            Clear(activePiece);
            SpawnPiece(holdPiece);
            holdPiece = tempPiece;
        }
    }
}