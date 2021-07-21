lexer grammar ImpalaLexerInternal;

options {
    superClass = ImpalaBaseLexer;
}

tokens {
    UNMATCHED_STRING_LITERAL,
    INTEGER_LITERAL,
    NUMERIC_OVERFLOW,
    DECIMAL_LITERAL,
    EMPTY_IDENT,
    IDENT,
    STRING_LITERAL,
    COMMENTED_PLAN_HINT_START,
    COMMENTED_PLAN_HINT_END,
    UNEXPECTED_CHAR,
    KW_AND,
    KW_DOUBLE,
    KW_INT,
    UNUSED_RESERVED_WORD
}

KW_AND1: '&&'  -> type(KW_AND);
KW_AND2: A N D -> type(KW_AND);
KW_ADD: A D D;
KW_AGGREGATE: A G G R E G A T E;
KW_ALL: A L L;
KW_ALTER: A L T E R;
KW_ANALYTIC: A N A L Y T I C;
KW_ANTI: A N T I;
KW_API_VERSION: A P I '_' V E R S I O N;
KW_ARRAY: A R R A Y;
KW_AS: A S;
KW_ASC: A S C;
KW_AUTHORIZATION: A U T H O R I Z A T I O N;
KW_AVRO: A V R O;
KW_BETWEEN: B E T W E E N;
KW_BIGINT: B I G I N T;
KW_BINARY: B I N A R Y;
KW_BLOCKSIZE: B L O C K '_' S I Z E;
KW_BOOLEAN: B O O L E A N;
KW_BY: B Y;
KW_CACHED: C A C H E D;
KW_CASCADE: C A S C A D E;
KW_CASE: C A S E;
KW_CAST: C A S T;
KW_CHANGE: C H A N G E;
KW_CHAR: C H A R;
KW_CLASS: C L A S S;
KW_CLOSE_FN: C L O S E '_' F N;
KW_COLUMN: C O L U M N;
KW_COLUMNS: C O L U M N S;
KW_COMMENT: C O M M E N T;
KW_COMPRESSION: C O M P R E S S I O N;
KW_COMPUTE: C O M P U T E;
KW_CONSTRAINT: C O N S T R A I N T;
KW_COPY: C O P Y;
KW_CREATE: C R E A T E;
KW_CROSS: C R O S S;
KW_CUBE: C U B E;
KW_CURRENT: C U R R E N T;
KW_DATA: D A T A;
KW_DATABASE: D A T A B A S E;
KW_DATABASES: D A T A B A S E S;
KW_DATE: D A T E;
KW_DATETIME: D A T E T I M E;
KW_DECIMAL: D E C I M A L;
KW_DEFAULT: D E F A U L T;
KW_DELETE: D E L E T E;
KW_DELIMITED: D E L I M I T E D;
KW_DESC: D E S C;
KW_DESCRIBE: D E S C R I B E;
KW_DISABLE: D I S A B L E;
KW_DISTINCT: D I S T I N C T;
KW_DIV: D I V;
KW_DOUBLE1: D O U B L E -> type(KW_DOUBLE);
KW_DOUBLE2: R E A L     -> type(KW_DOUBLE);
KW_DROP: D R O P;
KW_ELSE: E L S E;
KW_ENABLE: E N A B L E;
KW_ENCODING: E N C O D I N G;
KW_END: E N D;
KW_ESCAPED: E S C A P E D;
KW_EXCEPT: E X C E P T;
KW_EXISTS: E X I S T S;
KW_EXPLAIN: E X P L A I N;
KW_EXTENDED: E X T E N D E D;
KW_EXTERNAL: E X T E R N A L;
KW_FALSE: F A L S E;
KW_FIELDS: F I E L D S;
KW_FILEFORMAT: F I L E F O R M A T;
KW_FILES: F I L E S;
KW_FINALIZE_FN: F I N A L I Z E '_' F N;
KW_FIRST: F I R S T;
KW_FLOAT: F L O A T;
KW_FOLLOWING: F O L L O W I N G;
KW_FOR: F O R;
KW_FOREIGN: F O R E I G N;
KW_FORMAT: F O R M A T;
KW_FORMATTED: F O R M A T T E D;
KW_FROM: F R O M;
KW_FULL: F U L L;
KW_FUNCTION: F U N C T I O N;
KW_FUNCTIONS: F U N C T I O N S;
KW_GRANT: G R A N T;
KW_GROUP: G R O U P;
KW_GROUPING: G R O U P I N G;
KW_HASH: H A S H;
KW_HAVING: H A V I N G;
KW_HUDIPARQUET: H U D I P A R Q U E T;
KW_ICEBERG: I C E B E R G;
KW_IF: I F;
KW_IGNORE: I G N O R E;
KW_ILIKE: I L I K E;
KW_IN: I N;
KW_INCREMENTAL: I N C R E M E N T A L;
KW_INIT_FN: I N I T '_' F N;
KW_INNER: I N N E R;
KW_INPATH: I N P A T H;
KW_INSERT: I N S E R T;
KW_INT1: I N T         -> type(KW_INT);
KW_INT2: I N T E G E R -> type(KW_INT);
KW_INTERMEDIATE: I N T E R M E D I A T E;
KW_INTERSECT: I N T E R S E C T;
KW_INTERVAL: I N T E R V A L;
KW_INTO: I N T O;
KW_INVALIDATE: I N V A L I D A T E;
KW_IREGEXP: I R E G E X P;
KW_IS: I S;
KW_JOIN: J O I N;
KW_KUDU: K U D U;
KW_LAST: L A S T;
KW_LEFT: L E F T;
KW_LEXICAL: L E X I C A L;
KW_LIKE: L I K E;
KW_LIMIT: L I M I T;
KW_LINES: L I N E S;
KW_LOAD: L O A D;
KW_LOCATION: L O C A T I O N;
KW_MANAGED_LOCATION: M A N A G E D L O C A T I O N;
KW_MAP: M A P;
KW_MERGE_FN: M E R G E '_' F N;
KW_METADATA: M E T A D A T A;
KW_MINUS: M I N U S;
KW_NORELY: N O R E L Y;
KW_NOT: N O T;
KW_NOVALIDATE: N O V A L I D A T E;
KW_NULL: N U L L;
KW_NULLS: N U L L S;
KW_OFFSET: O F F S E T;
KW_ON: O N;
KW_OR: O R;
KW_LOGICAL_OR: '||';
KW_ORC: O R C;
KW_ORDER: O R D E R;
KW_OUTER: O U T E R;
KW_OVER: O V E R;
KW_OVERWRITE: O V E R W R I T E;
KW_PARQUET: P A R Q U E T;
KW_PARQUETFILE: P A R Q U E T F I L E;
KW_PARTITION: P A R T I T I O N;
KW_PARTITIONED: P A R T I T I O N E D;
KW_PARTITIONS: P A R T I T I O N S;
KW_PRECEDING: P R E C E D I N G;
KW_PREPARE_FN: P R E P A R E '_' F N;
KW_PRIMARY: P R I M A R Y;
KW_PRODUCED: P R O D U C E D;
KW_PURGE: P U R G E;
KW_RANGE: R A N G E;
KW_RCFILE: R C F I L E;
KW_RECOVER: R E C O V E R;
KW_REFERENCES: R E F E R E N C E S;
KW_REFRESH: R E F R E S H;
KW_REGEXP: R E G E X P;
KW_RELY: R E L Y;
KW_RENAME: R E N A M E;
KW_REPEATABLE: R E P E A T A B L E;
KW_REPLACE: R E P L A C E;
KW_REPLICATION: R E P L I C A T I O N;
KW_RESTRICT: R E S T R I C T;
KW_RETURNS: R E T U R N S;
KW_REVOKE: R E V O K E;
KW_RIGHT: R I G H T;
KW_RLIKE: R L I K E;
KW_ROLE: R O L E;
KW_ROLES: R O L E S;
KW_ROLLUP: R O L L U P;
KW_ROW: R O W;
KW_ROWS: R O W S;
KW_SCHEMA: S C H E M A;
KW_SCHEMAS: S C H E M A S;
KW_SELECT: S E L E C T;
KW_SEMI: S E M I;
KW_SEQUENCEFILE: S E Q U E N C E F I L E;
KW_SERDEPROPERTIES: S E R D E P R O P E R T I E S;
KW_SERIALIZE_FN: S E R I A L I Z E '_' F N;
KW_SET: S E T;
KW_SETS: S E T S;
KW_SHOW: S H O W;
KW_SMALLINT: S M A L L I N T;
KW_SORT: S O R T;
KW_SPEC: S P E C;
KW_STATS: S T A T S;
KW_STORED: S T O R E D;
KW_STRAIGHT_JOIN: S T R A I G H T '_' J O I N;
KW_STRING: S T R I N G;
KW_STRUCT: S T R U C T;
KW_SYMBOL: S Y M B O L;
KW_TABLE: T A B L E;
KW_TABLES: T A B L E S;
KW_TABLESAMPLE: T A B L E S A M P L E;
KW_TBLPROPERTIES: T B L P R O P E R T I E S;
KW_TERMINATED: T E R M I N A T E D;
KW_TEXTFILE: T E X T F I L E;
KW_THEN: T H E N;
KW_TIMESTAMP: T I M E S T A M P;
KW_TINYINT: T I N Y I N T;
KW_TO: T O;
KW_TRUE: T R U E;
KW_TRUNCATE: T R U N C A T E;
KW_UNBOUNDED: U N B O U N D E D;
KW_UNCACHED: U N C A C H E D;
KW_UNION: U N I O N;
KW_UNKNOWN: U N K N O W N;
KW_UNSET: U N S E T;
KW_UPDATE: U P D A T E;
KW_UPDATE_FN: U P D A T E '_' F N;
KW_UPSERT: U P S E R T;
KW_USE: U S E;
KW_USING: U S I N G;
KW_VALIDATE: V A L I D A T E;
KW_VALUES: V A L U E S;
KW_VARCHAR: V A R C H A R;
KW_VIEW: V I E W;
KW_WHEN: W H E N;
KW_WHERE: W H E R E;
KW_WITH: W I T H;
KW_ZORDER: Z O R D E R;

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

