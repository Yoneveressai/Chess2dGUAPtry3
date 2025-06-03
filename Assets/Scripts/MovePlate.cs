using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    //Board positions, not world positions
    int matrixX;
    int matrixY;

    //false: movement, true: attacking
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            //change to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            if (cp.name.Contains("king"))
            {
                controller.GetComponent<Game>().DeclareVictory(controller.GetComponent<Game>().OpponentOf(cp.GetComponent<Chessman>().player));
            }

            Destroy(cp);
        }

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard());

        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();
        reference.GetComponent<Chessman>().MarkMoved();

        controller.GetComponent<Game>().SetPosition(reference);

        // Проверка рокировки
        Chessman piece = reference.GetComponent<Chessman>();
        if (piece.name.Contains("king"))
        {
            // Левая рокировка
            if (matrixX == 2)
            {
                GameObject rook = controller.GetComponent<Game>().GetPosition(0, matrixY);
                controller.GetComponent<Game>().SetPositionEmpty(0, matrixY);
                rook.GetComponent<Chessman>().SetXBoard(3);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPosition(rook);
                rook.GetComponent<Chessman>().MarkMoved();
            }
            // Правая рокировка
            else if (matrixX == 6)
            {
                GameObject rook = controller.GetComponent<Game>().GetPosition(7, matrixY);
                controller.GetComponent<Game>().SetPositionEmpty(7, matrixY);
                rook.GetComponent<Chessman>().SetXBoard(5);
                rook.GetComponent<Chessman>().SetCoords();
                controller.GetComponent<Game>().SetPosition(rook);
                rook.GetComponent<Chessman>().MarkMoved();
            }
        }




        // Проверка на продвижение пешки
        if (piece.name.Contains("pawn") && (matrixY == 7 || matrixY == 0))
        {
            controller.GetComponent<Game>().ShowPromotionMenu(reference);

            controller.GetComponent<Game>().NextTurn();
        }
        else
        {
            controller.GetComponent<Game>().NextTurn();
        }

        string current = controller.GetComponent<Game>().GetCurrentPlayer();
        if (controller.GetComponent<Game>().IsCheckmate(current))
        {
            controller.GetComponent<Game>().DeclareVictory(controller.GetComponent<Game>().OpponentOf(current)); // или "black"
            // Тут можно показать UI «Мат»
        }
        else if (controller.GetComponent<Game>().IsInCheck(current))
        {
            Debug.Log("Шах " + current + " королю!");
        }

        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }


}