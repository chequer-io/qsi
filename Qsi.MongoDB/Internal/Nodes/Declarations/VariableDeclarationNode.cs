namespace Qsi.MongoDB.Internal.Nodes
{
    public class VariableDeclarationNode : BaseNode, IDeclarationNode
    {
        public VariableDeclaratorNode[] Declarations { get; set; }
        
        public string Kind
        {
            get => KindEnum.ToString();
            set
            {
                KindEnum = value.ToLower() switch
                {
                    "var" => Nodes.Kind.Var,
                    "let" => Nodes.Kind.Let,
                    "const" => Nodes.Kind.Const,
                    _ => Nodes.Kind.Let
                };
            }
        }

        public Kind KindEnum { get; set; }
    }

    public enum Kind
    {
        Var,
        Let,
        Const,
    }
}