COMMENT
    : '--'
      { Hint == default }?
      (~[\r\n]* ('\r'? '\n' | { InputStream.LA(1) == Eof }?))
      { !IsCommentPlanHint() }?
      -> skip
    ;

MULTILINE_COMMENT
    : '/*'
      { Hint == default }?
      (.*? '*/')
      { !IsCommentPlanHint() }?
      -> skip
    ;

CommentedHintBegin
    : '/*'
      { Hint == default }?
      (' '* '+')
      { Hint = LexHint.MultiLineComment; }
      -> type(COMMENTED_PLAN_HINT_START)
    ;

EolHintBegin
    : '--'
      { Hint == default }?
      (' '* '+')
      { Hint = LexHint.SingleLineComment; }
      -> type(COMMENTED_PLAN_HINT_START)
    ;

CommentedHintEnd
    : '*/'
      { Hint == LexHint.MultiLineComment }?
      { Hint = default; }
      -> type(COMMENTED_PLAN_HINT_END)
    ;

LineTerminator
    : ('\r'? '\n' | EOF)
      { Hint == LexHint.SingleLineComment }?
      { Hint = default; }
      -> type(COMMENTED_PLAN_HINT_END)
    ;

// Order of rules to resolve ambiguity:
// The rule for recognizing integer literals must come before the rule for
// double literals to, e.g., recognize "1234" as an integer literal.
// The rule for recognizing double literals must come before the rule for
// identifiers to, e.g., recognize "1e6" as a double literal.
INTEGER_LITERAL: DIGIT+;
fragment FLit1: [0-9]+ '.' [0-9]*;
fragment FLit2: '.' [0-9]+;
fragment FLit3: [0-9]+;
fragment Exponent: E? ('+' | '-')? [0-9]+;
DECIMAL_LITERAL: (FLit1 | FLit2 | FLit3) Exponent?;

