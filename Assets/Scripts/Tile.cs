using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece;
    public int x, y;

    public void FillPiece(Chunk[] chunks)
    {
        piece.SetChunks(chunks);
    }

    public List<Color> GetChunksColors()
    {
        return piece.GetChunksColors();
    }

    public bool IsOccupied()
    {
        return GetChunksColors().Count > 0;
    }

    public void AddChunk(Color color, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            piece.AddChunk(color);
        }
    }

    public void RemoveChunk(Color color, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            piece.RemoveChunk(color);
        }
    }

    private bool CheckIfEmptyPiece()
    {
        return GetChunksColors().Count == 0;
    }

    public void FreePiece()
    {
        //piece.animator.Play("Remove");
        //RemovePieceFromTile();
    }

    public void RemovePieceFromTile()
    {
        piece.Reset();
    }

    public double GetScore()
    {
        List<Color> chunkColors = GetChunksColors();
        Dictionary<Color, int> colorsDict = CountColors(chunkColors);

        return CalculateScore(colorsDict, chunkColors.Count);
    }

    public double GetScoreWithChunkChange(Color targetColor, int exchangeAmount)
    {
        List<Color> chunkColors = GetChunksColors();
        Dictionary<Color, int> colorsDict = CountColors(chunkColors);
    
        colorsDict[targetColor] += exchangeAmount;

        return CalculateScore(colorsDict, chunkColors.Count + exchangeAmount);
    }

    private Dictionary<Color, int> CountColors(List<Color> colors)
    {
        Dictionary<Color, int> colorsDict = new Dictionary<Color, int>();

        foreach (Color color in colors)
        {
            if (colorsDict.ContainsKey(color))
                colorsDict[color]++;
            else
                colorsDict[color] = 1;
        }

        return colorsDict;
    }

    private double CalculateScore(Dictionary<Color, int> colorsDict, int chunkCount)
    {
        double tileScore = 0;
        
        foreach (KeyValuePair<Color, int> color in colorsDict)
        {
            tileScore += Math.Pow(10, color.Value);
        }

        tileScore -= chunkCount;
        
        return tileScore;
    }

    public bool CheckIfCanBeCleared(Color color)
    {
        return piece.CheckIfCanBeCleared(color);
    }
}
