lexer grammar HanaLexerInternal;

K_ALL:                  A L L;
K_AND:                  A N D;
K_ANY:                  A N Y;
K_APPLICATION_TIME:     A P P L I C A T I O N '_' T I M E;
K_AS:                   A S;
K_ASC:                  A S C;
K_AUTOMATIC:            A U T O M A T I C;
K_AVG:                  A V G;
K_BALANCE:              B A L A N C E;
K_BERNOULLI:            B E R N O U L L I;
K_BEST:                 B E S T;
K_BETWEEN:              B E T W E E N;
K_BY:                   B Y;
K_CASE:                 C A S E;
K_CLOB:                 C L O B;
K_COLLATE:              C O L L A T E;
K_CONTAINS:             C O N T A I N S;
K_CORR:                 C O R R;
K_CORR_SPEARMAN:        C O R R '_' S P E A R M A N;
K_COUNT:                C O U N T;
K_CUBE:                 C U B E;
K_DATA_TRANSFER_COST:   D A T A '_' T R A N S F E R '_' C O S T;
K_DESC:                 D E S C;
K_DISTINCT:             D I S T I N C T;
K_ELSE:                 E L S E;
K_EMPTY:                E M P T Y;
K_END:                  E N D;
K_ESCAPE:               E S C A P E;
K_EXACT:                E X A C T;
K_EXCEPT:               E X C E P T;
K_EXISTS:               E X I S T S;
K_FALSE:                F A L S E;
K_FILL:                 F I L L;
K_FIRST:                F I R S T;
K_FLAG:                 F L A G;
K_FOR:                  F O R;
K_FROM:                 F R O M;
K_FULLTEXT:             F U L L T E X T;
K_FUZZY:                F U Z Z Y;
K_GROUP:                G R O U P;
K_GROUPING:             G R O U P I N G;
K_HAVING:               H A V I N G;
K_HINT:                 H I N T;
K_IGNORE:               I G N O R E;
K_IN:                   I N;
K_INTERSECT:            I N T E R S E C T;
K_INTO:                 I N T O;
K_IS:                   I S;
K_JOIN:                 J O I N;
K_JSON:                 J S O N;
K_LANGUAGE:             L A N G U A G E;
K_LAST:                 L A S T;
K_LIKE:                 L I K E;
K_LIKE_REGEXPR:         L I K E '_' R E G E X P R;
K_LIMIT:                L I M I T;
K_LINGUISTIC:           L I N G U I S T I C;
K_LOCK:                 L O C K;
K_LOCKED:               L O C K E D;
K_MANY:                 M A N Y;
K_MATCHES:              M A T C H E S;
K_MAX:                  M A X;
K_MEDIAN:               M E D I A N;
K_MEMBER:               M E M B E R;
K_MIN:                  M I N;
K_MULTIPLE:             M U L T I P L E;
K_NCLOB:                N C L O B;
K_NO_ROUTE_TO:          N O '_' R O U T E '_' T O;
K_NOT:                  N O T;
K_NOTHING:              N O T H I N G;
K_NOWAIT:               N O W A I T;
K_NULL:                 N U L L;
K_NULLS:                N U L L S;
K_NVARCHAR:             N V A R C H A R;
K_OF:                   O F;
K_OFF:                  O F F;
K_OFFSET:               O F F S E T;
K_ON:                   O N;
K_ONE:                  O N E;
K_OR:                   O R;
K_ORDER:                O R D E R;
K_OVERVIEW:             O V E R V I E W;
K_PARTITION:            P A R T I T I O N;
K_PREFIX:               P R E F I X;
K_RESULT:               R E S U L T;
K_RESULTSETS:           R E S U L T S E T S;
K_RETURNS:              R E T U R N S;
K_ROLLUP:               R O L L U P;
K_ROUTE_BY:             R O U T E '_' B Y;
K_ROUTE_BY_CARDINALITY: R O U T E '_' B Y '_' C A R D I N A L I T Y;
K_ROUTE_TO:             R O U T E '_' T O;
K_ROWCOUNT:             R O W C O U N T;
K_SELECT:               S E L E C T;
K_SETS:                 S E T S;
K_SHARE:                S H A R E;
K_SOME:                 S O M E;
K_SORT:                 S O R T;
K_SPECIFIED:            S P E C I F I E D;
K_STDDEV:               S T D D E V;
K_STDDEV_POP:           S T D D E V '_' P O P;
K_STDDEV_SAMP:          S T D D E V '_' S A M P;
K_STRING_AGG:           S T R I N G '_' A G G;
K_STRUCTURED:           S T R U C T U R E D;
K_SUBTOTAL:             S U B T O T A L;
K_SUM:                  S U M;
K_SYSTEM:               S Y S T E M;
K_SYSTEM_TIME:          S Y S T E M '_' T I M E;
K_TABLESAMPLE:          T A B L E S A M P L E;
K_TEXT_FILTER:          T E X T '_' F I L T E R;
K_THEN:                 T H E N;
K_TO:                   T O;
K_TOP:                  T O P;
K_TOTAL:                T O T A L;
K_TRUE:                 T R U E;
K_UNION:                U N I O N;
K_UP:                   U P;
K_UPDATE:               U P D A T E;
K_USING:                U S I N G;
K_UTCTIMESTAMP:         U T C T I M E S T A M P;
K_VAR:                  V A R;
K_VAR_POP:              V A R '_' P O P;
K_VAR_SAMP:             V A R '_' S A M P;
K_VARCHAR:              V A R C H A R;
K_WAIT:                 W A I T;
K_WEIGHT:               W E I G H T;
K_WHEN:                 W H E N;
K_WHERE:                W H E R E;
K_WITH:                 W I T H;
K_XML:                  X M L;

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
