lexer grammar CqlLexer;

channels { ERRORCHANNEL }

K_SELECT:      S E L E C T;
K_FROM:        F R O M;
K_AS:          A S;
K_WHERE:       W H E R E;
K_AND:         A N D;
K_KEY:         K E Y;
K_KEYS:        K E Y S;
K_ENTRIES:     E N T R I E S;
K_FULL:        F U L L;
K_INSERT:      I N S E R T;
K_UPDATE:      U P D A T E;
K_WITH:        W I T H;
K_LIMIT:       L I M I T;
K_PER:         P E R;
K_PARTITION:   P A R T I T I O N;
K_USING:       U S I N G;
K_USE:         U S E;
K_DISTINCT:    D I S T I N C T;
K_COUNT:       C O U N T;
K_SET:         S E T;
K_BEGIN:       B E G I N;
K_UNLOGGED:    U N L O G G E D;
K_BATCH:       B A T C H;
K_APPLY:       A P P L Y;
K_TRUNCATE:    T R U N C A T E;
K_DELETE:      D E L E T E;
K_IN:          I N;
K_CREATE:      C R E A T E;
K_SCHEMA:      S C H E M A;
K_KEYSPACE:    ( K E Y S P A C E
                 | K_SCHEMA );
K_KEYSPACES:   K E Y S P A C E S;
K_COLUMNFAMILY:( C O L U M N F A M I L Y
                 | T A B L E );
K_TABLES:      ( C O L U M N F A M I L I E S
                 | T A B L E S );
K_MATERIALIZED:M A T E R I A L I Z E D;
K_VIEW:        V I E W;
K_INDEX:       I N D E X;
K_CUSTOM:      C U S T O M;
K_ON:          O N;
K_TO:          T O;
K_DROP:        D R O P;
K_PRIMARY:     P R I M A R Y;
K_INTO:        I N T O;
K_VALUES:      V A L U E S;
K_TIMESTAMP:   T I M E S T A M P;
K_TTL:         T T L;
K_CAST:        C A S T;
K_ALTER:       A L T E R;
K_RENAME:      R E N A M E;
K_ADD:         A D D;
K_TYPE:        T Y P E;
K_TYPES:       T Y P E S;
K_COMPACT:     C O M P A C T;
K_STORAGE:     S T O R A G E;
K_ORDER:       O R D E R;
K_BY:          B Y;
K_ASC:         A S C;
K_DESC:        D E S C;
K_ALLOW:       A L L O W;
K_FILTERING:   F I L T E R I N G;
K_IF:          I F;
K_IS:          I S;
K_CONTAINS:    C O N T A I N S;
K_GROUP:       G R O U P;
K_CLUSTER:     C L U S T E R;
K_INTERNALS:   I N T E R N A L S;
K_ONLY:        O N L Y;

K_GRANT:       G R A N T;
K_ALL:         A L L;
K_PERMISSION:  P E R M I S S I O N;
K_PERMISSIONS: P E R M I S S I O N S;
K_OF:          O F;
K_REVOKE:      R E V O K E;
K_MODIFY:      M O D I F Y;
K_AUTHORIZE:   A U T H O R I Z E;
K_DESCRIBE:    D E S C R I B E;
K_EXECUTE:     E X E C U T E;
K_NORECURSIVE: N O R E C U R S I V E;
K_MBEAN:       M B E A N;
K_MBEANS:      M B E A N S;

K_USER:        U S E R;
K_USERS:       U S E R S;
K_ROLE:        R O L E;
K_ROLES:       R O L E S;
K_SUPERUSER:   S U P E R U S E R;
K_NOSUPERUSER: N O S U P E R U S E R;
K_PASSWORD:    P A S S W O R D;
K_LOGIN:       L O G I N;
K_NOLOGIN:     N O L O G I N;
K_OPTIONS:     O P T I O N S;
K_ACCESS:      A C C E S S;
K_DATACENTERS: D A T A C E N T E R S;

K_CLUSTERING:  C L U S T E R I N G;
K_ASCII:       A S C I I;
K_BIGINT:      B I G I N T;
K_BLOB:        B L O B;
K_BOOLEAN:     B O O L E A N;
K_COUNTER:     C O U N T E R;
K_DECIMAL:     D E C I M A L;
K_DOUBLE:      D O U B L E;
K_DURATION:    D U R A T I O N;
K_FLOAT:       F L O A T;
K_INET:        I N E T;
K_INT:         I N T;
K_SMALLINT:    S M A L L I N T;
K_TINYINT:     T I N Y I N T;
K_TEXT:        T E X T;
K_UUID:        U U I D;
K_VARCHAR:     V A R C H A R;
K_VARINT:      V A R I N T;
K_TIMEUUID:    T I M E U U I D;
K_TOKEN:       T O K E N;
K_WRITETIME:   W R I T E T I M E;
K_DATE:        D A T E;
K_TIME:        T I M E;

