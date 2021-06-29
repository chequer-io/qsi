parser grammar ImpalaParserInternal;

options { 
    tokenVocab=ImpalaLexerInternal;
}

@header {
    using Qsi.Data;
    using Qsi.Tree;
    using Qsi.Utilities;
}

root
    : HELLO
    ;