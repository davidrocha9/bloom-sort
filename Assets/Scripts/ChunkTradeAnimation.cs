using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkTradeAnimation
{
    public List<(int, int)> Changes = new List<(int, int)>();
    public int Amount { get; }
    public Color Color { get; }

    public int AddClearAnimation = -1;

    public ChunkTradeAnimation(BoardNode node)
    {
        for (int i = 0; i < BoardConstants.NUMBER_OF_ROWS; i++)
        {
            for (int j = 0; j < BoardConstants.NUMBER_OF_COLUMNS; j++)
            {
                foreach (var kvp in node.Parent.Board[i][j])
                {
                    if (kvp.Key == Color.white) continue;

                    int oldAmount = node.Parent.Board[i][j][kvp.Key];
                    int currentAmount = node.Board[i][j].ContainsKey(kvp.Key) ? node.Board[i][j][kvp.Key] : 0;

                    int amountDiff = currentAmount - oldAmount;

                    if (amountDiff != 0)
                    {
                        Color = kvp.Key;
                        Changes.Add((4 * i + j, amountDiff));
                    }
                }
            }
        }

        if (CheckIfPieceRemoval())
        {
            Changes[0] = (Changes[0].Item1, -Changes[1].Item2);
            AddClearAnimation = Changes[0].Item1;
        }
    }

    public ChunkTradeAnimation(ChunkTradeAnimation anim)
    {
        Changes.Add((anim.AddClearAnimation, -Constants.MAX_NUMBER_OF_CHUNKS));
        Amount = anim.Amount;
        Color = anim.Color;
    }

    private bool CheckIfPieceRemoval()
    {
        return Changes.Count > 0 && Changes[0].Item2 < 0 && Changes[1].Item2 < 0;
    }
}