K_NULL:        N U L L;
K_NOT:         N O T;
K_EXISTS:      E X I S T S;

K_MAP:         M A P;
K_LIST:        L I S T;
K_POSITIVE_NAN: N A N;
K_NEGATIVE_NAN: '-' N A N;
K_POSITIVE_INFINITY:    I N F I N I T Y;
K_NEGATIVE_INFINITY: '-' I N F I N I T Y;
K_TUPLE:       T U P L E;

K_TRIGGER:     T R I G G E R;
K_STATIC:      S T A T I C;
K_FROZEN:      F R O Z E N;

K_FUNCTION:    F U N C T I O N;
K_FUNCTIONS:   F U N C T I O N S;
K_AGGREGATE:   A G G R E G A T E;
K_AGGREGATES:  A G G R E G A T E S;
K_SFUNC:       S F U N C;
K_STYPE:       S T Y P E;
K_FINALFUNC:   F I N A L F U N C;
K_INITCOND:    I N I T C O N D;
K_RETURNS:     R E T U R N S;
K_CALLED:      C A L L E D;
K_INPUT:       I N P U T;
K_LANGUAGE:    L A N G U A G E;
K_OR:          O R;
K_REPLACE:     R E P L A C E;

K_JSON:        J S O N;
K_DEFAULT:     D E F A U L T;
K_UNSET:       U N S E T;
K_LIKE:        L I K E;

K_EXPR:        E X P R;

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

STRING_LITERAL
    : '\'' ('\'\'' | ~'\'')+ '\''
    // | $$..$$
    ;

QUOTED_NAME
    : '"' ('""' | ~'"')+ '"'
    ;

EMPTY_QUOTED_NAME
    : '"' '"'
    ;

fragment DIGIT
    : [0-9]
    ;

fragment LETTER
    : [A-Za-z]
    ;

fragment HEX
    : [0-9A-Fa-f]
    ;

fragment EXPONENT
    : E [+-]? DIGIT+
    ;

fragment DURATION_UNIT
    : Y
    | M O
    | W
    | D
    | H
    | M
    | S
    | M S
    | U S
    | '\u00B5' S
    | N S
    ;

RANGE
    : '..'
    ;

INTEGER
    : '-'? DIGIT+
    ;

QMARK
    : '?'
    ;

FLOAT
    : INTEGER '.' DIGIT* EXPONENT?
    | INTEGER EXPONENT
    ;

BOOLEAN
    : T R U E | F A L S E
    ;

DURATION
    : '-'? DIGIT+ DURATION_UNIT (DIGIT+ DURATION_UNIT)*
    | '-'? 'P' (DIGIT+ 'Y')? (DIGIT+ 'M')? (DIGIT+ 'D')? ('T' (DIGIT+ 'H')? (DIGIT+ 'M')? (DIGIT+ 'S')?)?
    | '-'? 'P' DIGIT+ 'W'
    | '-'? 'P' DIGIT DIGIT DIGIT DIGIT '-' DIGIT DIGIT '-' DIGIT DIGIT 'T' DIGIT DIGIT ':' DIGIT DIGIT ':' DIGIT DIGIT
    ;

IDENT
    : LETTER (LETTER | DIGIT | '_')*
    ;

HEXNUMBER
    : '0' X HEX*
    ;

UUID
    : HEX HEX HEX HEX HEX HEX HEX HEX '-'
      HEX HEX HEX HEX '-'
      HEX HEX HEX HEX '-'
      HEX HEX HEX HEX '-'
      HEX HEX HEX HEX HEX HEX HEX HEX HEX HEX HEX HEX
    ;

WS
    : [ \t\r\n]+ -> channel(HIDDEN)
    ;

COMMENT
    : ('--' | '//') ~[\r\n]* ('\r'? '\n' | EOF) -> channel(HIDDEN)
    ;

MULTILINE_COMMENT
    : '/*' .*? '*/' -> channel(HIDDEN)
    ;

PLUS:                                '+';
MINUS:                               '-';
DIVIDE:                              '/';
MULTIPLY:                            '*';
MODULUS:                             '%';

DOT:                                 '.';
L_BRACKET:                           '<';
R_BRACKET:                           '>';
LR_BRACKET:                          '(';
RR_BRACKET:                          ')';
LC_BRACKET:                          '{';
RC_BRACKET:                          '}';
LS_BRACKET:                          '[';
RS_BRACKET:                          ']';
COMMA:                               ',';
SEMI:                                ';';
COLON:                               ':';
AT_SIGN:                             '@';

EQUAL:                               '=';
LESS_THEN_EQUAL:                    '<=';
GREATER_THEN_EQUAL:                 '>=';
NOT_EQUAL:                          '!=';
ADDITION_ASSIGNMENT:                '+=';
SUBRACT_ASSIGNMENT:                 '-=';

MINUSMINUS:                         '--';

// Last tokens must generate Errors

ERROR_RECONGNIGION: . -> channel(ERRORCHANNEL);