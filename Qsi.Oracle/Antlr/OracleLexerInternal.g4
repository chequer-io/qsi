lexer grammar OracleLexerInternal;

channels { ERRORCHANNEL }
options { tokenVocab=predefined; }

MOD_SYMBOL:                         '%';
DOT_SYMBOL:                         '.';
COMMA_SYMBOL:                       ',';
SEMICOLON_SYMBOL:                   ';';
EQUAL_OPERATOR:                     '=';
DOUBLE_EQUAL_OPERATOR:              '==';
APPROXIMATELY_EQUAL_TO_OPERATOR:    '~=';
NOT_EQUAL_OPERATOR:                 '!=';
NOT_EQUAL2_OPERATOR:                '<>';
LOGICAL_AND_OPERATOR:               '&&';
BITWISE_AND_OPERATOR:               '&';
LESS_THEN_EQUAL_OPERATOR:           '<=';
GREATER_THEN_EQUAL_OPERATOR:        '>=';
LESS_THEN_OPERATOR:                 '<';
GREATER_THEN_OPERATOR:              '>';
BITWISE_XOR_OPERATOR:               '^';
BIT_XOR_OPERATOR:                   '^=';
PLUS_OPERATOR:                      '+';
MINUS_OPERATOR:                     '-';
MULT_OPERATOR:                      '*';
DIV_OPERATOR:                       '/';
HINT_OPEN_OPERATOR:                 '/*+';
HINT_CLOSE_OPERATOR:                '*/';
HINT_OPEN2_OPERATOR:                '--+';
CONCAT_OPERATOR:                    '||';
LEFT_BRACKET:                       '[';
LEFT_CURLY_BRACKET:                 '{';
OPEN_PAR_SYMBOL:                    '(';
RIGHT_BRACKET:                      ']';
RIGHT_CURLY_BRACKET:                '}';
CLOSE_PAR_SYMBOL:                   ')';
AT_SIGN_BRACKET:                    '@';
PIPE_SIGN:                          '|';
DOLLAR_SIGN:                        '$';
QMARK:                              '?';
LEFT_CURLY_BRACKET2:                '{-';
RIGHT_CURLY_BRACKET2:               '-}';
COLON:                              ':';
HASH_SYMBOL:                        '#';
JSON_PATH_SYMBOL:                   '$' DOT_SYMBOL;

S_SINGLE_QUOTE: SINGLE_QUOTE;

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];
fragment DIGIT: [0-9];
fragment NON_ZERO_DIGIT: [1-9];
fragment SINGLE_QUOTE: '\'';
fragment DOUBLE_QUOTE: '"';
fragment ALPHABET: [A-Za-z];

fragment LETTER_WHEN_UNQUOTED: DIGIT | LETTER_WHEN_UNQUOTED_NO_DIGIT;
fragment LETTER_WHEN_UNQUOTED_NO_DIGIT: [a-zA-Z_$#\u0080-\uffff];
fragment IDENTIFIER_FRAGMENT: LETTER_WHEN_UNQUOTED_NO_DIGIT LETTER_WHEN_UNQUOTED*;

HEXA1: X;
HEXA2: '0' X;

IDENTIFIER_OR_KEYWORD: IDENTIFIER_FRAGMENT { CategorizeIdentifier(); };

IDENTIFIER: DOUBLE_QUOTE ~[\r\n"]* DOUBLE_QUOTE;

S_INTEGER_WITHOUT_SIGN: DIGIT+;
S_NUMBER_WITHOUT_SIGN: (DIGIT+ '.'? DIGIT* | '.' DIGIT+) (E? ('+' | '-')? DIGIT+)? (F|D)?;

SINGLE_QUOTED_STRING: '\'' ( '\'\'' | ~['] )* '\'';

fragment QS_OTHER_CH: ~('<' | '{' | '[' | '(' | ' ' | '\t' | '\n' | '\r');
fragment QS_ANGLE: '<' .*? '>';
fragment QS_BRACE: '{' .*? '}';
fragment QS_BRACK: '[' .*? ']';
fragment QS_PAREN: '(' .*? ')';

QUOTED_STRING
    : Q SINGLE_QUOTE ( QS_ANGLE 
        | QS_BRACE 
        | QS_BRACK 
        | QS_PAREN 
        | QS_OTHER_CH ( {!isValidDelimiter()}? . )* QS_OTHER_CH 
        )
      SINGLE_QUOTE
    ;

//QUOTED_STRING
//    : Q '\'' ~[\u0000]*? '\''
//    ;

NATIONAL_STRING
    : N (
          SINGLE_QUOTED_STRING
        | QUOTED_STRING
    );

H_WHITESPACE:    [ \t\f\r\n] -> channel(HIDDEN); // Ignore whitespaces.
H_BLOCK_COMMENT: ( '/**/' | '/*' ~[+] .*? '*/') -> channel(HIDDEN);
H_COMMENT:       '--' ~[\r\n]* ('\r'? '\n' | EOF) -> channel(HIDDEN);