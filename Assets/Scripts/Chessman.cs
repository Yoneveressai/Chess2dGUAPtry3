using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //references
    public GameObject controller;
    public GameObject movePlate;

    //Positions
    private int xBoard = -1;
    private int yBoard = -1;

    private bool hasMoved = false;
    public bool HasMoved() => hasMoved;
    public void MarkMoved() => hasMoved = true;

    //variable to keep track of black or white player
    public string player;

    //References for all the sprotes that the chesspiece can be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //take the instantiated location adjust the transform
        SetCoords();

        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;

            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }


    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.88f;
        y *= 0.88f;

        x += -3.08f;
        y += -3.08f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;

    }
    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    private void OnMouseUp()
    {
        Game game = controller.GetComponent<Game>();

        if (!game.IsGameOver() && game.GetCurrentPlayer() == player)
        {


            DestroyMovePlates();
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] MovePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < MovePlates.Length; i++)
        {
            Destroy(MovePlates[i]);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1,
-1)
;
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                CastleMovePlates();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;

        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            // Основной ход на 1 клетку вперед
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);

                // Дополнительный ход на 2 клетки для пешки в начальной позиции
                if ((player == "white" && yBoard == 1) || (player == "black" && yBoard == 6))
                {
                    int doubleMoveY = player == "white" ? y + 1 : y - 1;
                    if (sc.PositionOnBoard(x, doubleMoveY) && sc.GetPosition(x, doubleMoveY) == null)
                    {
                        MovePlateSpawn(x, doubleMoveY);
                    }
                }
            }

            // Атаки по диагонали (оставляем без изменений)
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
                sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
                sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }



    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        // Если это король — проверим, не будет ли он под шахом
        if (name.Contains("king"))
        {
            if (WouldBeInCheck(game, matrixX, matrixY))
            {
                return; // Не создаём movePlate, если в результате хода будет шах
            }
        }

        float x = matrixX * 0.88f - 3.08f;
        float y = matrixY * 0.88f - 3.08f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.88f;
        y *= 0.88f;

        x += -3.08f;
        y += -3.08f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void CastleMovePlates()
    {
        Game sc = controller.GetComponent<Game>();

        // Только если король не двигался
        if (this.HasMoved()) return;

        // Левая ладья (ферзевый фланг)
        GameObject leftRook = sc.GetPosition(0, yBoard);
        if (leftRook != null && leftRook.name.Contains("rook") && leftRook.GetComponent<Chessman>().player == player && !leftRook.GetComponent<Chessman>().HasMoved())
        {
            if (sc.GetPosition(1, yBoard) == null &&
                sc.GetPosition(2, yBoard) == null &&
                sc.GetPosition(3, yBoard) == null)
            {
                MovePlateSpawn(2, yBoard); // рокировка влево
            }
        }

        // Правая ладья (королевский фланг)
        GameObject rightRook = sc.GetPosition(7, yBoard);
        if (rightRook != null && rightRook.name.Contains("rook") && rightRook.GetComponent<Chessman>().player == player && !rightRook.GetComponent<Chessman>().HasMoved())
        {
            if (sc.GetPosition(5, yBoard) == null &&
                sc.GetPosition(6, yBoard) == null)
            {
                MovePlateSpawn(6, yBoard); // рокировка вправо
            }
        }
    }

    public List<Vector2Int> PotentialMoves(Game game)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int x = GetXBoard();
        int y = GetYBoard();

        if (name.Contains("pawn"))
            GetPawnMoves(game, x, y, moves);
        else if (name.Contains("rook"))
            GetLineMoves(game, x, y, moves, new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
        else if (name.Contains("bishop"))
            GetLineMoves(game, x, y, moves, new Vector2Int[] {
            Vector2Int.up + Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down + Vector2Int.left });
        else if (name.Contains("queen"))
            GetLineMoves(game, x, y, moves, new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down + Vector2Int.left });
        else if (name.Contains("knight"))
            GetKnightMoves(game, x, y, moves);
        else if (name.Contains("king"))
            GetKingMoves(game, x, y, moves);

        return moves;
    }

    private void GetPawnMoves(Game game, int x, int y, List<Vector2Int> moves)
    {
        int direction = name.Contains("white") ? 1 : -1;
        int startRow = name.Contains("white") ? 1 : 6;

        // Вперёд
        if (game.PositionOnBoard(x, y + direction) && game.GetPosition(x, y + direction) == null)
            moves.Add(new Vector2Int(x, y + direction));

        // Двойной ход
        if (y == startRow && game.GetPosition(x, y + direction) == null &&
            game.GetPosition(x, y + 2 * direction) == null)
            moves.Add(new Vector2Int(x, y + 2 * direction));

        // Взятие по диагонали
        foreach (int dx in new int[] { -1, 1 })
        {
            int nx = x + dx;
            int ny = y + direction;

            if (game.PositionOnBoard(nx, ny))
            {
                GameObject other = game.GetPosition(nx, ny);
                if (other != null && !other.name.Contains(GetColor()))
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
    }

    private void GetLineMoves(Game game, int x, int y, List<Vector2Int> moves, Vector2Int[] directions)
    {
        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            while (game.PositionOnBoard(nx, ny))
            {
                GameObject piece = game.GetPosition(nx, ny);
                if (piece == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                }
                else
                {
                    if (!piece.name.Contains(GetColor()))
                        moves.Add(new Vector2Int(nx, ny));
                    break;
                }

                nx += dir.x;
                ny += dir.y;
            }
        }
    }

    private void GetKnightMoves(Game game, int x, int y, List<Vector2Int> moves)
    {
        Vector2Int[] offsets = {
        new Vector2Int(2,1), new Vector2Int(1,2),
        new Vector2Int(-1,2), new Vector2Int(-2,1),
        new Vector2Int(-2,-1), new Vector2Int(-1,-2),
        new Vector2Int(1,-2), new Vector2Int(2,-1)
    };

        foreach (Vector2Int offset in offsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;

            if (game.PositionOnBoard(nx, ny))
            {
                GameObject piece = game.GetPosition(nx, ny);
                if (piece == null || !piece.name.Contains(GetColor()))
                    moves.Add(new Vector2Int(nx, ny));
            }
        }
    }
    private void GetKingMoves(Game game, int x, int y, List<Vector2Int> moves)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (game.PositionOnBoard(nx, ny))
                {
                    GameObject piece = game.GetPosition(nx, ny);
                    if (piece == null || !piece.name.Contains(GetColor()))
                        moves.Add(new Vector2Int(nx, ny));
                }
            }
        }
    }

    private string GetColor()
    {
        return name.Contains("white") ? "white" : "black";
    }

    private bool WouldBeInCheck(Game game, int targetX, int targetY)
    {
        // Сохраняем текущие позиции
        int originalX = GetXBoard();
        int originalY = GetYBoard();
        GameObject originalOccupant = game.GetPosition(targetX, targetY);

        // Пробуем сделать ход
        game.SetPositionEmpty(originalX, originalY);
        SetXBoard(targetX);
        SetYBoard(targetY);
        game.SetPosition(this.gameObject);

        bool inCheck = game.IsSquareUnderAttack(targetX, targetY, GetColor());

        // Откатываем
        SetXBoard(originalX);
        SetYBoard(originalY);
        game.SetPosition(this.gameObject);
        game.SetPosition(targetX, targetY, originalOccupant);

        return inCheck;
    }

}