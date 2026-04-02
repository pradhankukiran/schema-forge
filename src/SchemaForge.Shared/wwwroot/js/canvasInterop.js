export function getElementSize(element) {
    const rect = element.getBoundingClientRect();
    return { width: rect.width, height: rect.height, left: rect.left, top: rect.top };
}

let _resizeCallback = null;
export function observeResize(element, dotNetRef) {
    const observer = new ResizeObserver(entries => {
        for (const entry of entries) {
            const { width, height } = entry.contentRect;
            dotNetRef.invokeMethodAsync("OnContainerResized", width, height);
        }
    });
    observer.observe(element);
    _resizeCallback = observer;
}

export function disposeResize() {
    if (_resizeCallback) { _resizeCallback.disconnect(); _resizeCallback = null; }
}
