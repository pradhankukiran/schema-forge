const editors = new Map();
let monacoReady = false;
let dotNetRef = null;
let monacoLoadPromise = null;

export async function initialize(dotNetReference) {
    dotNetRef = dotNetReference;

    if (!monacoLoadPromise) {
        monacoLoadPromise = loadMonacoLocally();
    }

    await monacoLoadPromise;
    monacoReady = true;
    return true;
}

function loadMonacoLocally() {
    return new Promise((resolve, reject) => {
        if (window.monaco) {
            resolve();
            return;
        }

        const vsBaseUrl = new URL("../vendor/monaco-editor/min/vs/", import.meta.url).toString().replace(/\/$/, "");
        const script = document.createElement("script");
        script.src = new URL("../vendor/monaco-editor/min/vs/loader.js", import.meta.url).toString();
        script.onload = () => {
            window.require.config({
                paths: { vs: vsBaseUrl }
            });
            window.require(["vs/editor/editor.main"], resolve, reject);
        };
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

export function createEditor(elementId, initialValue, language) {
    if (!monacoReady) throw new Error("Monaco not initialized");

    const container = document.getElementById(elementId);
    if (!container) throw new Error(`Element ${elementId} not found`);

    dispose(elementId);

    const editor = monaco.editor.create(container, {
        value: initialValue || "",
        language: language || "sql",
        theme: "vs",
        minimap: { enabled: false },
        fontSize: 13,
        fontFamily: "'JetBrains Mono', 'Cascadia Code', 'Fira Code', monospace",
        lineNumbers: "on",
        scrollBeyondLastLine: false,
        automaticLayout: true,
        tabSize: 2,
        wordWrap: "on",
        padding: { top: 8 },
        renderLineHighlight: "line",
        scrollbar: {
            verticalScrollbarSize: 10,
            horizontalScrollbarSize: 10
        }
    });

    // Ctrl+Enter / F5 to execute
    editor.addAction({
        id: "execute-query",
        label: "Execute Query",
        keybindings: [
            monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter,
            monaco.KeyCode.F5
        ],
        run: () => {
            if (dotNetRef) {
                const selectedText = editor.getModel().getValueInRange(editor.getSelection());
                const text = selectedText || editor.getValue();
                dotNetRef.invokeMethodAsync("OnExecuteRequested_Internal", elementId, text);
            }
        }
    });

    const record = { editor, context: "" };
    editor.onDidChangeModelContent(() => {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync(
                "OnContentChanged_Internal",
                elementId,
                record.context || "",
                editor.getValue());
        }
    });

    editors.set(elementId, record);
    return true;
}

export function hasEditor(elementId) {
    return editors.has(elementId);
}

export function setContext(elementId, context) {
    const record = editors.get(elementId);
    if (record) {
        record.context = context || "";
    }
}

export function getValue(elementId) {
    const record = editors.get(elementId);
    return record ? record.editor.getValue() : "";
}

export function setValue(elementId, value) {
    const record = editors.get(elementId);
    if (record) {
        record.editor.setValue(value || "");
    }
}

export function getSelectedText(elementId) {
    const record = editors.get(elementId);
    if (!record) return "";
    return record.editor.getModel().getValueInRange(record.editor.getSelection()) || "";
}

export function setCompletionItems(tableNames, tableColumns) {
    if (!monacoReady) return;

    // Dispose previous provider if any
    if (window._sfCompletionDisposable) {
        window._sfCompletionDisposable.dispose();
    }

    window._sfCompletionDisposable = monaco.languages.registerCompletionItemProvider("sql", {
        provideCompletionItems: (model, position) => {
            const word = model.getWordUntilPosition(position);
            const range = {
                startLineNumber: position.lineNumber,
                endLineNumber: position.lineNumber,
                startColumn: word.startColumn,
                endColumn: word.endColumn
            };

            const suggestions = [];

            // Table names
            for (const name of tableNames) {
                suggestions.push({
                    label: name,
                    kind: monaco.languages.CompletionItemKind.Struct,
                    insertText: name,
                    range: range
                });
            }

            // Column names (grouped by table)
            for (const [table, columns] of Object.entries(tableColumns)) {
                for (const col of columns) {
                    suggestions.push({
                        label: col,
                        kind: monaco.languages.CompletionItemKind.Field,
                        detail: table,
                        insertText: col,
                        range: range
                    });
                }
            }

            // SQL keywords
            const keywords = [
                "SELECT", "FROM", "WHERE", "INSERT", "INTO", "VALUES",
                "UPDATE", "SET", "DELETE", "CREATE", "TABLE", "DROP",
                "ALTER", "ADD", "INDEX", "JOIN", "LEFT", "RIGHT",
                "INNER", "OUTER", "ON", "AND", "OR", "NOT", "NULL",
                "IS", "IN", "LIKE", "BETWEEN", "ORDER", "BY",
                "GROUP", "HAVING", "LIMIT", "OFFSET", "AS",
                "DISTINCT", "COUNT", "SUM", "AVG", "MIN", "MAX",
                "PRIMARY", "KEY", "FOREIGN", "REFERENCES",
                "CONSTRAINT", "UNIQUE", "CHECK", "DEFAULT",
                "CASCADE", "RESTRICT", "INTEGER", "TEXT", "REAL",
                "BLOB", "VARCHAR", "BOOLEAN", "EXPLAIN", "QUERY", "PLAN"
            ];

            for (const kw of keywords) {
                suggestions.push({
                    label: kw,
                    kind: monaco.languages.CompletionItemKind.Keyword,
                    insertText: kw,
                    range: range
                });
            }

            return { suggestions };
        }
    });
}

export function focus(elementId) {
    const record = editors.get(elementId);
    if (record) {
        record.editor.focus();
    }
}

export function dispose(elementId) {
    const record = editors.get(elementId);
    if (record) {
        record.editor.dispose();
        editors.delete(elementId);
    }
}

export function disposeAll() {
    for (const [, record] of editors) {
        record.editor.dispose();
    }
    editors.clear();
    if (window._sfCompletionDisposable) {
        window._sfCompletionDisposable.dispose();
    }
}
