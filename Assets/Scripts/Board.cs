
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SocialPlatforms.Impl;

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
            tiles[i].RemovePieceFromTile();
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

    private double GetScore()
    {
        double boardScore = 0;

        foreach (Tile tile in tiles)
        {
            boardScore += tile.GetScore();
        }

        return boardScore;
    }

    private Tile GetLeftNeighbor(Tile tile)
    {
        int XCoord = tile.x;
        int YCoord = tile.y;

        if (XCoord < 3)
        {
            int neighborIndex = XCoord + 4 * YCoord + 1;
            return tiles[neighborIndex];
        }

        return null;
    }

    private Tile GetTopNeighbor(Tile tile)
    {
        int XCoord = tile.x;
        int YCoord = tile.y;

        if (YCoord < 5)
        {
            int neighborIndex = XCoord + 4 * YCoord + 1;
            return tiles[neighborIndex];
        }

        return null;
    }

    private bool CheckIfValidNeighbor(int tileIndex, Vector vec)
    {
        int XCoord = tileIndex % 4;
        int YCoord = tileIndex / 4;

        if (vec.x == 1)
        {
            if (XCoord == 3)
            {
                return false;
            }
        }

        if (vec.x == -1)
        {
            if (XCoord == 0)
            {
                return false;
            }
        }

        if (vec.y == 1)
        {
            if (YCoord == 5)
            {
                return false;
            }
        }

        if (vec.y == -1)
        {
            if (YCoord == 0)
            {
                return false;
            }
        }

        return true;
    }

    private int GetPossibleIncomingSwaps(List<Color> tile1ChunksColors, List<Color> tile2ChunksColors, Color color)
    {
        int tile1ColorCount = tile1ChunksColors.Count(color1 => color1 == color);
        int tile2ColorCount = tile2ChunksColors.Count(color1 => color1 == color);

        int exchangeAmount = Mathf.Min(6 - tile1ChunksColors.Count, tile2ColorCount);

        return exchangeAmount;
    }

    private int GetPossibleOutgoingSwaps(List<Color> tile1ChunksColors, List<Color> tile2ChunksColors, Color color)
    {
        int tile1ColorCount = tile1ChunksColors.Count(color1 => color1 == color);
        int tile2ColorCount = tile2ChunksColors.Count(color1 => color1 == color);

        int exchangeAmount = Mathf.Min(6 - tile2ChunksColors.Count, tile1ColorCount);
    
        return exchangeAmount;
    }

    private double GetPossibleSwapScore(Tile tile1, Tile tile2, Color color, int exchangeAmount)
    {
        double tile1Score = tile1.GetScoreWithChunkChange(color, exchangeAmount);
        double tile2Score = tile2.GetScoreWithChunkChange(color, -exchangeAmount);

        return tile1Score + tile2Score;
    }

    private ChunkSwap FindBestSwap(int tileIndex)
    {        
        ChunkSwap bestSwap = new ChunkSwap();
        Tile tile1 = tiles[tileIndex];

        if (!tile1.IsOccupied())
        {
            return bestSwap;
        }

        foreach (Vector vec in Constants.directions)
        {
            if (CheckIfValidNeighbor(tileIndex, vec))
            {
                int neighborIndex = tileIndex + vec.x + vec.y * 4;
                Tile tile2 = tiles[neighborIndex];
                
                List<Color> tile1ChunksColors = tile1.GetChunksColors();
                List<Color> tile2ChunksColors = tile2.GetChunksColors();

                List<Color> commonColors = tile1ChunksColors.Where(color => tile2ChunksColors.Contains(color)).Distinct().ToList();

                foreach (Color color in commonColors)
                {
                    int incomingAmount = GetPossibleIncomingSwaps(tile1ChunksColors, tile2ChunksColors, color);
                    int outgoingAmount = GetPossibleOutgoingSwaps(tile1ChunksColors, tile2ChunksColors, color);
                    
                    double incomingScore = GetPossibleSwapScore(tile1, tile2, color, incomingAmount);
                    double outgoingScore = GetPossibleSwapScore(tile1, tile2, color, -outgoingAmount);
                    double currentScore = tile1.GetScore() + tile2.GetScore();

                    // Debug.Log("Incoming amount is " + incomingAmount + " outgoing amount is " + outgoingAmount);
                    // Debug.Log("Incoming score is " + incomingScore + " outgoing score is " + outgoingScore);

                    double swapScoreGain = 0;
                    if (incomingScore >= outgoingScore)
                    {
                        swapScoreGain = incomingScore - currentScore;

                        if (swapScoreGain > bestSwap.score)
                        {
                            bestSwap = new ChunkSwap(swapScoreGain, tile1, tile2, color, incomingAmount);
                        }
                    }
                    else
                    {
                        swapScoreGain = outgoingScore - currentScore;

                        if (swapScoreGain > bestSwap.score)
                        {
                            bestSwap = new ChunkSwap(swapScoreGain, tile1, tile2, color, - outgoingAmount);
                        }
                    }
                }
            }
        }

        Debug.Log("Best move is: " + bestSwap.score + " " + bestSwap.tile1 + " " + bestSwap.tile2 + " " + bestSwap.color + " " + bestSwap.amount);
        
        return bestSwap;
    }

    private void CheckForChunkSwaps()
    {
        int tileIndex = 0;

        Debug.Log("Start");    

        do
        {
            ChunkSwap bestSwap = FindBestSwap(tileIndex);

            if (bestSwap.score > 0)
            {
                TradeChunks(bestSwap.tile1, bestSwap.tile2, bestSwap.color, bestSwap.amount);
                tileIndex = 0;
            }
            tileIndex++;
        } while (tileIndex < Constants.NUMBER_OF_TILES);

        Debug.Log("End"); 
    }


    private void TradeChunks(Tile tile1, Tile tile2, Color color, int amount)
    {
        if (amount > 0)
        {
            tile1.AddChunk(color, amount);
            tile2.RemoveChunk(color, amount);
        }
        else
        {
            tile1.RemoveChunk(color, amount);
            tile2.AddChunk(color, amount);
        }

        if (tile1.CheckIfCanBeCleared(color))
        {
            tile1.RemovePieceFromTile();
        }

        if (tile2.CheckIfCanBeCleared(color))
        {
            tile2.RemovePieceFromTile();
        }
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

    internal class ChunkSwap : IComparable<ChunkSwap>
    {
        public double score { get; }
        public Tile tile1 { get; }
        public Tile tile2 { get; }
        public Color color { get; }
        public int amount { get; }

        public ChunkSwap()
        {
            score = 0;
        }

        public ChunkSwap(double score, Tile tile1, Tile tile2, Color color, int amount)
        {   
            this.score  = score;
            this.tile1  = tile1;
            this.tile2  = tile2;
            this.color  = color;
            this.amount = amount;
        }

        public int CompareTo(ChunkSwap other)
        {
            return other.score.CompareTo(score);
        }
    }
}