using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : ChessPiece
{
    private readonly Vector2Int[] possibleMoves = new Vector2Int[]
     {
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1,1), new Vector2Int(1,0),
        new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1,-1), new Vector2Int(-1, 0)
     };
    private bool IsPositionValid(int x, int y, int tileCountX, int tileCountY)
    {
        return x >= 0 && x < tileCountX && y >= 0 && y < tileCountY;
    }
    private bool IsMoveLegal(ChessPiece[,] board, int x, int y)
    {
        return board[x, y] == null || board[x, y].team != board[currentX, currentY].team;
    }

    public override List<Vector2Int> GetAvaialbeMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {   
        List<Vector2Int> r = new List<Vector2Int>();
        
        for(int i = 0; i < possibleMoves.Length; i++)
        {
            int x = currentX + possibleMoves[i].x;
            int y = currentY + possibleMoves[i].y;

            if (IsPositionValid(x,y, tileCountX, tileCountY) && IsMoveLegal(board,x,y) )
            {
                r.Add(new Vector2Int(x,y)); 
            }
        }

        return r;
    }  
}
