using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;    //스탭걸리는 시간
    public float lockDelay = 0.5f;  //락에 걸리는 시간

    private float stepTime;         //다음 스탭까지의 시간
    private float lockTime;         //다음 락까지의 시간


    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="board"></param>
    /// <param name="position"></param>
    /// <param name="data"></param>
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }


    private void Update()
    {
        board.Clear(this);

        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            board.HoladChange();
        }

        if (Time.time >= stepTime)
        {
            Step();
        }

        board.Set(this);
    }
    
    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if(lockTime >= lockDelay)
        {
            Lock();
        }
    }

    /// <summary>
    /// 잠금
    /// </summary>
    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece(); //새 조각 생성
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
            continue;

        Lock();
    }

    /// <summary>
    /// 좌, 우, 하 이동
    /// </summary>
    /// <param name="translation">이동할 방향</param>
    /// <returns>이동 할 수 있는지 없는지</returns>
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        //이동할 수 있는 자리인지 확인 후 이동
        if (valid)
        {
            position = newPosition;
            lockTime = 0;
        }

        return valid;
    }

    /// <summary>
    /// 회전
    /// </summary>
    /// <param name="direction">회전할 방향</param>
    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if(!TestWallKicks(rotationIndex, direction))    //회전 시킨 후 벽킥을 테스트 한다.
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);            //벽킥이라면 원래의 위치로 이동 한다.
        }
    }

    /// <summary>
    /// 실질적 회전
    /// </summary>
    /// <param name="direction">회전 방향</param>
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                //I와 O는 모양이 2개 뿐이기 때문에 따로 처리 
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;

                    //Mathf.CeilToInt: 올림
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    //Mathf.RoundToInt: 반올림
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if(Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        
        if(rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if(input < min)
            return max - (min - input) % (max - min);

        else
            return min + (input - min) % (max - min);
    }
}