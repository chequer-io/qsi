namespace Qsi.Data
{
    public enum QsiAssignmentKind
    {
        /// <summary>=</summary>
        Equals,

        /// <summary>:=</summary>
        ColonEquals,

        /// <summary>+=</summary>
        AddEquals,

        /// <summary>-=</summary>
        SubtractEquals,

        /// <summary>*=</summary>
        MultiplyEquals,

        /// <summary>/=</summary>
        DivideEquals,

        /// <summary>%=</summary>
        ModEquals,

        /// <summary>&=</summary>
        BitwiseAndEquals,

        /// <summary>|=</summary>
        BitwiseOrEquals,

        /// <summary>^=</summary>
        BitwiseXorEquals
    }
}