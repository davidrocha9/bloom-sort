using System.Collections.Generic;
using UnityEngine;

public class ChunkTradeAnimation
{
    public List<(int, int)> Changes = new List<(int, int)>();
    public int Amount { get; }
    public Color Color { get; }

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
                        Changes.Add((4 * i + j, amountDiff));
                        Color = kvp.Key;
                    }
                }
            }
        }
    }
}