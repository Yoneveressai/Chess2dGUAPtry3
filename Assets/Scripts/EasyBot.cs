using System.Collections.Generic;
using UnityEngine;

public class EasyBot : MonoBehaviour
{
    public GameObject cl;

    public void MakeMove()
    {
        Debug.Log("EasyBot: MakeMove() called");
        Game game = cl.GetComponent<Game>();
        List<GameObject> pieces = game.GetAllPieces(game.GetCurrentPlayer());
        List<GameObject> validPieces = new List<GameObject>();

        // Собираем только фигуры, у которых есть доступные ходы
        foreach (GameObject piece in pieces)
        {
            if (piece.GetComponent<Chessman>().PotentialMoves(game).Count > 0)
            {
                Debug.Log(piece);
                validPieces.Add(piece);
            }

            Debug.Log(piece);
        }


        if (validPieces.Count > 0)
        {
            GameObject randomPiece = validPieces[Random.Range(0, validPieces.Count)];
            List<Vector2Int> possibleMoves = randomPiece.GetComponent<Chessman>().PotentialMoves(game);
            Vector2Int randomMove = possibleMoves[Random.Range(0, possibleMoves.Count)];

            // **Используем твою механику передвижения**
            game.SetPositionEmpty(randomPiece.GetComponent<Chessman>().GetXBoard(), randomPiece.GetComponent<Chessman>().GetYBoard());
            randomPiece.GetComponent<Chessman>().SetXBoard(randomMove.x);
            randomPiece.GetComponent<Chessman>().SetYBoard(randomMove.y);
            randomPiece.GetComponent<Chessman>().SetCoords();
            game.SetPosition(randomMove.x, randomMove.y, randomPiece);

            game.NextTurn();
        }
    }
}
