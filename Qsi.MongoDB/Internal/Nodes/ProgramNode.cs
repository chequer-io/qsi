namespace Qsi.MongoDB.Internal.Nodes
{
    public class ProgramNode : BaseNode, INode
    {
        public string SourceType
        {
            get => SourceTypeEnum.ToString();
            set
            {
                SourceTypeEnum = value.ToLower() switch
                {
                    "script" => Nodes.SourceType.Script,
                    "module" => Nodes.SourceType.Module,
                    _ => Nodes.SourceType.Module
                };
            }
        }

        public SourceType SourceTypeEnum { get; set; }
        
        public INode[] Body { get; set; }
    }

    public enum SourceType
    {
        Script,
        Module,
    }
}