fragment DIGIT: [0-9];
fragment LETTER: [a-zA-Z\u0080-\u00FF_];
fragment LETTER_OR_DIGIT: LETTER | DIGIT;

fragment Identifier: DIGIT* LETTER LETTER_OR_DIGIT*;

IdentifierOrKw: (Identifier | KW_AND1 | KW_LOGICAL_OR) { CategorizeIdentifier(); };

EMPTY_IDENT: '``';
IDENT: '`' ('\\' . | ~[\\`])+? '`';

STRING_LITERAL
    : '\'' ('\\' . | ~[\\'])*? '\''
    | '"' ('\\' . | ~[\\"])*? '"'
    ;

// Put '...' before '.'
DOTDOTDOT: '...';

// single-character tokens
COLON: ':';
SEMICOLON: ';';
COMMA: ',';
DOT: '.';
STAR: '*';
LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';
DIVIDE: '/';
MOD: '%';
ADD: '+';
SUBTRACT: '-';
BITAND: '&';
BITOR: '|';
BITXOR: '^';
BITNOT: '~';
EQUAL: '=';
NOT: '!';
LESSTHAN: '<';
GREATERTHAN: '>';
UNMATCHED_STRING_LITERAL: '"' | '\'' | '`';
NOTEQUAL: '!=';
//INTEGER_LITERAL: "INTEGER LITERAL";
//NUMERIC_OVERFLOW: "NUMERIC OVERFLOW";
//DECIMAL_LITERAL: "DECIMAL LITERAL";
//EMPTY_IDENT: "EMPTY IDENTIFIER";
//IDENT: "IDENTIFIER";
//STRING_LITERAL: "STRING LITERAL";
//COMMENTED_PLAN_HINT_START: "COMMENTED_PLAN_HINT_START";
//COMMENTED_PLAN_HINT_END: "COMMENTED_PLAN_HINT_END";
//UNEXPECTED_CHAR: "Unexpected character";

WS: [ \t\f\r\n] -> skip;

UNEXPECTED_CHAR: .;
