lexer grammar HanaLexerInternal;

K_ALL:                                 A L L;
K_ALTER:                               A L T E R;
K_AS:                                  A S;
K_BEFORE:                              B E F O R E;
K_BEGIN:                               B E G I N;
K_BOTH:                                B O T H;
K_CASE:                                C A S E;
K_CHAR:                                C H A R;
K_CONDITION:                           C O N D I T I O N;
K_CONNECT:                             C O N N E C T;
K_CROSS:                               C R O S S;
K_CUBE:                                C U B E;
K_CURRENT_CONNECTION:                  C U R R E N T '_' C O N N E C T I O N;
K_CURRENT_DATE:                        C U R R E N T '_' D A T E;
K_CURRENT_SCHEMA:                      C U R R E N T '_' S C H E M A;
K_CURRENT_TIME:                        C U R R E N T '_' T I M E;
K_CURRENT_TIMESTAMP:                   C U R R E N T '_' T I M E S T A M P;
K_CURRENT_TRANSACTION_ISOLATION_LEVEL: C U R R E N T '_' T R A N S A C T I O N '_' I S O L A T I O N '_' L E V E L;
K_CURRENT_USER:                        C U R R E N T '_' U S E R;
K_CURRENT_UTCDATE:                     C U R R E N T '_' U T C D A T E;
K_CURRENT_UTCTIME:                     C U R R E N T '_' U T C T I M E;
K_CURRENT_UTCTIMESTAMP:                C U R R E N T '_' U T C T I M E S T A M P;
K_CURRVAL:                             C U R R V A L;
K_CURSOR:                              C U R S O R;
K_DECLARE:                             D E C L A R E;
K_DISTINCT:                            D I S T I N C T;
K_ELSE:                                E L S E;
K_ELSEIF:                              E L S E I F;
K_END:                                 E N D;
K_EXCEPT:                              E X C E P T;
K_EXCEPTION:                           E X C E P T I O N;
K_EXEC:                                E X E C;
K_FALSE:                               F A L S E;
K_FOR:                                 F O R;
K_FROM:                                F R O M;
K_FULL:                                F U L L;
K_GROUP:                               G R O U P;
K_HAVING:                              H A V I N G;
K_IF:                                  I F;
K_IN:                                  I N;
K_INNER:                               I N N E R;
K_INOUT:                               I N O U T;
K_INTERSECT:                           I N T E R S E C T;
K_INTO:                                I N T O;
K_IS:                                  I S;
K_JOIN:                                J O I N;
K_LEADING:                             L E A D I N G;
K_LEFT:                                L E F T;
K_LIMIT:                               L I M I T;
K_LOOP:                                L O O P;
K_MINUS:                               M I N U S;
K_NATURAL:                             N A T U R A L;
K_NCHAR:                               N C H A R;
K_NEXTVAL:                             N E X T V A L;
K_NULL:                                N U L L;
K_ON:                                  O N;
K_ORDER:                               O R D E R;
K_OUT:                                 O U T;
K_PRIOR:                               P R I O R;
K_RETURN:                              R E T U R N;
K_RETURNS:                             R E T U R N S;
K_REVERSE:                             R E V E R S E;
K_RIGHT:                               R I G H T;
K_ROLLUP:                              R O L L U P;
K_ROWID:                               R O W I D;
K_SELECT:                              S E L E C T;
K_SESSION_USER:                        S E S S I O N '_' U S E R;
K_SET:                                 S E T;
K_SQL:                                 S Q L;
K_START:                               S T A R T;
K_SYSUUID:                             S Y S U U I D;
K_TABLESAMPLE:                         T A B L E S A M P L E;
K_TOP:                                 T O P;
K_TRAILING:                            T R A I L I N G;
K_TRUE:                                T R U E;
K_UNION:                               U N I O N;
K_UNKNOWN:                             U N K N O W N;
K_USING:                               U S I N G;
K_UTCTIMESTAMP:                        U T C T I M E S T A M P;
K_VALUES:                              V A L U E S;
K_WHEN:                                W H E N;
K_WHERE:                               W H E R E;
K_WHILE:                               W H I L E;
K_WITH:                                W I T H;
K_ROUTE_TO:                            R O U T E '_' T O; 
K_NO_ROUTE_TO:                         N O '_' R O U T E '_' T O;
K_ROUTE_BY:                            R O U T E '_' B Y; 
K_ROUTE_BY_CARDINALITY:                R O U T E '_' B Y '_' C A R D I N A L I T Y;
K_DATA_TRANSFER_COST:                  D A T A '_' T R A N S F E R '_' C O S T;
K_WITHHINT:                            W I T H H I N T;

