using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MyTestScript
{
    private class ColorEqualityComparer : IEqualityComparer<Color>
    {
        public bool Equals(Color x, Color y)
        {
            return Mathf.Approximately(x.r, y.r) &&
                   Mathf.Approximately(x.g, y.g) &&
                   Mathf.Approximately(x.b, y.b) &&
                   Mathf.Approximately(x.a, y.a);
        }

        public int GetHashCode(Color obj)
        {
            int hash = 17;
            hash = hash * 31 + obj.r.GetHashCode();
            hash = hash * 31 + obj.g.GetHashCode();
            hash = hash * 31 + obj.b.GetHashCode();
            hash = hash * 31 + obj.a.GetHashCode();
            return hash;
        }
    }

    private class DictionaryComparer : IEqualityComparer<Dictionary<Color, int>>
    {
        private readonly IEqualityComparer<Color> colorComparer = new ColorEqualityComparer();

        public bool Equals(Dictionary<Color, int> x, Dictionary<Color, int> y)
        {
            foreach (var kvp in x)
            {
                if (kvp.Key == Color.white) continue;

                if (!y.TryGetValue(kvp.Key, out int value) && kvp.Value != value)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(Dictionary<Color, int> obj)
        {
            int hash = 17;
            foreach (var kvp in obj)
            {
                hash = hash * 31 + colorComparer.GetHashCode(kvp.Key);
                hash = hash * 31 + kvp.Value.GetHashCode();
            }
            return hash;
        }
    }

    private List<List<Dictionary<Color, int>>> GetEmptyBoard()
    {
        List<List<Dictionary<Color, int>>> board = new List<List<Dictionary<Color, int>>>();
        List<Dictionary<Color, int>> row = new List<Dictionary<Color, int>>();

        for (int i = 0; i < BoardConstants.NUMBER_OF_ROWS; i++)
        {
            for (int j = 0; j < BoardConstants.NUMBER_OF_COLUMNS; j++)
            {
                Dictionary<Color, int> tile = new Dictionary<Color, int>(new ColorEqualityComparer());
                tile[Color.white] = 6;
                row.Add(tile);
            }
            board.Add(row);
            row = new List<Dictionary<Color, int>>();
        }

        return board;
    }

    private bool AreBoardsEqual(List<List<Dictionary<Color, int>>> board1, List<List<Dictionary<Color, int>>> board2)
    {
        if (board1.Count != board2.Count) return false;

        for (int i = 0; i < board1.Count; i++)
        {
            if (board1[i].Count != board2[i].Count) return false;

            for (int j = 0; j < board1[i].Count; j++)
            {
                if (!new DictionaryComparer().Equals(board1[i][j], board2[i][j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Test]
    public void Test1To1Exchange_1()
    {
        List<List<Dictionary<Color, int>>> boardBefore = GetEmptyBoard();
        List<List<Dictionary<Color, int>>> boardAfter = GetEmptyBoard();

        boardBefore[0][0][Color.red] = 1;
        boardBefore[0][0][Color.white] = 5;
        boardBefore[0][1][Color.red] = 1;
        boardBefore[0][1][Color.white] = 5;

        boardAfter[0][0][Color.red] = 2;
        boardAfter[0][0][Color.white] = 4;
        boardAfter[0][1][Color.red] = 0;
        boardAfter[0][1][Color.white] = 6;

        BoardUpdater boardUpdater = new BoardUpdater();
        boardBefore = boardUpdater.GestBestBoard(0, boardBefore);

        Assert.IsTrue(AreBoardsEqual(boardAfter, boardBefore));
    }

    [Test]
    public void Test1To1Exchange_2()
    {
        List<List<Dictionary<Color, int>>> boardBefore = GetEmptyBoard();
        List<List<Dictionary<Color, int>>> boardAfter = GetEmptyBoard();

        boardBefore[0][0][Color.red] = 1;
        boardBefore[0][0][Color.blue] = 1;
        boardBefore[0][0][Color.white] = 4;
        boardBefore[0][1][Color.red] = 1;
        boardBefore[0][1][Color.blue] = 1;
        boardBefore[0][1][Color.white] = 4;

        boardAfter[0][0][Color.red] = 2;
        boardAfter[0][0][Color.blue] = 2;
        boardAfter[0][0][Color.white] = 2;
        boardAfter[0][1][Color.red] = 0;
        boardAfter[0][1][Color.blue] = 0;
        boardAfter[0][1][Color.white] = 6;

        BoardUpdater boardUpdater = new BoardUpdater();
        boardBefore = boardUpdater.GestBestBoard(0, boardBefore);

        Assert.IsTrue(AreBoardsEqual(boardAfter, boardBefore));
    }

    [Test]
    public void Test1To1Exchange_3()
    {
        List<List<Dictionary<Color, int>>> boardBefore = GetEmptyBoard();
        List<List<Dictionary<Color, int>>> boardAfter = GetEmptyBoard();

        boardBefore[0][0][Color.red] = 1;
        boardBefore[0][0][Color.blue] = 1;
        boardBefore[0][0][Color.white] = 3;
        boardBefore[0][1][Color.red] = 2;
        boardBefore[0][1][Color.blue] = 1;
        boardBefore[0][1][Color.white] = 3;

        boardAfter[0][0][Color.red] = 3;
        boardAfter[0][0][Color.blue] = 2;
        boardAfter[0][0][Color.white] = 1;
        boardAfter[0][1][Color.red] = 0;
        boardAfter[0][1][Color.blue] = 0;
        boardAfter[0][1][Color.white] = 6;

        BoardUpdater boardUpdater = new BoardUpdater();
        boardBefore = boardUpdater.GestBestBoard(0, boardBefore);

        Assert.IsTrue(AreBoardsEqual(boardAfter, boardBefore));
    }

    [Test]
    public void Test1To1Exchange_4()
    {
        List<List<Dictionary<Color, int>>> boardBefore = GetEmptyBoard();
        List<List<Dictionary<Color, int>>> boardAfter = GetEmptyBoard();

        boardBefore[0][0][Color.red] = 4;
        boardBefore[0][0][Color.blue] = 1;
        boardBefore[0][0][Color.white] = 1;
        boardBefore[0][1][Color.red] = 1;
        boardBefore[0][1][Color.blue] = 4;
        boardBefore[0][1][Color.white] = 1;

        boardAfter[0][0][Color.red] = 5;
        boardAfter[0][0][Color.blue] = 0;
        boardAfter[0][0][Color.white] = 1;
        boardAfter[0][1][Color.red] = 0;
        boardAfter[0][1][Color.blue] = 5;
        boardAfter[0][1][Color.white] = 1;

        BoardUpdater boardUpdater = new BoardUpdater();
        boardBefore = boardUpdater.GestBestBoard(0, boardBefore);

        Assert.IsTrue(AreBoardsEqual(boardAfter, boardBefore));
    }

    [Test]
    public void Test2To1Exchange_1()
    {
        List<List<Dictionary<Color, int>>> boardBefore = GetEmptyBoard();
        List<List<Dictionary<Color, int>>> boardAfter = GetEmptyBoard();

        boardBefore[0][0][Color.red] = 1;
        boardBefore[0][0][Color.white] = 5;
        boardBefore[0][1][Color.red] = 1;
        boardBefore[0][1][Color.white] = 5;
        boardBefore[1][0][Color.red] = 1;
        boardBefore[1][0][Color.white] = 5;

        boardAfter[0][0][Color.red] = 3;
        boardAfter[0][0][Color.white] = 3;
        boardAfter[0][1][Color.red] = 0;
        boardAfter[0][1][Color.white] = 6;
        boardAfter[1][0][Color.red] = 0;
        boardAfter[1][0][Color.white] = 6;

        BoardUpdater boardUpdater = new BoardUpdater();
        boardBefore = boardUpdater.GestBestBoard(0, boardBefore);

        Assert.IsTrue(AreBoardsEqual(boardAfter, boardBefore));
    }
}