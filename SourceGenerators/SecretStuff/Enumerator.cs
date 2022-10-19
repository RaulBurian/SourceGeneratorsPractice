namespace SourceGenerators.SecretStuff;

public static class Enumerator
{
    public static CustomEnumerator GetEnumerator(this Range self)
    {
        return new CustomEnumerator(self, 1, self.Start.Value < self.End.Value ? Direction.Asc : Direction.Desc);
    }

    public static CustomEnumerator GetEnumerator(this (Range Range, int Step) self)
    {
        return new CustomEnumerator(self.Range, self.Step, self.Step > 0 ? Direction.Asc : Direction.Desc);
    }
}

public struct CustomEnumerator
{
    private readonly int _end;
    private readonly int _step;
    private readonly Direction _direction;

    public CustomEnumerator(Range range, int step, Direction direction)
    {
        Current = range.Start.Value + (direction is Direction.Asc ? -1 : +1);
        _end = range.End.Value;
        _step = step;
        _direction = direction;
    }

    public int Current { get; private set; }

    public bool MoveNext()
    {
        Current += _direction is Direction.Asc ? _step : -_step;

        return _direction is Direction.Asc ? Current < _end : Current > _end;
    }
}

public enum Direction
{
    Asc,
    Desc
}
