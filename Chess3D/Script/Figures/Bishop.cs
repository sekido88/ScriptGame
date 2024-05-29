using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvaialbeMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // duong cheo chinh
        for (int i = currentY + 1, j = currentX + 1; i < tileCountY && j < tileCountX; i++, j++)
        {
            if (board[j, i] == null)
            {
                r.Add(new Vector2Int(j, i));
            }
            else if (board[j, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(j, i));
                break;
            }
            else break;
        }
        for (int i = currentY -1, j = currentX - 1; i >= 0 && j >= 0; i-- ,j--)
        {
            if (board[j, i] == null)
            {
                r.Add(new Vector2Int(j, i));
            }
            else if (board[j, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(j, i));
                break;
            }
            else break;
        }

        // duong cheo phu
        for (int i = currentY + 1, j = currentX - 1; i < tileCountY && j >= 0; i++, j--)
        {
            if (board[j, i] == null)
            {
                r.Add(new Vector2Int(j, i));
            }
            else if (board[j, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(j, i));
                break;
            }
            else break;
        }
        for (int i = currentY - 1, j = currentX + 1; i >= 0 && j < tileCountX; i--, j++)
        {
            if (board[j, i] == null)
            {
                r.Add(new Vector2Int(j, i));
            }
            else if (board[j, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(j, i));
                break;
            }
            else break;
        }
        return r;
    }

}
