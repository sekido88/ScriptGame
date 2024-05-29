using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetAvaialbeMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // Right
        for (int i =currentX + 1; i < tileCountX; i++)
        {
            if (board[i, currentY] == null)
            {
                r.Add(new Vector2Int(i, currentY));
            }
            else if (board[i, currentY].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(i, currentY));
                break;
            }
            else break;
           
        }
        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null)
            {
                r.Add(new Vector2Int(i, currentY));
            }
            else if (board[i, currentY].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(i, currentY));
                break;
            }
            else break;
        }
        // Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if (board[currentX, i] == null)
            {
                r.Add(new Vector2Int(currentX, i));
            }
            else if (board[currentX, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(currentX, i));
                break;
            }
            else break;
        }
        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (board[currentX, i] == null)
            {
                r.Add(new Vector2Int(currentX, i));
            }
            else if (board[currentX, i].team != board[currentX, currentY].team)
            {
                r.Add(new Vector2Int(currentX, i));
                break;
            }
            else break;
        }

        return r;
    }
}
