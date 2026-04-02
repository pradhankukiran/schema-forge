let db = null;
let SQL = null;

export async function initialize() {
    if (SQL) return true;

    SQL = await initSqlJs({
        locateFile: file => `https://cdnjs.cloudflare.com/ajax/libs/sql.js/1.11.0/${file}`
    });

    db = new SQL.Database();
    return true;
}

export function execute(sql) {
    if (!db) throw new Error("Database not initialized");

    const start = performance.now();
    try {
        const results = db.exec(sql);
        const elapsed = performance.now() - start;

        if (results.length === 0) {
            const changes = db.getRowsModified();
            return {
                success: true,
                columns: [],
                rows: [],
                rowsAffected: changes,
                executionTimeMs: elapsed,
                errorMessage: null
            };
        }

        const result = results[0];
        return {
            success: true,
            columns: result.columns,
            rows: result.values,
            rowsAffected: 0,
            executionTimeMs: elapsed,
            errorMessage: null
        };
    } catch (e) {
        const elapsed = performance.now() - start;
        return {
            success: false,
            columns: [],
            rows: [],
            rowsAffected: 0,
            executionTimeMs: elapsed,
            errorMessage: e.message
        };
    }
}

export function executeBatch(sql) {
    if (!db) throw new Error("Database not initialized");

    const statements = sql.split(";").map(s => s.trim()).filter(s => s.length > 0);
    const results = [];

    for (const stmt of statements) {
        results.push(execute(stmt + ";"));
    }

    return results;
}

export function reset() {
    if (db) {
        db.close();
    }
    db = new SQL.Database();
    return true;
}

export function isReady() {
    return db !== null;
}

export function getTableNames() {
    if (!db) return [];
    try {
        const result = db.exec("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name");
        if (result.length === 0) return [];
        return result[0].values.map(r => r[0]);
    } catch {
        return [];
    }
}

export function getTableColumns(tableName) {
    if (!db) return [];
    try {
        const result = db.exec(`PRAGMA table_info("${tableName}")`);
        if (result.length === 0) return [];
        return result[0].values.map(r => ({
            name: r[1],
            type: r[2],
            notNull: r[3] === 1,
            defaultValue: r[4],
            isPrimaryKey: r[5] === 1
        }));
    } catch {
        return [];
    }
}
