let db = null;
let SQL = null;

let _initPromise = null;
let _scriptLoadPromise = null;

function loadSqlJsLocally() {
    if (window.initSqlJs) {
        return Promise.resolve();
    }

    if (_scriptLoadPromise) {
        return _scriptLoadPromise;
    }

    _scriptLoadPromise = new Promise((resolve, reject) => {
        const script = document.createElement("script");
        script.src = new URL("../vendor/sql.js/sql-wasm.js", import.meta.url).toString();
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });

    return _scriptLoadPromise;
}

export function initialize() {
    if (!_initPromise) {
        _initPromise = (async () => {
            await loadSqlJsLocally();
            SQL = await initSqlJs({
                locateFile: file => new URL(`../vendor/sql.js/${file}`, import.meta.url).toString()
            });
            db = new SQL.Database();
            return true;
        })();
    }
    return _initPromise;
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
    const start = performance.now();
    try {
        const results = db.exec(sql);
        const elapsed = performance.now() - start;
        return results.map(r => ({
            success: true,
            columns: r.columns,
            rows: r.values,
            rowsAffected: 0,
            executionTimeMs: elapsed / results.length,
            errorMessage: null
        }));
    } catch (e) {
        return [{ success: false, columns: [], rows: [], rowsAffected: 0, executionTimeMs: performance.now() - start, errorMessage: e.message }];
    }
}

export function reset() {
    if (db) { db.close(); }
    if (!SQL) throw new Error("Database not initialized");
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
