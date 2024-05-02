using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Board : MonoBehaviour
{
    public GameObject restartButton;
    public Tile[] tiles;
    public PieceSpawner pieceSpawner;
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private BoardUpdater boardUpdater;
    private List<ChunkTradeAnimation> animations = new List<ChunkTradeAnimation>();

    void Start()
    {
        boardUpdater = new BoardUpdater();

        pieceSpawner.DropPieceEvent += OnPieceDropped;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].IsOccupied())
            {
                foreach (Chunk chunk in tiles[i].piece.chunks)
                {
                    chunk.animator.SetBool("Idle", true);
                }
            }
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
        List<Color> chunks = e.chunks;

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

                animations = boardUpdater.GetChunkTradingAnimations(tileIndex, GetBoardDict());
                PlayAnimationFromQueue();

                /*if (CheckGameOver())
                {
                    ShowGameOverScreen();
                }*/
            }
        }
    }

    private List<List<Dictionary<Color, int>>> GetBoardDict()
    {
        List<List<Dictionary<Color, int>>> board = new List<List<Dictionary<Color, int>>>();
        List<Dictionary<Color, int>> row = new List<Dictionary<Color, int>>();
        Dictionary<Color, int> dic;

        foreach (Tile tile in tiles)
        {
            dic = new Dictionary<Color, int>();

            if (tile.IsOccupied())
            {
                List<Color> tileChunksColors = tile.GetChunksColors();

                foreach (Color color in tileChunksColors)
                {
                    if (dic.ContainsKey(color))
                    {
                        dic[color]++;
                    }
                    else
                    {
                        dic[color] = 1;
                    }
                }

                dic[Color.white] = Constants.MAX_NUMBER_OF_CHUNKS - tileChunksColors.Count;
            }
            else
            {
                dic[Color.white] = Constants.MAX_NUMBER_OF_CHUNKS;
            }

            row.Add(dic);

            if (row.Count == 4)
            {
                board.Add(row);
                row = new List<Dictionary<Color, int>>();
            }
        }

        return board;
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

    public void AnimationEnded()
    {
        PlayAnimationFromQueue();
    }

    private void PlayAnimationFromQueue()
    {
        if (animations.Count > 0)
        {
            ChunkTradeAnimation animation = animations[animations.Count - 1];
            animations.RemoveAt(animations.Count - 1);

            List<(int, int)> changes = animation.Changes;
            Color color = animation.Color;

            foreach (var change in changes)
            {
                Tile targetTile = tiles[change.Item1];

                if (change.Item2 > 0)
                {
                    targetTile.AddChunk(color, change.Item2);
                }
                else
                {
                    targetTile.RemoveChunk(color, -change.Item2);
                }
            }
        }
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
            this.score = score;
            this.tile1 = tile1;
            this.tile2 = tile2;
            this.color = color;
            this.amount = amount;
        }

        public int CompareTo(ChunkSwap other)
        {
            return other.score.CompareTo(score);
        }
    }
}