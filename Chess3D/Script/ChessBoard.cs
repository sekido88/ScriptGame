using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float yOffset = 0.8f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float deadSize = 0.9f;
    [SerializeField] private float deathSpacing = 0.35f;
    [SerializeField] private float dragOffset = 1.5f;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    //LOGIC
    private const int TITLE_COUNT_X = 8;
    private const int TITLE_COUNT_Y = 8;

    private bool isWhiteTurn;
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> avaiableMoves = new List<Vector2Int>();

    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlack = new List<ChessPiece>();

    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(1.0f, TITLE_COUNT_X, TITLE_COUNT_Y);
        SpawnAllPieces();
        PositionALlPieces();
        isWhiteTurn = true;
    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        ProcessHoverTile();

    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * 1.0f , 0.9f , y * 1.0f) - bounds + new Vector3(0.5f,0,0.5f);
    }
    private void ProcessHoverTile()
    {
     
        
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexes of the tile I've hit
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);
            // If we're hovering a different tile
            if (currentHover != hitPosition )
            {
                
                // If we were   already hovering a tile, change the previous one
                if (currentHover != -Vector2Int.one)
                {
                    tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                  //  tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                }

                // Set the material and layer of the hovered tile
                currentHover = hitPosition;
                if (currentHover.x < 8 && currentHover.x >= 0 && currentHover.x >= 0 && currentHover.x < 8)
                {
                    tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = (ConstValidMoves(ref avaiableMoves,currentHover)) ? highlightMaterial : hoverMaterial;
                  //  tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Hover");
                }
            }
          

            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x,hitPosition.y] != null) // neu ma o nay phai la o chua quan co
                {
                    if (chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn || chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn) 
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];  //
                        avaiableMoves = currentlyDragging.GetAvaialbeMoves(ref chessPieces, TITLE_COUNT_X, TITLE_COUNT_Y);
                        
                        HighlightTiles();
                    }

                }
            }
            /*if (currentlyDragging)
            {

                Plane hozizontalPlane = new Plane(Vector3.up, Vector3.up * 0.9f);
                float distance = 0.0f;
                if (hozizontalPlane.Raycast(ray, out distance))
                {
                    Vector3 posPoint = ray.GetPoint(distance);
                    currentlyDragging.SetPosition(posPoint + Vector3.up * dragOffset, true);
                }
            }*/
 
            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
                
                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if(validMove)
                {
                    RemoveHighlightTiles();
                    currentlyDragging.SetPosition(GetTileCenter(hitPosition.x,hitPosition.y),true) ;
                    currentlyDragging = null;
                }
                else
                {
                    currentlyDragging = null;
                }
            }
 
        }
        else
        {
            // If not hovering over any tile
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = (ConstValidMoves(ref avaiableMoves,currentHover)) ? highlightMaterial : tileMaterial; // Set material back to original when not hovering over any tile
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if(currentlyDragging && Input.GetMouseButtonDown(0))
            {
                  currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                  currentlyDragging = null;
            }
        }
     
         
    }
    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
 
        bounds = new Vector3 ((tileCountX / 2) * tileSize , 0 ,(tileCountX / 2) * tileSize ) + boardCenter;
         

        tiles = new GameObject[tileCountX , tileCountX ]; 
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(1.0f, x, y);
                tiles[x, y].transform.position = new Vector3(0, 0.3f, 0);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0},Y:{1}", x, y));
        tileObject.transform.parent = transform; // Gán cha cho tileObject

        // Lấy vị trí của GameObject cha (chessboard)
        Vector3 parentPosition = transform.position;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        // Tạo các đỉnh (vertices) của tile dựa trên vị trí của cha
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset , y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset , (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset , y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset , (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TITLE_COUNT_X, TITLE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        // white team 
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for(int i = 0;  i < TITLE_COUNT_X; i++)
        {
            chessPieces[i,1]  = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }

        // black team 
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TITLE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type,int team)
    {
        
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team; 
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        cp.SetScale(Vector3.one, true);
        return cp;

    }
    private void PositionALlPieces()
    {
        for (int i = 0; i< TITLE_COUNT_X; i++)
        {
            for(int j = 0; j < TITLE_COUNT_Y; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    PositionSinglePiece(i, j, true);
                }
            }
        }
    }
    private void PositionSinglePiece(int x, int y,bool force = false )
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition( GetTileCenter(x,y) , force );
    }

    // Operation
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if (!ConstValidMoves(ref avaiableMoves, new Vector2Int(x,y)))
        {
            return false;
        }
        Vector2Int previousPosition = new Vector2Int(cp.currentX,cp.currentY);

        // is there another piece on the target position
        if (chessPieces[x,y] != null)
        {
            ChessPiece ocp = chessPieces[x,y];


            if (cp.team == ocp.team)
                return false;


            ocp.SetScale(Vector3.one * deadSize,true);
            if (ocp.team == 0)
            {
                if (ocp.type == ChessPieceType.King)
                {
                    CheckMate(1);
                }

                deadWhites.Add(ocp);
                ocp.SetPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                    - bounds 
                    + new Vector3 (tileSize / 2 , 0 , tileSize / 2 )
                    + ((Vector3.forward * deathSpacing) * deadWhites.Count),true);
            }
            if (ocp.team == 1)
            {
                if (ocp.type == ChessPieceType.King)
                {
                    CheckMate(0);
                }

                deadBlack.Add(ocp);
                ocp.SetPosition(new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                     - bounds
                     + new Vector3(tileSize / 2, 0, tileSize / 2)
                     + ((Vector3.back * deathSpacing) * deadBlack.Count),true);
            }

        }
        // neu ma o do khong co quan co trung mau voi quan co hien tai thi o do se bang quan co draging
        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn;
        return true;
    }
    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        return -Vector2Int.one;
    }

    private void HighlightTiles()
    {
        for(int i = 0;i < avaiableMoves.Count; i++)
        {
            tiles[avaiableMoves[i].x, avaiableMoves[i].y].GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < avaiableMoves.Count; i++)
        {
            tiles[avaiableMoves[i].x, avaiableMoves[i].y].GetComponent<MeshRenderer>().material = tileMaterial;
        }
        avaiableMoves.Clear();
    }
    private bool ConstValidMoves(ref List<Vector2Int> moves,Vector2Int pos) 
    {
        for(int i = 0; i< moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }



    // check mate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    private void OnRestButton()
    {

    }
    public void OnExitButon()
    {
        Application.Quit();
    }
}
 