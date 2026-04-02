const editors = new Map();
let monacoReady = false;
let dotNetRef = null;

export async function initialize(dotNetReference) {
    dotNetRef = dotNetReference;

    if (monacoReady) return true;

    await loadMonacoFromCdn();
    monacoReady = true;
    return true;
}

function loadMonacoFromCdn() {
    return new Promise((resolve, reject) => {
        if (window.monaco) {
            resolve();
            return;
        }

        const script = document.createElement("script");
        script.src = "https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.52.2/min/vs/loader.min.js";
        script.onload = () => {
            window.require.config({
                paths: { vs: "https://cdnjs.cloudflare.com/ajax/libs/monaco-editor/0.52.2/min/vs" }
            });
            window.require(["vs/editor/editor.main"], () => {
                resolve();
            });
        };
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

export function createEditor(elementId, initialValue, language) {
    if (!monacoReady) throw new Error("Monaco not initialized");

    const container = document.getElementById(elementId);
    if (!container) throw new Error(`Element ${elementId} not found`);

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
                dotNetRef.invokeMethodAsync("OnExecuteRequested", elementId, text);
            }
        }
    });

    editors.set(elementId, editor);
    return true;
}

export function getValue(elementId) {
    const editor = editors.get(elementId);
    return editor ? editor.getValue() : "";
}

export function setValue(elementId, value) {
    const editor = editors.get(elementId);
    if (editor) editor.setValue(value || "");
}

export function getSelectedText(elementId) {
    const editor = editors.get(elementId);
    if (!editor) return "";
    return editor.getModel().getValueInRange(editor.getSelection()) || "";
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
    const editor = editors.get(elementId);
    if (editor) editor.focus();
}

export function dispose(elementId) {
    const editor = editors.get(elementId);
    if (editor) {
        editor.dispose();
        editors.delete(elementId);
    }
}

export function disposeAll() {
    for (const [id, editor] of editors) {
        editor.dispose();
    }
    editors.clear();
    if (window._sfCompletionDisposable) {
        window._sfCompletionDisposable.dispose();
    }
}
