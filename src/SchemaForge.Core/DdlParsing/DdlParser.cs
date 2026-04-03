using System.Text.RegularExpressions;
using SchemaForge.Core.Models;

namespace SchemaForge.Core.DdlParsing;

public static class DdlParser
{
    private const double GridSpacing = 300;

    private static readonly Dictionary<string, LogicalDataType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["INT"] = LogicalDataType.Integer,
        ["INTEGER"] = LogicalDataType.Integer,
        ["SERIAL"] = LogicalDataType.Integer,
        ["BIGINT"] = LogicalDataType.BigInteger,
        ["BIGSERIAL"] = LogicalDataType.BigInteger,
        ["SMALLINT"] = LogicalDataType.SmallInteger,
        ["TINYINT"] = LogicalDataType.SmallInteger,
        ["MEDIUMINT"] = LogicalDataType.Integer,
        ["DECIMAL"] = LogicalDataType.Decimal,
        ["NUMERIC"] = LogicalDataType.Decimal,
        ["MONEY"] = LogicalDataType.Decimal,
        ["FLOAT"] = LogicalDataType.Float,
        ["DOUBLE"] = LogicalDataType.Float,
        ["DOUBLE PRECISION"] = LogicalDataType.Float,
        ["REAL"] = LogicalDataType.Float,
        ["BOOLEAN"] = LogicalDataType.Boolean,
        ["BOOL"] = LogicalDataType.Boolean,
        ["BIT"] = LogicalDataType.Boolean,
        ["TEXT"] = LogicalDataType.Text,
        ["MEDIUMTEXT"] = LogicalDataType.Text,
        ["LONGTEXT"] = LogicalDataType.Text,
        ["TINYTEXT"] = LogicalDataType.Text,
        ["CLOB"] = LogicalDataType.Text,
        ["VARCHAR"] = LogicalDataType.Varchar,
        ["NVARCHAR"] = LogicalDataType.Varchar,
        ["CHARACTER VARYING"] = LogicalDataType.Varchar,
        ["VARCHAR2"] = LogicalDataType.Varchar,
        ["CHAR"] = LogicalDataType.Char,
        ["NCHAR"] = LogicalDataType.Char,
        ["CHARACTER"] = LogicalDataType.Char,
        ["DATE"] = LogicalDataType.Date,
        ["DATETIME"] = LogicalDataType.DateTime,
        ["DATETIME2"] = LogicalDataType.DateTime,
        ["SMALLDATETIME"] = LogicalDataType.DateTime,
        ["TIMESTAMP"] = LogicalDataType.DateTime,
        ["TIMESTAMP WITHOUT TIME ZONE"] = LogicalDataType.DateTime,
        ["TIMESTAMP WITH TIME ZONE"] = LogicalDataType.Timestamp,
        ["TIMESTAMPTZ"] = LogicalDataType.Timestamp,
        ["DATETIMEOFFSET"] = LogicalDataType.Timestamp,
        ["TIME"] = LogicalDataType.Time,
        ["TIME WITHOUT TIME ZONE"] = LogicalDataType.Time,
        ["TIME WITH TIME ZONE"] = LogicalDataType.Time,
        ["BLOB"] = LogicalDataType.Blob,
        ["BYTEA"] = LogicalDataType.Blob,
        ["VARBINARY"] = LogicalDataType.Blob,
        ["BINARY"] = LogicalDataType.Blob,
        ["IMAGE"] = LogicalDataType.Blob,
        ["LONGBLOB"] = LogicalDataType.Blob,
        ["MEDIUMBLOB"] = LogicalDataType.Blob,
        ["TINYBLOB"] = LogicalDataType.Blob,
        ["UUID"] = LogicalDataType.Uuid,
        ["UNIQUEIDENTIFIER"] = LogicalDataType.Uuid,
        ["JSON"] = LogicalDataType.Json,
        ["JSONB"] = LogicalDataType.Json,
    };

    public static SchemaDocument Parse(string sql)
    {
        var tables = new List<TableDefinition>();
        var relationships = new List<RelationshipDefinition>();

        var statements = SplitStatements(sql);

        foreach (var stmt in statements)
        {
            var trimmed = stmt.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (IsCreateTable(trimmed))
            {
                var (table, inlineRelationships) = ParseCreateTable(trimmed);
                if (table != null)
                {
                    tables.Add(table);
                    relationships.AddRange(inlineRelationships);
                }
            }
            else if (IsAlterTableAddConstraint(trimmed))
            {
                var rel = ParseAlterTableForeignKey(trimmed);
                if (rel != null)
                    relationships.Add(rel);
            }
            else if (IsCreateIndex(trimmed))
            {
                ParseCreateIndex(trimmed, tables);
            }
        }

        // Resolve relationship table/column IDs by name
        var resolvedRelationships = ResolveRelationships(relationships, tables);

        // Position tables in a grid
        LayoutTables(tables);

        return new SchemaDocument
        {
            Name = "Imported Schema",
            Tables = tables,
            Relationships = resolvedRelationships,
        };
    }

    private static List<string> SplitStatements(string sql)
    {
        var statements = new List<string>();
        var current = new System.Text.StringBuilder();
        int depth = 0;
        bool inSingleQuote = false;
        bool inDoubleQuote = false;
        bool inLineComment = false;
        bool inBlockComment = false;

        for (int i = 0; i < sql.Length; i++)
        {
            char c = sql[i];
            char next = i + 1 < sql.Length ? sql[i + 1] : '\0';

            if (inLineComment)
            {
                if (c == '\n')
                    inLineComment = false;
                continue;
            }

            if (inBlockComment)
            {
                if (c == '*' && next == '/')
                {
                    inBlockComment = false;
                    i++;
                }
                continue;
            }

            if (c == '-' && next == '-')
            {
                inLineComment = true;
                i++;
                continue;
            }

            if (c == '/' && next == '*')
            {
                inBlockComment = true;
                i++;
                continue;
            }

            if (!inDoubleQuote && c == '\'')
            {
                // Handle escaped single quotes ('')
                if (inSingleQuote && next == '\'')
                {
                    current.Append(c);
                    current.Append(next);
                    i++;
                    continue;
                }
                inSingleQuote = !inSingleQuote;
                current.Append(c);
                continue;
            }

            if (!inSingleQuote && c == '"')
            {
                inDoubleQuote = !inDoubleQuote;
                current.Append(c);
                continue;
            }

            if (inSingleQuote || inDoubleQuote)
            {
                current.Append(c);
                continue;
            }

            if (c == '(') depth++;
            if (c == ')') depth--;

            if (c == ';' && depth <= 0)
            {
                var text = current.ToString().Trim();
                if (text.Length > 0)
                    statements.Add(text);
                current.Clear();
                depth = 0;
                continue;
            }

            current.Append(c);
        }

        var remaining = current.ToString().Trim();
        if (remaining.Length > 0)
            statements.Add(remaining);

        return statements;
    }

    private static bool IsCreateTable(string stmt) =>
        Regex.IsMatch(stmt, @"^\s*CREATE\s+TABLE", RegexOptions.IgnoreCase);

    private static bool IsAlterTableAddConstraint(string stmt) =>
        Regex.IsMatch(stmt, @"^\s*ALTER\s+TABLE", RegexOptions.IgnoreCase);

    private static bool IsCreateIndex(string stmt) =>
        Regex.IsMatch(stmt, @"^\s*CREATE\s+(UNIQUE\s+)?INDEX", RegexOptions.IgnoreCase);

    private static (TableDefinition? Table, List<RelationshipDefinition> Relationships) ParseCreateTable(string stmt)
    {
        // Extract table name: CREATE TABLE [IF NOT EXISTS] <name> (
        var headerMatch = Regex.Match(stmt,
            @"CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?(?:(\w+)\.)?([`""\[\w][^(]*?)\s*\(",
            RegexOptions.IgnoreCase);

        if (!headerMatch.Success)
            return (null, []);

        var schemaName = headerMatch.Groups[1].Success ? headerMatch.Groups[1].Value : "public";
        var tableName = UnquoteIdentifier(headerMatch.Groups[2].Value.Trim());

        // Extract the body between the outermost parentheses
        var body = ExtractParenthesizedBody(stmt, headerMatch.Index + headerMatch.Length - 1);
        if (body == null)
            return (null, []);

        var elements = SplitTableBody(body);

        var columns = new List<ColumnDefinition>();
        var relationships = new List<RelationshipDefinition>();
        var checkConstraints = new List<CheckConstraint>();
        var compositePkColumns = new List<string>();
        var uniqueConstraintColumns = new List<List<string>>();
        int ordinal = 0;

        foreach (var element in elements)
        {
            var trimmed = element.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (IsPrimaryKeyConstraint(trimmed))
            {
                compositePkColumns.AddRange(ExtractConstraintColumns(trimmed));
            }
            else if (IsForeignKeyConstraint(trimmed))
            {
                var rel = ParseInlineForeignKey(trimmed, tableName);
                if (rel != null)
                    relationships.Add(rel);
            }
            else if (IsUniqueConstraint(trimmed))
            {
                uniqueConstraintColumns.Add(ExtractConstraintColumns(trimmed));
            }
            else if (IsCheckConstraint(trimmed))
            {
                var check = ParseCheckConstraint(trimmed);
                if (check != null)
                    checkConstraints.Add(check);
            }
            else
            {
                // It's a column definition
                var col = ParseColumnDefinition(trimmed, ordinal);
                if (col != null)
                {
                    columns.Add(col);
                    ordinal++;
                }
            }
        }

        // Apply composite primary key
        if (compositePkColumns.Count > 0)
        {
            columns = columns.Select(c =>
                compositePkColumns.Any(pk => string.Equals(pk, c.Name, StringComparison.OrdinalIgnoreCase))
                    ? c with { IsPrimaryKey = true, IsNullable = false }
                    : c
            ).ToList();
        }

        // Apply unique constraints
        foreach (var uniqueCols in uniqueConstraintColumns)
        {
            if (uniqueCols.Count == 1)
            {
                columns = columns.Select(c =>
                    string.Equals(uniqueCols[0], c.Name, StringComparison.OrdinalIgnoreCase)
                        ? c with { IsUnique = true }
                        : c
                ).ToList();
            }
        }

        var table = new TableDefinition
        {
            Name = tableName,
            Schema = schemaName,
            Columns = columns,
            CheckConstraints = checkConstraints,
        };

        return (table, relationships);
    }

    private static ColumnDefinition? ParseColumnDefinition(string def, int ordinal)
    {
        // Column starts with an identifier (possibly quoted)
        var match = Regex.Match(def, @"^(\[[^\]]*\]|`[^`]+`|""[^""]+""|\w+)\s+(.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (!match.Success)
            return null;

        var name = UnquoteIdentifier(match.Groups[1].Value);
        var rest = match.Groups[2].Value.Trim();

        // Check for keywords that indicate this is NOT a column definition
        if (IsReservedTableKeyword(name))
            return null;

        // Parse data type
        var (dataType, length, precision, scale, remaining) = ParseDataType(rest);

        // Parse column constraints from the remaining text
        bool isPrimaryKey = false;
        bool isAutoIncrement = false;
        bool isNullable = true;
        bool isUnique = false;
        string? defaultValue = null;
        string? checkExpression = null;

        var upper = remaining.ToUpperInvariant();

        if (Regex.IsMatch(upper, @"\bPRIMARY\s+KEY\b"))
        {
            isPrimaryKey = true;
            isNullable = false;
        }

        if (Regex.IsMatch(upper, @"\b(AUTOINCREMENT|AUTO_INCREMENT|IDENTITY|GENERATED\s+(ALWAYS|BY\s+DEFAULT)\s+AS\s+IDENTITY)\b"))
        {
            isAutoIncrement = true;
        }

        // SERIAL types imply auto-increment and primary-key-capable
        if (Regex.IsMatch(rest, @"^\s*(BIG)?SERIAL\b", RegexOptions.IgnoreCase))
        {
            isAutoIncrement = true;
        }

        if (Regex.IsMatch(upper, @"\bNOT\s+NULL\b"))
        {
            isNullable = false;
        }
        else if (Regex.IsMatch(upper, @"\bNULL\b") && !Regex.IsMatch(upper, @"\bNOT\s+NULL\b"))
        {
            isNullable = true;
        }

        if (Regex.IsMatch(upper, @"\bUNIQUE\b"))
        {
            isUnique = true;
        }

        // Extract DEFAULT value
        var defaultMatch = Regex.Match(remaining,
            @"\bDEFAULT\s+('(?:[^']|'')*'|[^\s,)]+(?:\([^)]*\))?)",
            RegexOptions.IgnoreCase);
        if (defaultMatch.Success)
        {
            defaultValue = defaultMatch.Groups[1].Value;
        }

        // Extract inline CHECK constraint
        var checkMatch = Regex.Match(remaining, @"\bCHECK\s*\(", RegexOptions.IgnoreCase);
        if (checkMatch.Success)
        {
            checkExpression = ExtractParenthesizedBody(remaining, checkMatch.Index + checkMatch.Length - 1);
        }

        // Handle TINYINT(1) -> Boolean
        if (dataType == LogicalDataType.SmallInteger && length == 1)
        {
            dataType = LogicalDataType.Boolean;
            length = null;
        }

        return new ColumnDefinition
        {
            Name = name,
            DataType = dataType,
            Length = length,
            Precision = precision,
            Scale = scale,
            IsPrimaryKey = isPrimaryKey,
            IsAutoIncrement = isAutoIncrement,
            IsNullable = isNullable,
            IsUnique = isUnique,
            DefaultValue = defaultValue,
            CheckExpression = checkExpression,
            OrdinalPosition = ordinal,
        };
    }

    private static (LogicalDataType Type, int? Length, int? Precision, int? Scale, string Remaining) ParseDataType(string text)
    {
        // Try to match multi-word types first (e.g., "DOUBLE PRECISION", "CHARACTER VARYING",
        // "TIMESTAMP WITH TIME ZONE", etc.)
        var typeMatch = Regex.Match(text,
            @"^(TIMESTAMP\s+WITH(?:OUT)?\s+TIME\s+ZONE|TIME\s+WITH(?:OUT)?\s+TIME\s+ZONE|DOUBLE\s+PRECISION|CHARACTER\s+VARYING|GENERATED\s+ALWAYS\s+AS\s+IDENTITY|GENERATED\s+BY\s+DEFAULT\s+AS\s+IDENTITY)\s*(\()?",
            RegexOptions.IgnoreCase);

        string typeName;
        string remaining;

        if (typeMatch.Success)
        {
            typeName = Regex.Replace(typeMatch.Groups[1].Value.Trim(), @"\s+", " ");
            remaining = text[typeMatch.Length..];
            if (typeMatch.Groups[2].Success)
            {
                // There's a parenthesis; re-include it so the length parsing below sees it
                remaining = "(" + remaining;
            }
        }
        else
        {
            // Single-word type name
            var singleMatch = Regex.Match(text, @"^(\w+)", RegexOptions.IgnoreCase);
            if (!singleMatch.Success)
                return (LogicalDataType.Text, null, null, null, text);

            typeName = singleMatch.Groups[1].Value;
            remaining = text[singleMatch.Length..].TrimStart();
        }

        int? length = null;
        int? precision = null;
        int? scale = null;

        // Check for (N) or (P,S) after type name
        var paramMatch = Regex.Match(remaining, @"^\s*\(\s*(\d+)(?:\s*,\s*(\d+))?\s*\)");
        if (paramMatch.Success)
        {
            var first = int.Parse(paramMatch.Groups[1].Value);
            remaining = remaining[paramMatch.Length..];

            if (paramMatch.Groups[2].Success)
            {
                precision = first;
                scale = int.Parse(paramMatch.Groups[2].Value);
            }
            else
            {
                length = first;
            }
        }

        var normalizedType = typeName.ToUpperInvariant();

        // Map to LogicalDataType
        if (TypeMap.TryGetValue(normalizedType, out var dataType))
            return (dataType, length, precision, scale, remaining);

        // Fallback: try without trailing digits (e.g., INT4, INT8)
        var baseType = Regex.Replace(normalizedType, @"\d+$", "");
        if (baseType != normalizedType && TypeMap.TryGetValue(baseType, out dataType))
            return (dataType, length, precision, scale, remaining);

        return (LogicalDataType.Text, length, precision, scale, remaining);
    }

    private static bool IsPrimaryKeyConstraint(string element)
    {
        return Regex.IsMatch(element,
            @"^(?:CONSTRAINT\s+\S+\s+)?PRIMARY\s+KEY\s*\(",
            RegexOptions.IgnoreCase);
    }

    private static bool IsForeignKeyConstraint(string element)
    {
        return Regex.IsMatch(element,
            @"^(?:CONSTRAINT\s+\S+\s+)?FOREIGN\s+KEY\s*\(",
            RegexOptions.IgnoreCase);
    }

    private static bool IsUniqueConstraint(string element)
    {
        return Regex.IsMatch(element,
            @"^(?:CONSTRAINT\s+\S+\s+)?UNIQUE\s*\(",
            RegexOptions.IgnoreCase);
    }

    private static bool IsCheckConstraint(string element)
    {
        return Regex.IsMatch(element,
            @"^(?:CONSTRAINT\s+\S+\s+)?CHECK\s*\(",
            RegexOptions.IgnoreCase);
    }

    private static bool IsReservedTableKeyword(string name)
    {
        var upper = name.ToUpperInvariant();
        return upper is "PRIMARY" or "FOREIGN" or "UNIQUE" or "CHECK"
            or "CONSTRAINT" or "INDEX" or "KEY";
    }

    private static List<string> ExtractConstraintColumns(string element)
    {
        var match = Regex.Match(element, @"\(\s*([^)]+)\s*\)");
        if (!match.Success)
            return [];

        return match.Groups[1].Value
            .Split(',')
            .Select(c => UnquoteIdentifier(c.Trim()))
            .Where(c => !string.IsNullOrEmpty(c))
            .ToList();
    }

    private static RelationshipDefinition? ParseInlineForeignKey(string element, string sourceTableName)
    {
        // CONSTRAINT name FOREIGN KEY (col) REFERENCES target_table (col) [ON DELETE ...] [ON UPDATE ...]
        // Or without CONSTRAINT: FOREIGN KEY (col) REFERENCES target_table (col)
        var match = Regex.Match(element,
            @"(?:CONSTRAINT\s+(\S+)\s+)?FOREIGN\s+KEY\s*\(\s*([^)]+)\s*\)\s*REFERENCES\s+(\S+)\s*\(\s*([^)]+)\s*\)" +
            @"(?:\s+ON\s+DELETE\s+(CASCADE|SET\s+NULL|SET\s+DEFAULT|RESTRICT|NO\s+ACTION))?" +
            @"(?:\s+ON\s+UPDATE\s+(CASCADE|SET\s+NULL|SET\s+DEFAULT|RESTRICT|NO\s+ACTION))?",
            RegexOptions.IgnoreCase);

        if (!match.Success)
            return null;

        var constraintName = match.Groups[1].Success
            ? UnquoteIdentifier(match.Groups[1].Value)
            : "";
        var sourceColumn = UnquoteIdentifier(match.Groups[2].Value.Split(',')[0].Trim());
        var targetTable = UnquoteIdentifier(match.Groups[3].Value.Trim());
        var targetColumn = UnquoteIdentifier(match.Groups[4].Value.Split(',')[0].Trim());

        var onDelete = match.Groups[5].Success
            ? ParseReferentialAction(match.Groups[5].Value)
            : ReferentialAction.NoAction;
        var onUpdate = match.Groups[6].Success
            ? ParseReferentialAction(match.Groups[6].Value)
            : ReferentialAction.NoAction;

        return new RelationshipDefinition
        {
            Name = constraintName,
            // Store table/column names temporarily; resolved later by name
            SourceTableId = sourceTableName,
            SourceColumnId = sourceColumn,
            TargetTableId = targetTable,
            TargetColumnId = targetColumn,
            Cardinality = Cardinality.ManyToOne,
            OnDelete = onDelete,
            OnUpdate = onUpdate,
        };
    }

    private static CheckConstraint? ParseCheckConstraint(string element)
    {
        var nameMatch = Regex.Match(element, @"^CONSTRAINT\s+(\S+)\s+CHECK\s*\(",
            RegexOptions.IgnoreCase);
        var name = nameMatch.Success ? UnquoteIdentifier(nameMatch.Groups[1].Value) : "";

        var parenIndex = element.IndexOf('(');
        if (parenIndex < 0)
            return null;

        var expression = ExtractParenthesizedBody(element, parenIndex);
        if (expression == null)
            return null;

        return new CheckConstraint
        {
            Name = name,
            Expression = expression,
        };
    }

    private static RelationshipDefinition? ParseAlterTableForeignKey(string stmt)
    {
        // ALTER TABLE <table> ADD CONSTRAINT <name> FOREIGN KEY (<col>) REFERENCES <table> (<col>) ...
        var match = Regex.Match(stmt,
            @"ALTER\s+TABLE\s+(?:IF\s+EXISTS\s+)?(?:\w+\.)?(\S+)\s+" +
            @"ADD\s+(?:CONSTRAINT\s+(\S+)\s+)?" +
            @"FOREIGN\s+KEY\s*\(\s*([^)]+)\s*\)\s*" +
            @"REFERENCES\s+(?:\w+\.)?(\S+)\s*\(\s*([^)]+)\s*\)" +
            @"(?:\s+ON\s+DELETE\s+(CASCADE|SET\s+NULL|SET\s+DEFAULT|RESTRICT|NO\s+ACTION))?" +
            @"(?:\s+ON\s+UPDATE\s+(CASCADE|SET\s+NULL|SET\s+DEFAULT|RESTRICT|NO\s+ACTION))?",
            RegexOptions.IgnoreCase);

        if (!match.Success)
            return null;

        var sourceTable = UnquoteIdentifier(match.Groups[1].Value);
        var constraintName = match.Groups[2].Success
            ? UnquoteIdentifier(match.Groups[2].Value)
            : "";
        var sourceColumn = UnquoteIdentifier(match.Groups[3].Value.Split(',')[0].Trim());
        var targetTable = UnquoteIdentifier(match.Groups[4].Value);
        var targetColumn = UnquoteIdentifier(match.Groups[5].Value.Split(',')[0].Trim());

        var onDelete = match.Groups[6].Success
            ? ParseReferentialAction(match.Groups[6].Value)
            : ReferentialAction.NoAction;
        var onUpdate = match.Groups[7].Success
            ? ParseReferentialAction(match.Groups[7].Value)
            : ReferentialAction.NoAction;

        return new RelationshipDefinition
        {
            Name = constraintName,
            SourceTableId = sourceTable,
            SourceColumnId = sourceColumn,
            TargetTableId = targetTable,
            TargetColumnId = targetColumn,
            Cardinality = Cardinality.ManyToOne,
            OnDelete = onDelete,
            OnUpdate = onUpdate,
        };
    }

    private static void ParseCreateIndex(string stmt, List<TableDefinition> tables)
    {
        // CREATE [UNIQUE] INDEX <name> ON <table> (<columns>) [WHERE <expr>]
        var match = Regex.Match(stmt,
            @"CREATE\s+(UNIQUE\s+)?INDEX\s+(?:IF\s+NOT\s+EXISTS\s+)?(\S+)\s+ON\s+(?:\w+\.)?(\S+)\s*\(\s*([^)]+)\s*\)" +
            @"(?:\s+WHERE\s+(.+))?",
            RegexOptions.IgnoreCase);

        if (!match.Success)
            return;

        var isUnique = match.Groups[1].Success;
        var indexName = UnquoteIdentifier(match.Groups[2].Value);
        var tableName = UnquoteIdentifier(match.Groups[3].Value);
        var columnNames = match.Groups[4].Value
            .Split(',')
            .Select(c => UnquoteIdentifier(c.Trim().Split(' ')[0]))
            .ToList();
        var whereClause = match.Groups[5].Success ? match.Groups[5].Value.Trim() : null;

        var table = tables.FirstOrDefault(t =>
            string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

        if (table == null)
            return;

        var columnIds = columnNames
            .Select(name => table.Columns.FirstOrDefault(c =>
                string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            .Where(c => c != null)
            .Select(c => c!.Id)
            .ToList();

        if (columnIds.Count == 0)
            return;

        var index = new IndexDefinition
        {
            Name = indexName,
            ColumnIds = columnIds,
            IsUnique = isUnique,
            WhereClause = whereClause,
        };

        // Replace the table with one that includes the new index
        var tableIndex = tables.IndexOf(table);
        tables[tableIndex] = table with
        {
            Indexes = [.. table.Indexes, index],
        };
    }

    private static List<RelationshipDefinition> ResolveRelationships(
        List<RelationshipDefinition> unresolved,
        List<TableDefinition> tables)
    {
        var resolved = new List<RelationshipDefinition>();

        foreach (var rel in unresolved)
        {
            var sourceTable = tables.FirstOrDefault(t =>
                string.Equals(t.Name, rel.SourceTableId, StringComparison.OrdinalIgnoreCase));
            var targetTable = tables.FirstOrDefault(t =>
                string.Equals(t.Name, rel.TargetTableId, StringComparison.OrdinalIgnoreCase));

            if (sourceTable == null || targetTable == null)
                continue;

            var sourceColumn = sourceTable.Columns.FirstOrDefault(c =>
                string.Equals(c.Name, rel.SourceColumnId, StringComparison.OrdinalIgnoreCase));
            var targetColumn = targetTable.Columns.FirstOrDefault(c =>
                string.Equals(c.Name, rel.TargetColumnId, StringComparison.OrdinalIgnoreCase));

            if (sourceColumn == null || targetColumn == null)
                continue;

            resolved.Add(rel with
            {
                SourceTableId = sourceTable.Id,
                SourceColumnId = sourceColumn.Id,
                TargetTableId = targetTable.Id,
                TargetColumnId = targetColumn.Id,
            });
        }

        return resolved;
    }

    private static void LayoutTables(List<TableDefinition> tables)
    {
        int columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(tables.Count)));

        for (int i = 0; i < tables.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            tables[i] = tables[i] with
            {
                Position = new CanvasPosition
                {
                    X = col * GridSpacing,
                    Y = row * GridSpacing,
                },
            };
        }
    }

    private static string UnquoteIdentifier(string identifier)
    {
        var trimmed = identifier.Trim();

        if (trimmed.Length >= 2)
        {
            // Double-quoted: "name"
            if (trimmed[0] == '"' && trimmed[^1] == '"')
                return trimmed[1..^1];

            // Backtick-quoted: `name`
            if (trimmed[0] == '`' && trimmed[^1] == '`')
                return trimmed[1..^1];

            // Bracket-quoted: [name]
            if (trimmed[0] == '[' && trimmed[^1] == ']')
                return trimmed[1..^1];
        }

        return trimmed;
    }

    private static string? ExtractParenthesizedBody(string text, int openParenIndex)
    {
        int depth = 0;
        int start = openParenIndex + 1;

        for (int i = openParenIndex; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '(') depth++;
            else if (c == ')')
            {
                depth--;
                if (depth == 0)
                    return text[start..i];
            }
        }

        // Unbalanced parentheses - take what we have
        return text.Length > start ? text[start..] : null;
    }

    private static List<string> SplitTableBody(string body)
    {
        var elements = new List<string>();
        var current = new System.Text.StringBuilder();
        int depth = 0;
        bool inSingleQuote = false;
        bool inDoubleQuote = false;

        for (int i = 0; i < body.Length; i++)
        {
            char c = body[i];

            if (!inDoubleQuote && c == '\'')
            {
                if (inSingleQuote && i + 1 < body.Length && body[i + 1] == '\'')
                {
                    current.Append(c);
                    current.Append(body[i + 1]);
                    i++;
                    continue;
                }
                inSingleQuote = !inSingleQuote;
                current.Append(c);
                continue;
            }

            if (!inSingleQuote && c == '"')
            {
                inDoubleQuote = !inDoubleQuote;
                current.Append(c);
                continue;
            }

            if (inSingleQuote || inDoubleQuote)
            {
                current.Append(c);
                continue;
            }

            if (c == '(') depth++;
            if (c == ')') depth--;

            if (c == ',' && depth == 0)
            {
                var text = current.ToString().Trim();
                if (text.Length > 0)
                    elements.Add(text);
                current.Clear();
                continue;
            }

            current.Append(c);
        }

        var remaining = current.ToString().Trim();
        if (remaining.Length > 0)
            elements.Add(remaining);

        return elements;
    }

    private static ReferentialAction ParseReferentialAction(string action) =>
        action.Trim().ToUpperInvariant().Replace("  ", " ") switch
        {
            "CASCADE" => ReferentialAction.Cascade,
            "SET NULL" => ReferentialAction.SetNull,
            "SET DEFAULT" => ReferentialAction.SetDefault,
            "RESTRICT" => ReferentialAction.Restrict,
            _ => ReferentialAction.NoAction,
        };
}
