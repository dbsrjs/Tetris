using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Piece holdPiece { get; private set; }
    public Piece tempPiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;        //���� ��ġ
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
    /// ��Ʈ��̳� ����
    /// </summary>
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);   //�������� ��Ʈ��̳� ����
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        //������ �� �ִ� ��ġ���� üũ
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

        //������ �� �ִ� ��ġ���� üũ
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
    /// ���� ����
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
    /// �̵��ϱ� �� �ܻ� ����
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
    /// ��ȿ�� ��ġ���� üũ
    /// </summary>
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            //���� ����� �ʾҴ��� �Ǵ�
            if(!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            //�ش� ������ �����ϰ� �ִ��� �Ǵ�
            if(tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// ���� ����
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
    /// ���� ���� á���� üũ
    /// </summary>
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if(!tilemap.HasTile(position))  //�� ���� ���� ���� �ʾҴ�.
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// �� ����
    /// </summary>
    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);    //Ÿ���� null�� ����
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
        if (holdPiece == null) // Ȧ�尡 ����ִٸ�, ���� ����� Ȧ���ϰ� �� ����� ��ȯ
        {
            holdPiece = activePiece; // ���ο� ��ü ���� (�ܼ� ������ �ƴ� �������� ���纻)
            print(holdPiece);
            Clear(activePiece); // ���� ��� �����
            SpawnPiece(); // ���ο� ��� ����
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