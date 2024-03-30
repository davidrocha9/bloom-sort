public class Move
{
    public int Row { get; }
    public int Column { get; }
    public int Value { get; }

    public Move(int row, int column, int value)
    {
        Row = row;
        Column = column;
        Value = value;
    }
}