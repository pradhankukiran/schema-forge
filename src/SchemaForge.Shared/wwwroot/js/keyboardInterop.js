let dotNetRef = null;

export function initialize(ref) {
    dotNetRef = ref;

    document.addEventListener("keydown", handleKeyDown);
}

function handleKeyDown(e) {
    if (!dotNetRef) return;

    // Don't intercept when typing in inputs/textareas
    const tag = e.target.tagName;
    const isInput = tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT";
    const isMonaco = e.target.closest(".monaco-editor");

    // Let Monaco handle its own shortcuts except our global ones
    const isGlobalShortcut = (
        (e.ctrlKey && e.key === "1") ||
        (e.ctrlKey && e.key === "2") ||
        (e.ctrlKey && e.key === "s") ||
        (e.ctrlKey && e.key === "z" && !isMonaco) ||
        (e.ctrlKey && e.key === "y" && !isMonaco) ||
        (e.ctrlKey && e.key === "t" && !isMonaco) ||
        (e.ctrlKey && e.key === "b") ||
        (e.ctrlKey && e.key === "g" && !isMonaco)
    );

    if (!isGlobalShortcut) return;

    e.preventDefault();

    const shortcut = [
        e.ctrlKey ? "Ctrl" : "",
        e.shiftKey ? "Shift" : "",
        e.altKey ? "Alt" : "",
        e.key.toUpperCase()
    ].filter(Boolean).join("+");

    dotNetRef.invokeMethodAsync("OnShortcut_Internal", shortcut);
}

export function dispose() {
    document.removeEventListener("keydown", handleKeyDown);
    dotNetRef = null;
}
