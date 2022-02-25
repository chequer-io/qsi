namespace Qsi.MongoDB.Internal.Nodes.Location;

public struct NodeLocation
{
    public Location Start { get; set; }

    public Location End { get; set; }

    public NodeLocation(Location start, Location end)
    {
        Start = start;
        End = end;
    }

    public override string ToString()
    {
        return $"Start: [{Start}], End: [{End}]";
    }
}

public struct Location
{
    public int Line { get; set; }

    public int Column { get; set; }

    public Location(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"Line: {Line}, Column: {Column}";
    }
}
