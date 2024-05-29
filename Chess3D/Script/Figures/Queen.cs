using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvaialbeMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
     // Rook 
        // Right
        for (int i = currentX + 1; i < tileCountX; i++)
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
     // Bisshop
        // duong chep chinh
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
        for (int i = currentY - 1, j = currentX - 1; i >= 0 && j >= 0; i--, j--)
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
