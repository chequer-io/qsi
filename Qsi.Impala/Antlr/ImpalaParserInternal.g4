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
    : lex+
    ;

lex
    : IntegerLiteral
    | DecimalLiteral
    | STRING_LITERAL
    | DOTDOTDOT
    | COLON
    | SEMICOLON
    | COMMA
    | DOT
    | STAR
    | LPAREN
    | RPAREN
    | LBRACKET
    | RBRACKET
    | DIVIDE
    | MOD
    | ADD
    | SUBTRACT
    | BITAND
    | BITOR
    | BITXOR
    | BITNOT
    | EQUAL
    | NOT
    | LESSTHAN
    | GREATERTHAN
    | UNMATCHED_STRING_LITERAL
    | NOTEQUAL
    ;