fragment A: ('a'|'A');
fragment B: ('b'|'B');
fragment C: ('c'|'C');
fragment D: ('d'|'D');
fragment E: ('e'|'E');
fragment F: ('f'|'F');
fragment G: ('g'|'G');
fragment H: ('h'|'H');
fragment I: ('i'|'I');
fragment J: ('j'|'J');
fragment K: ('k'|'K');
fragment L: ('l'|'L');
fragment M: ('m'|'M');
fragment N: ('n'|'N');
fragment O: ('o'|'O');
fragment P: ('p'|'P');
fragment Q: ('q'|'Q');
fragment R: ('r'|'R');
fragment S: ('s'|'S');
fragment T: ('t'|'T');
fragment U: ('u'|'U');
fragment V: ('v'|'V');
fragment W: ('w'|'W');
fragment X: ('x'|'X');
fragment Y: ('y'|'Y');
fragment Z: ('z'|'Z');

fragment DIGIT: [0-9];
fragment LETTER: [A-Za-z];
fragment LETTER_OR_DIGIT: [A-Za-z0-9];
fragment SIGN: ('+'|'-');

SEMI:                 ';';
EQUAL:                '=';
LESS_THEN_EQUAL:     '<=';
GREATER_THEN_EQUAL:  '>=';
NOT_EQUAL:           '!=';
ADDITION_ASSIGNMENT: '+=';
SUBRACT_ASSIGNMENT:  '-=';
MINUSMINUS:          '--';
COMMA:                ',';
DOLLAR_SIGN:          '$';
DOUBLE_QUOTE:         '"';
GREATER_THAN_SIGN:    '>';
HASH_SYMBOL:          '#';
LEFT_BRACKET:         '[';
LEFT_CURLY_BRACKET:   '{';
OPEN_PAR_SYMBOL:      '(';
LOWER_THAN_SIGN:      '<';
PERIOD:               '.';
PIPE_SIGN:            '|';
RIGHT_BRACKET:        ']';
RIGHT_CURLY_BRACKET:  '}';
CLOSE_PAR_SYMBOL:     ')';
SINGLE_QUOTE:        '\'';
UNDERSCORE:           '_';
MULTIPLY:             '*';

fragment UNICODE_NAME
    : [\p{L}]+
    ;

APOSTROPHE: SINGLE_QUOTE;

APPROXIMATE_NUMERIC_LITERAL
    : MANTISSA 'E' EXPONENT
    ;

EXACT_NUMERIC_LITERAL
    : UNSIGNED_INTEGER ('.' UNSIGNED_INTEGER?)? 
    | '.' UNSIGNED_INTEGER
    ;

EXPONENT
    : SIGNED_INTEGER
    ;

IDENTIFIER
    : SIMPLE_IDENTIFIER
    | SPECIAL_IDENTIFIER
    ;

MANTISSA
    : EXACT_NUMERIC_LITERAL
    ;

NUMERIC_LITERAL
    : SIGNED_NUMERIC_LITERAL 
    | SIGNED_INTEGER
    ;

PORT_NUMBER
    : UNSIGNED_INTEGER
    ;

SCHEMA_NAME
    : UNICODE_NAME
    ;

SIMPLE_IDENTIFIER
    : (LETTER | '_') (LETTER | DIGIT | '_' | '#' | '$')*
    ;

SPECIAL_IDENTIFIER
    : '"' ('""' | ~'"')* '"'
    ;

STRING_LITERAL
    : '\'' ('\'\'' | ~'\'')* '\''
    ;

SIGNED_INTEGER
    : SIGN? UNSIGNED_INTEGER
    ;

SIGNED_NUMERIC_LITERAL
    : SIGN? UNSIGNED_NUMERIC_LITERAL
    ;

UNSIGNED_INTEGER
    : DIGIT+
    ;

UNSIGNED_NUMERIC_LITERAL
    : EXACT_NUMERIC_LITERAL 
    | APPROXIMATE_NUMERIC_LITERAL
    ;

USER_NAME
    : UNICODE_NAME
    ;

WS: [ \t\r\n]+                                     -> channel(HIDDEN);
COMMENT: ('--' | '//') ~[\r\n]* ('\r'? '\n' | EOF) -> channel(HIDDEN);
POUND_COMMENT:     '#' ~([\n\r])*                  -> channel(HIDDEN);
MULTILINE_COMMENT: '/*' .*? '*/'                   -> channel(HIDDEN);
