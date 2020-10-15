namespace Qsi.MongoDB.Internal.Nodes.Location
{
    public struct NodeLocation
    {
        public Location Start { get; set; }
        
        public Location End { get; set; }

        public override string ToString()
        {
            return $"Start: [{Start}], End: [{End}]";
        }
    }

    public struct Location
    {
        public int Line { get; set; }
        
        public int Column { get; set; }

        public override string ToString()
        {
            return $"Line: {Line}, Column: {Column}";
        }
    }
}
