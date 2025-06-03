using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    public GameObject Chesspiece;
    public GameObject movePlatePrefab;

    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    public EasyBot bot;
    public MediumBot botMedium;
    public HardBot botHard;

    private float switcher = 0;

    //Positions and team for each chesspiece
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";

    private bool gameOver = false;

    public GameObject whitePromotionMenu;
    public GameObject blackPromotionMenu;
    private GameObject pawnToPromote = null; // Пешка, которую нужно заменить



    // Start is called before the first frame update
    void Start()
    {

        playerWhite = new GameObject[]
        {
            Create("white_rook",0,0), Create("white_knight",1,0), Create("white_bishop",2,0),
            Create("white_queen",3,0), Create("white_king",4,0), Create("white_bishop",5,0),
            Create("white_knight",6,0), Create("white_rook",7,0), Create("white_pawn",0,1),
            Create("white_pawn",1,1), Create("white_pawn",2,1), Create("white_pawn",3,1),
            Create("white_pawn",4,1), Create("white_pawn",5,1), Create("white_pawn",6,1),
            Create("white_pawn",7,1),
        };

        playerBlack = new GameObject[]
        {
            Create("black_rook",0,7), Create("black_knight",1,7), Create("black_bishop",2,7),
            Create("black_queen",3,7), Create("black_king",4,7), Create("black_bishop",5,7),
            Create("black_knight",6,7), Create("black_rook",7,7), Create("black_pawn",0,6),
            Create("black_pawn",1,6), Create("black_pawn",2,6), Create("black_pawn",3,6),
            Create("black_pawn",4,6), Create("black_pawn",5,6), Create("black_pawn",6,6),
            Create("black_pawn",7,6),
        };

        //Set all piece positions on the position board
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }


    }

    public void ShowPromotionMenu(GameObject pawn)
    {
        pawnToPromote = pawn;

        bool isWhite = pawn.name.Contains("white");

        if (isWhite)
        {
            whitePromotionMenu.SetActive(true);
            blackPromotionMenu.SetActive(false);
        }
        else
        {
            blackPromotionMenu.SetActive(true);
            whitePromotionMenu.SetActive(false);
        }
    }

    public void HidePromotionMenu()
    {
        whitePromotionMenu.SetActive(false);
        blackPromotionMenu.SetActive(false);
        pawnToPromote = null;
    }
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(Chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);

        cm.movePlate = movePlatePrefab;

        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();


        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }


    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
            StartCoroutine(DelayedBotMove());
        }
        else
        {
            currentPlayer = "white";
        }
    }

    // Корутина для задержки перед ходом бота
    private IEnumerator DelayedBotMove()
    {
        yield return new WaitForSeconds(1.5f); // Задержка 1.5 секунды
        switch (switcher)
        {

            case 1: bot.MakeMove(); break;
            case 2: botMedium.MakeMove(); break;
            case 3: botHard.MakeMove(); break;
            default: break;
        }

    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            SceneManager.LoadScene("Game");
        }
    }

    public void PromoteToQueen()
    {
        PromotePawn("queen");
    }

    public void PromoteToRook()
    {
        PromotePawn("rook");
    }

    public void PromoteToBishop()
    {
        PromotePawn("bishop");
    }

    public void PromoteToKnight()
    {
        PromotePawn("knight");
    }

    private void PromotePawn(string newType)
    {
        if (pawnToPromote == null) return;

        int x = pawnToPromote.GetComponent<Chessman>().GetXBoard();
        int y = pawnToPromote.GetComponent<Chessman>().GetYBoard();
        string color = pawnToPromote.name.Contains("white") ? "white" : "black";

        Destroy(pawnToPromote);

        GameObject newPiece = Create(color + "_" + newType, x, y);
        SetPosition(newPiece);

        HidePromotionMenu();

    }

    public bool IsInCheck(string player)
    {
        GameObject king = FindKing(player);
        if (king == null) return false;

        int kingX = king.GetComponent<Chessman>().GetXBoard();
        int kingY = king.GetComponent<Chessman>().GetYBoard();

        foreach (GameObject piece in GetAllPieces(OpponentOf(player)))
        {
            Chessman cm = piece.GetComponent<Chessman>();
            List<Vector2Int> moves = cm.PotentialMoves(this);

            foreach (Vector2Int move in moves)
            {
                if (move.x == kingX && move.y == kingY)
                    return true; // Шах
            }
        }

        return false;
    }

    private GameObject FindKing(string player)
    {
        GameObject[] pieces = player == "white" ? playerWhite : playerBlack;
        foreach (GameObject piece in pieces)
        {
            if (piece != null && piece.name == player + "_king")
                return piece;
        }
        return null;
    }

    public string OpponentOf(string player)
    {
        return player == "white" ? "black" : "white";
    }

    public List<GameObject> GetAllPieces(string player)
    {
        GameObject[] pieces = player == "white" ? playerWhite : playerBlack;
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject piece in pieces)
        {
            if (piece != null)
                list.Add(piece);
        }
        return list;
    }

    public bool IsCheckmate(string player)
    {
        if (!IsInCheck(player)) return false;

        List<GameObject> pieces = GetAllPieces(player);

        foreach (GameObject piece in pieces)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            List<Vector2Int> moves = cm.PotentialMoves(this);

            foreach (Vector2Int move in moves)
            {
                // Копия позиции
                int startX = cm.GetXBoard();
                int startY = cm.GetYBoard();

                GameObject target = GetPosition(move.x, move.y);
                SetPositionEmpty(startX, startY);
                SetPosition(piece);
                SetPositionEmpty(move.x, move.y);
                SetPosition(piece);
                cm.SetXBoard(move.x);
                cm.SetYBoard(move.y);

                if (!IsInCheck(player))
                {
                    // откат
                    cm.SetXBoard(startX);
                    cm.SetYBoard(startY);
                    SetPosition(piece);
                    SetPositionEmpty(move.x, move.y);
                    if (target != null) SetPosition(target);
                    return false;
                }

                // откат
                cm.SetXBoard(startX);
                cm.SetYBoard(startY);
                SetPosition(piece);
                SetPositionEmpty(move.x, move.y);
                if (target != null) SetPosition(target);
            }
        }

        return true;
    }

    public void DeclareVictory(string winnerColor)
    {
        gameOver = true;
        gameOverPanel.SetActive(true);

        if (winnerColor == "white")
            gameOverText.text = "Белые победили!";
        else
            gameOverText.text = "Чёрные победили!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsSquareUnderAttack(int x, int y, string byColor)
    {
        GameObject[] opponentPieces = byColor == "white" ? playerBlack : playerWhite;

        foreach (GameObject piece in opponentPieces)
        {
            if (piece == null) continue;

            List<Vector2Int> moves = piece.GetComponent<Chessman>().PotentialMoves(this);
            foreach (Vector2Int move in moves)
            {
                if (move.x == x && move.y == y)
                    return true;
            }
        }

        return false;
    }
    public void SetPosition(int x, int y, GameObject obj)
    {
        positions[x, y] = obj;
    }

    public bool IsKingInCheck(string color)
    {
        GameObject[] pieces = color == "white" ? playerWhite : playerBlack;

        foreach (GameObject piece in pieces)
        {
            if (piece != null && piece.name.Contains("king"))
            {
                int kingX = piece.GetComponent<Chessman>().GetXBoard();
                int kingY = piece.GetComponent<Chessman>().GetYBoard();

                return IsSquareUnderAttack(kingX, kingY, color);
            }
        }

        return false;
    }
    public void ActivateEzBot()
    {
        switcher = 1;
    }

    public void ActivateMediumBot()
    {
        switcher = 2;
    }

    public void ActivateHardBot()
    {
        switcher = 3;
    }

    public void PlayWithFriend()
    {
        switcher = 0;
    }

}

