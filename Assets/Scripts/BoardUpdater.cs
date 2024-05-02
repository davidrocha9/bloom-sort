using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.Collections;
using UnityEngine;

public class BoardEqualityComparer : IEqualityComparer<BoardNode>
{
    public static Vector TOP = new Vector { i = 1, j = 0 };
    public static Vector BOTTOM = new Vector { i = -1, j = 0 };
    public static Vector LEFT = new Vector { i = 0, j = 1 };
    public static Vector RIGHT = new Vector { i = 0, j = -1 };

    public static List<Vector> directions = new List<Vector> {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    };

    public bool Equals(BoardNode b1, BoardNode b2)
    {
        List<List<Dictionary<Color, int>>> x = b1.Board;
        List<List<Dictionary<Color, int>>> y = b2.Board;

        if (x.Count != y.Count)
            return false;

        for (int i = 0; i < x.Count; i++)
        {
            if (x[i].Count != y[i].Count)
                return false;

            for (int j = 0; j < x[i].Count; j++)
            {
                if (!DictionariesEqual(x[i][j], y[i][j]))
                    return false;
            }
        }

        if (b1.Root != b2.Root)
        {
            return false;
        }

        return true;
    }

    private bool DictionariesEqual(Dictionary<Color, int> dict1, Dictionary<Color, int> dict2)
    {
        if (dict1.Count != dict2.Count)
            return false;

        foreach (var kvp in dict1)
        {
            if (!dict2.ContainsKey(kvp.Key) || dict2[kvp.Key] != kvp.Value)
                return false;
        }

        return true;
    }

    public int GetHashCode(BoardNode obj)
    {
        unchecked
        {
            int hash = 17;
            foreach (var list in obj.Board)
            {
                foreach (var dictionary in list)
                {
                    foreach (var pair in dictionary)
                    {
                        hash = hash * 23 + pair.Key.GetHashCode();
                        hash = hash * 23 + pair.Value.GetHashCode();
                    }
                }
            }
            return hash;
        }
    }
}

public class BoardNode
{
    public List<List<Dictionary<Color, int>>> Board { get; }
    public BoardNode Parent { get; }
    public int Depth { get; }
    public double Score { get; }

    public int Root { get; }

    public BoardNode(List<List<Dictionary<Color, int>>> board, BoardNode parent, int depth, int i, int j)
    {
        Board = board;
        Parent = parent;
        Depth = depth;
        Score = CalculateScore();
        Root = 4 * i + j;
    }

    private double CalculateScore()
    {
        double boardScore = 0.0;

        foreach (var row in Board)
        {
            foreach (var tile in row)
            {
                double score = 0;
                foreach (var kvp in tile)
                {
                    score -= Math.Exp(kvp.Value);
                    if (kvp.Key != Color.white) boardScore -= Math.Exp(6 - kvp.Value);
                }
            }
        }

        return boardScore;
    }
}

public class BoardUpdater
{
    private HashSet<BoardNode> visited = new HashSet<BoardNode>(new BoardEqualityComparer());

    public bool CheckIfValidNeighbor(int i, int j, Vector vec, List<List<Dictionary<Color, int>>> board)
    {
        if (i + vec.i < 0 || i + vec.i > 5 || j + vec.j < 0 || j + vec.j > 3)
        {
            return false;
        }

        Dictionary<Color, int> neighborints = board[i + vec.i][j + vec.j];

        return neighborints.ContainsKey(Color.white) && neighborints[Color.white] < 6;
    }

    private List<List<Dictionary<Color, int>>> GetUpdatedBoardWithTrade(int i, int j, Vector vec, List<List<Dictionary<Color, int>>> board, int amount, Color color)
    {
        List<List<Dictionary<Color, int>>> updatedBoard = new List<List<Dictionary<Color, int>>>();
        foreach (var row in board)
        {
            var newRow = new List<Dictionary<Color, int>>();
            foreach (var tile in row)
            {
                var newTile = new Dictionary<Color, int>(tile);
                newRow.Add(newTile);
            }
            updatedBoard.Add(newRow);
        }

        updatedBoard[i][j][color] += amount;
        updatedBoard[i + vec.i][j + vec.j][color] -= amount;

        if (updatedBoard[i][j].ContainsKey(Color.white))
        {
            updatedBoard[i][j][Color.white] -= amount;
        }
        else
        {
            updatedBoard[i][j][Color.white] = -amount;
        }

        if (updatedBoard[i + vec.i][j + vec.j].ContainsKey(Color.white))
        {
            updatedBoard[i + vec.i][j + vec.j][Color.white] += amount;
        }
        else
        {
            updatedBoard[i + vec.i][j + vec.j][Color.white] = amount;
        }

        /*if (CheckIfFullPiece(updatedBoard, i, j))
        {
            updatedBoard = ClearPiece(updatedBoard, i, j);
        }

        if (CheckIfFullPiece(updatedBoard, i + vec.i, j + vec.j))
        {
            updatedBoard = ClearPiece(updatedBoard, i + vec.i, j + vec.j);
        }*/

        // Remove keys with value 0
        updatedBoard[i][j] = updatedBoard[i][j].Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        updatedBoard[i + vec.i][j + vec.j] = updatedBoard[i + vec.i][j + vec.j].Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);

