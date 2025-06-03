using System.Collections.Generic;
using UnityEngine;

public class HardBot : MonoBehaviour
{
    public GameObject cl;

    public void MakeMove()
    {
        Debug.Log("HardBot: MakeMove() called");
        Game game = cl.GetComponent<Game>();
        List<GameObject> pieces = game.GetAllPieces(game.GetCurrentPlayer());
        GameObject bestPiece = null;
        Vector2Int bestMove = Vector2Int.zero;
        int bestScore = int.MinValue;

        // Анализируем все возможные ходы
        foreach (GameObject piece in pieces)
        {
            List<Vector2Int> possibleMoves = piece.GetComponent<Chessman>().PotentialMoves(game);

            foreach (Vector2Int move in possibleMoves)
            {
                int score = EvaluateMove(game, piece, move);

                // Выбираем ход с наилучшим рейтингом
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPiece = piece;
                    bestMove = move;
                }
            }
        }

        if (bestPiece != null)
        {
            Debug.Log("HardBot: Выполняет стратегический ход");
            game.SetPositionEmpty(bestPiece.GetComponent<Chessman>().GetXBoard(), bestPiece.GetComponent<Chessman>().GetYBoard());
            bestPiece.GetComponent<Chessman>().SetXBoard(bestMove.x);
            bestPiece.GetComponent<Chessman>().SetYBoard(bestMove.y);
            bestPiece.GetComponent<Chessman>().SetCoords();
            if (game.GetPosition(bestMove.x, bestMove.y) != null)
            {
                Destroy(game.GetPosition(bestMove.x, bestMove.y)); // Удаляем фигуру противника
            }
            game.SetPosition(bestMove.x, bestMove.y, bestPiece);
        }

        game.NextTurn();
    }

    private int EvaluateMove(Game game, GameObject piece, Vector2Int move)
    {
        int score = 0;

        // Проверяем, захватывает ли ход фигуру противника
        GameObject target = game.GetPosition(move.x, move.y);
        if (target != null && target.GetComponent<Chessman>().player != game.GetCurrentPlayer())
        {
            score += GetPieceValue(target.GetComponent<Chessman>().name); // Оцениваем важность съеденной фигуры
        }

        // Оцениваем контроль над центральными позициями
        score += GetBoardControlBonus(move);

        return score;
    }

    private int GetPieceValue(string pieceName)
    {
        if (pieceName.Contains("queen")) return 9;
        if (pieceName.Contains("rook")) return 5;
        if (pieceName.Contains("bishop") || pieceName.Contains("knight")) return 3;
        if (pieceName.Contains("pawn")) return 1;
        return 0;
    }

    private int GetBoardControlBonus(Vector2Int move)
    {
        // Центр доски более выгоден для стратегии
        return (4 - Mathf.Abs(move.x - 4)) + (4 - Mathf.Abs(move.y - 4));
    }
}
