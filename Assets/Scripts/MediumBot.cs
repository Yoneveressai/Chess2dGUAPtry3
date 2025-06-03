using System.Collections.Generic;
using UnityEngine;

public class MediumBot : MonoBehaviour
{
    public GameObject cl;

    public void MakeMove()
    {
        Debug.Log("MediumBot: MakeMove() called");
        Game game = cl.GetComponent<Game>();
        List<GameObject> pieces = game.GetAllPieces(game.GetCurrentPlayer());
        GameObject bestPiece = null;
        Vector2Int bestMove = Vector2Int.zero;
        GameObject bestTarget = null;

        // Проверяем возможные атаки
        foreach (GameObject piece in pieces)
        {
            List<Vector2Int> possibleMoves = piece.GetComponent<Chessman>().PotentialMoves(game);

            foreach (Vector2Int move in possibleMoves)
            {
                GameObject target = game.GetPosition(move.x, move.y);

                // Если клетка занята фигурой противника, выбираем ее для атаки
                if (target != null && target.GetComponent<Chessman>().player != game.GetCurrentPlayer())
                {
                    bestPiece = piece;
                    bestMove = move;
                    bestTarget = target;
                    break;
                }
            }
        }

        if (bestPiece != null)
        {
            Debug.Log("MediumBot: Атакует " + bestTarget.name);
            game.SetPositionEmpty(bestPiece.GetComponent<Chessman>().GetXBoard(), bestPiece.GetComponent<Chessman>().GetYBoard());
            bestPiece.GetComponent<Chessman>().SetXBoard(bestMove.x);
            bestPiece.GetComponent<Chessman>().SetYBoard(bestMove.y);
            bestPiece.GetComponent<Chessman>().SetCoords();
            game.SetPosition(bestMove.x, bestMove.y, bestPiece);

            Destroy(bestTarget); // Удаляем атакованную фигуру
        }
        else
        {
            // Если атакующих ходов нет, делаем случайный ход
            List<GameObject> validPieces = new List<GameObject>();

            foreach (GameObject piece in pieces)
            {
                if (piece.GetComponent<Chessman>().PotentialMoves(game).Count > 0)
                {
                    validPieces.Add(piece);
                }
            }

            if (validPieces.Count > 0)
            {
                GameObject randomPiece = validPieces[Random.Range(0, validPieces.Count)];
                List<Vector2Int> possibleMoves = randomPiece.GetComponent<Chessman>().PotentialMoves(game);
                Vector2Int randomMove = possibleMoves[Random.Range(0, possibleMoves.Count)];

                game.SetPositionEmpty(randomPiece.GetComponent<Chessman>().GetXBoard(), randomPiece.GetComponent<Chessman>().GetYBoard());
                randomPiece.GetComponent<Chessman>().SetXBoard(randomMove.x);
                randomPiece.GetComponent<Chessman>().SetYBoard(randomMove.y);
                randomPiece.GetComponent<Chessman>().SetCoords();
                game.SetPosition(randomMove.x, randomMove.y, randomPiece);
            }
        }

        game.NextTurn();
    }
}