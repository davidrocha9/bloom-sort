using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    private Piece[] pieces;
    public event EventHandler<DropPieceEventArgs> DropPieceEvent;
    private int currentNumberOfPieces = 3;

    private void Awake()
    {
        // Subscribing to piece dropped event
        for (int x = 0; x < pieces.Length; x++)
        {
            pieces[x].PieceDroppedEvent += OnPieceDropped;
            pieces[x].Reset();
        }

        // Generating initial pieces
        GeneratePieces();
    }

    void OnValidate()
    {
        UpdatePieces();
    }

    void UpdatePieces()
    {
        pieces = GetComponentsInChildren<Piece>();
    }

    private void GeneratePieces()
    {
        for (int x = 0; x < pieces.Length; x++)
        {
            pieces[x].Init(Constants.SPAWN_COORDS[x]);
            pieces[x].SetIndex(x);
        }
    }

    private bool CheckIfPieceHolderEmpty()
    {
        return currentNumberOfPieces == 0;
    }

    public void RemovePiece(int pieceIndex)
    {
        pieces[pieceIndex].RemoveInstantly();

        currentNumberOfPieces--;

        if (CheckIfPieceHolderEmpty())
        {
            GeneratePieces();
            currentNumberOfPieces = 3;
        }
    }

    private void OnPieceDropped(object sender, DropPieceEventArgs e)
    {
        float XCoord = e.XCoord;
        float YCoord = e.YCoord;
        List<Color> chunks = e.chunks;
        int index = e.index;

        DropPieceEvent?.Invoke(this, new DropPieceEventArgs(XCoord, YCoord, chunks, index));
    }
}
