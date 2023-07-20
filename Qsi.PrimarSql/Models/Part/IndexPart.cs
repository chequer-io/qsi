namespace PrimarSql.Data.Models.Columns;

internal sealed class IndexPart : IPart
{
    public int Index { get; }

    public IndexPart(int index)
    {
        Index = index;
    }

    public override bool Equals(object obj)
    {
        if (obj is not IndexPart indexPart)
            return false;

        return Index == indexPart.Index;
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }
}