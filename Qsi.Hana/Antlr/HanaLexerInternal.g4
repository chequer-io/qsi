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
K_COUNT:                               C O U N T;
K_CORR:                                C O R R;
K_CORR_SPEARMAN:                       C O R R '_' S P E A R M A N;
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
K_MIN:                                 M I N;
K_MAX:                                 M A X;
K_MEDIAN:                              M E D I A N;
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
K_STRING_AGG:                          S T R I N G '_' A G G;
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
K_THEN:                                T H E N;
K_FIRST:                               F I R S T;
K_AND:                                 A N D;
K_OR:                                  O R;
K_NOT:                                 N O T;
K_SUM:                                 S U M;
K_AVG:                                 A V G;
K_STDDEV:                              S T D D E V;
K_VAR:                                 V A R;
K_STDDEV_POP:                          S T D D E V '_' P O P;
K_VAR_POP:                             V A R '_' P O P;
K_STDDEV_SAMP:                         S T D D E V '_' S A M P;
K_VAR_SAMP:                            V A R '_' S A M P;
K_ASC:                                 A S C;
K_DESC:                                D E S C;
K_BY:                                  B Y;
K_NULLS:                               N U L L S;
K_LAST:                                L A S T;
K_ONE:                                 O N E;
K_MANY:                                M A N Y;
K_TO:                                  T O;
K_HINT:                                H I N T;
K_OFF:                                 O F F;
K_AUTOMATIC:                           A U T O M A T I C;
K_FULLTEXT:                            F U L L T E X T;
K_ANY:                                 A N Y;
K_SOME:                                S O M E;
K_EMPTY:                               E M P T Y;
K_NOTHING:                             N O T H I N G;
K_SPECIFIED:                           S P E C I F I E D;
K_EXACT:                               E X A C T;
K_FUZZY:                               F U Z Z Y;
K_LINGUISTIC:                          L I N G U I S T I C;
K_WEIGHT:                              W E I G H T;
K_EXISTS:                              E X I S T S;
K_LIKE:                                L I K E;
K_ESCAPE:                              E S C A P E;
K_FLAG:                                F L A G;
K_LIKE_REGEXPR:                        L I K E '_' R E G E X P R;
K_MEMBER:                              M E M B E R;
K_OF:                                  O F;
K_BETWEEN:                             B E T W E E N;
K_CONTAINS:                            C O N T A I N S;
K_LANGUAGE:                            L A N G U A G E;

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
fragment LETTER: [a-zA-Z];
fragment LETTER_OR_DIGIT: [a-zA-Z0-9];
fragment SIGN: ('+'|'-');
fragment UNICODE_LETTER: [\p{L}];

SEMI:                 ';';
EQUAL:                '=';
LESS_THEN_EQUAL:     '<=';
GREATER_THEN_EQUAL:  '>=';
NOT_EQUAL1:          '!=';
NOT_EQUAL2:          '<>';
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
DOT:                  '.';
PIPE_SIGN:            '|';
CONCAT_SIGN:         '||';
RIGHT_BRACKET:        ']';
RIGHT_CURLY_BRACKET:  '}';
CLOSE_PAR_SYMBOL:     ')';
SINGLE_QUOTE:        '\'';
UNDERSCORE:           '_';
PLUS:                 '+';
MINUS:                '-';
MULTIPLY:             '*';
DIVIDE:               '/';
MODULUS:              '%';
COLON:                ':';

fragment APPROXIMATE_NUMERIC_LITERAL
    : EXACT_NUMERIC_LITERAL 'E' SIGNED_INTEGER
    ;

fragment EXACT_NUMERIC_LITERAL
    : UNSIGNED_INTEGER ('.' UNSIGNED_INTEGER?)? 
    | '.' UNSIGNED_INTEGER
    ;

fragment UNSIGNED_NUMERIC_LITERAL
    : EXACT_NUMERIC_LITERAL 
    | APPROXIMATE_NUMERIC_LITERAL
    ;

fragment SIGNED_NUMERIC_LITERAL
    : SIGN? UNSIGNED_NUMERIC_LITERAL
    ;

UNQUOTED_IDENTIFIER
    : (LETTER | '_') (LETTER | DIGIT | '_' | '#' | '$')*
    ;

QUOTED_IDENTIFIER
    : '"' ('""' | ~'"')* '"'
    ;

UNICODE_IDENTIFIER
    : (UNICODE_LETTER | '_') (UNICODE_LETTER | DIGIT | DIGIT | '_' | '#' | '$')*
    ;

STRING_LITERAL
    : '\'' ('\'\'' | ~'\'')* '\''
    ;

BOOLEAN_LITERAL
    : K_TRUE
    | K_FALSE
    ;

NUMERIC_LITERAL
    : SIGNED_NUMERIC_LITERAL
    | SIGNED_INTEGER
    ;

SIGNED_INTEGER
    : SIGN? UNSIGNED_INTEGER
    ;

UNSIGNED_INTEGER
    : DIGIT+
    ;

WS: [ \t\r\n]+                                     -> channel(HIDDEN);
COMMENT: ('--' | '//') ~[\r\n]* ('\r'? '\n' | EOF) -> channel(HIDDEN);
POUND_COMMENT:     '#' ~([\n\r])*                  -> channel(HIDDEN);
MULTILINE_COMMENT: '/*' .*? '*/'                   -> channel(HIDDEN);