        return updatedBoard;
    }

    private List<List<Dictionary<Color, int>>> ClearPiece(List<List<Dictionary<Color, int>>> board, int i, int j)
    {
        board[i][j] = new Dictionary<Color, int>();

        board[i][j][Color.white] = 6;

        return board;
    }

    public void FindAllPossibleMovesBetween(List<List<Dictionary<Color, int>>> board, int i, int j, Vector vec, BoardNode currentNode)
    {
        Dictionary<Color, int> tile1ints = board[i][j];
        Dictionary<Color, int> tile2ints = board[i + vec.i][j + vec.j];

        List<Color> commonColors = tile1ints.Keys.Intersect(tile2ints.Keys).ToList();
        commonColors.RemoveAll(color => color == Color.white);

        foreach (Color color in commonColors)
        {
            int amountOfintInTile1 = tile1ints[color];
            int amountOfintInTile2 = tile2ints[color];

            int amountOfFreeSpaceInTile1 = tile1ints.ContainsKey(Color.white) ? tile1ints[Color.white] : 0;
            int amountOfFreeSpaceInTile2 = tile2ints.ContainsKey(Color.white) ? tile2ints[Color.white] : 0;

            int incomingExchangeAmount = Math.Min(amountOfFreeSpaceInTile1, amountOfintInTile2);
            //int outgoingExchangeAmount = -Math.Min(amountOfFreeSpaceInTile2, amountOfintInTile1);

            List<List<Dictionary<Color, int>>> incomingBoard = GetUpdatedBoardWithTrade(i, j, vec, board, incomingExchangeAmount, color);
            //List<List<Dictionary<Color, int>>> outgoingBoard = GetUpdatedBoardWithTrade(i, j, vec, board, outgoingExchangeAmount, color);

            if (incomingExchangeAmount > 0)
            {
                GetPossibleMoves(incomingBoard, i, j, currentNode);
                GetPossibleMoves(board, i + vec.i, j + vec.j, currentNode);
            }
            /*if (outgoingExchangeAmount < 0)
            {
                //GetPossibleMoves(outgoingBoard, i, j, vec, outgoingExchangeAmount, color, currentNode);
            }*/
        }
    }

    public bool CheckIfFullPiece(List<List<Dictionary<Color, int>>> board, int i, int j)
    {
        Dictionary<Color, int> colors = board[i][j];

        foreach (Color color in colors.Keys)
        {
            if (colors[color] == 6)
            {
                return true;
            }
        }

        return false;
    }

    public void GetPossibleMoves(List<List<Dictionary<Color, int>>> board, int i, int j, BoardNode parent = null)
    {
        if (CheckIfFullPiece(board, i, j))
        {
            ClearPiece(board, i, j);
        }

        var currentNode = new BoardNode(board, parent, parent?.Depth + 1 ?? 0, i, j);
        if (!visited.Add(currentNode))
        {
            return;
        }

        foreach (Vector vec in Constants.directions)
        {
            if (CheckIfValidNeighbor(i, j, vec, board))
            {
                FindAllPossibleMovesBetween(board, i, j, vec, currentNode);
            }
        }
    }

    public BoardNode GetBestNode()
    {
        BoardNode bestNode = null;

        foreach (var node in visited)
        {
            if (bestNode == null || node.Score > bestNode.Score)
            {
                bestNode = node;
            }
        }

        return bestNode;
    }

    public List<ChunkTradeAnimation> GetChunkTradingAnimations(int tileIndex, List<List<Dictionary<Color, int>>> board)
    {
        visited.Clear();

        int IVal = tileIndex / 4;
        int JVal = tileIndex % 4;

        GetPossibleMoves(board, IVal, JVal, null);

        List<ChunkTradeAnimation> animations = new List<ChunkTradeAnimation>();

        BoardNode bestNode = GetBestNode();

        while (bestNode.Depth > 0)
        {
            ChunkTradeAnimation animation = new ChunkTradeAnimation(bestNode);

            if (animation.Changes.Count > 0)
            {
                animations.Add(animation);
            }

            bestNode = bestNode.Parent;
        }

        return new List<ChunkTradeAnimation>(animations);
    }
}
