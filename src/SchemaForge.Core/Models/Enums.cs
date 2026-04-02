namespace SchemaForge.Core.Models;

public enum SqlDialect
{
    SQLite,
    PostgreSQL,
    MySQL,
    SqlServer
}

public enum ErdNotation
{
    CrowsFoot,
    IDEF1X,
    UML
}

public enum Cardinality
{
    OneToOne,
    OneToMany,
    ManyToOne,
    ManyToMany
}

public enum ReferentialAction
{
    NoAction,
    Cascade,
    SetNull,
    Restrict,
    SetDefault
}

public enum LogicalDataType
{
    Integer,
    BigInteger,
    SmallInteger,
    Decimal,
    Float,
    Boolean,
    Text,
    Varchar,
    Char,
    Date,
    DateTime,
    Time,
    Timestamp,
    Blob,
    Uuid,
    Json
}
