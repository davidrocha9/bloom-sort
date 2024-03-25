
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class Board : MonoBehaviour
{
    public GameObject restartButton;
    public Tile[] tiles;
    public PieceSpawner pieceSpawner;
    public TextMeshProUGUI scoreText;
    private int score = 0;

    void Start()
    {
        pieceSpawner.DropPieceEvent += OnPieceDropped;

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Reset();
        }
    }

    private bool CheckIfCoordsAreInBounds(float XCoord, float YCoord)
    {
        return XCoord >= 0 && XCoord <= 3 && YCoord >= 0 && YCoord <= 5;
    }

    private void OnPieceDropped(object sender, DropPieceEventArgs e)
    {
        float XCoord = e.XCoord;
        float YCoord = e.YCoord;
        Chunk[] chunks = e.chunks;

        XCoord = Mathf.Floor((Constants.MIN_X - XCoord) / Constants.UNIT);
        YCoord = Mathf.Floor((YCoord - Constants.MIN_Y) / Constants.UNIT);

        if (CheckIfCoordsAreInBounds(XCoord, YCoord))
        {
            int tileIndex = Mathf.RoundToInt(XCoord + 4 * YCoord);
            Tile chosenTileToDropPiece = tiles[tileIndex];

            if (!chosenTileToDropPiece.IsOccupied())
            {
                chosenTileToDropPiece.FillPiece(chunks);
                pieceSpawner.RemovePiece(e.index);

                CheckForChunkSwaps();

                if (CheckGameOver())
                {
                    ShowGameOverScreen();
                }
            }
        }
    }

    private void CheckForChunkSwaps()
    {
        // Filter out occupied tiles
        List<Tile> occupiedTiles = tiles.Where(tile => tile.IsOccupied()).ToList();

        // Iterate over occupied tiles
        foreach (Tile currentTile in occupiedTiles)
        {
            // Get coordinates of the current tile
            int XCoord = currentTile.x;
            int YCoord = currentTile.y;

            // Check if the current tile is occupied
            if (!currentTile.IsOccupied())
            {
                continue; // Skip to the next iteration if the tile has been emptied out in the process
            }

            // TODO: implement heuristic for checking if it's a good idea to swap chunks

            // Check left neighbour
            if (XCoord < 3)
            {
                Tile leftNeighbour = tiles[XCoord + 4 * YCoord + 1];
                if (leftNeighbour.IsOccupied())
                {
                    TradeChunks(currentTile, leftNeighbour);
                }
            }

            // Check top neighbour
            if (YCoord < 5)
            {
                Tile topNeighbour = tiles[XCoord + 4 * (YCoord + 1)];
                if (topNeighbour.IsOccupied())
                {
                    TradeChunks(currentTile, topNeighbour);
                }
            }
        }
    }


    private void TradeChunks(Tile tile1, Tile tile2)
    {
        // Loop until no more swaps can be made
        bool checkForSwaps;
        do
        {
            // Get the colors of chunks on both tiles
            List<Color> tile1Chunks = tile1.GetChunksColors();
            List<Color> tile2Chunks = tile2.GetChunksColors();

            // If either tile has reached maximum chunks, exit
            if (tile1Chunks.Count == 6 || tile2Chunks.Count == 6)
            {
                return;
            }

            HashSet<Color> tile1ChunksSet = new HashSet<Color>(tile1Chunks);
            checkForSwaps = false;

            foreach (Color color in tile1ChunksSet)
            {
                int tile1Count = tile1Chunks.Count(num => num == color);
                int tile2Count = tile2Chunks.Count(num => num == color);

                // If both tiles have chunks of the same color
                if (tile1Count > 0 && tile2Count > 0)
                {
                    checkForSwaps = true;

                    if (tile1Count >= tile2Count)
                    {
                        tile1.AddChunk(color);
                        tile2.RemoveChunk(color);

                        // If tile1 becomes unoccupied, increase score
                        if (!tile1.IsOccupied())
                        {
                            IncreaseScore();
                        }
                    }
                    else
                    {
                        tile1.RemoveChunk(color);
                        tile2.AddChunk(color);

                        // If tile2 becomes unoccupied, increase score
                        if (!tile2.IsOccupied())
                        {
                            IncreaseScore();
                        }
                    }
                }
            }
        } while (checkForSwaps);
    }


    private bool CheckGameOver()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (!tiles[i].IsOccupied())
            {
                return false;
            }
        }
        
        return true;
    }

    private void ShowGameOverScreen()
    {
        restartButton.SetActive(true);
    }

    private void IncreaseScore()
    {
        score += 50;
        scoreText.text = $"Score: {score}";
    }
}
