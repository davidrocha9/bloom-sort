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

    public void Reset()
    {
        piece.Reset();
    }

    public void FillPiece(Chunk[] chunks)
    {
        piece.SetChunks(chunks);
        piece.gameObject.SetActive(true);
    }

    public List<Color> GetChunksColors()
    {
        return piece.GetChunksColors();
    }

    public bool IsOccupied()
    {
        return GetChunksColors().Count > 0;
    }

    public void AddChunk(Color color)
    {
        piece.AddChunk(color);

        if (CanRemovePiece(color))
        {
            RemovePieceFromTile();
        }
    }

    public void RemoveChunk(Color color)
    {
        piece.RemoveChunk(color);

        if (CheckIfEmptyPiece())
        {
            RemovePieceFromTile();
        }
    }

    private bool CanRemovePiece(Color targetColor)
    {
        List<Color> colors = GetChunksColors();

        return colors.Count(color => color == targetColor) == 6;
    }

    private bool CheckIfEmptyPiece()
    {
        return GetChunksColors().Count == 0;
    }

    private void RemovePieceFromTile()
    {
        piece.Reset();
    }
}
