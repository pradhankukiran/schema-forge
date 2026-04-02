using SchemaForge.Core.Models;

namespace SchemaForge.Core.State;

public class SchemaState
{
    private const int MaxUndoDepth = 100;

    private readonly Stack<(ISchemaAction Action, ISchemaAction Inverse)> _undoStack = new();
    private readonly Stack<(ISchemaAction Action, ISchemaAction Inverse)> _redoStack = new();

    public event Action? OnChange;

    public SchemaDocument Document { get; private set; } = new();

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    public void LoadDocument(SchemaDocument document)
    {
        Document = document;
        _undoStack.Clear();
        _redoStack.Clear();
        OnChange?.Invoke();
    }

    public void Dispatch(ISchemaAction action)
    {
        var inverse = action.CreateInverse(Document);
        Document = action.Apply(Document);
        _undoStack.Push((action, inverse));
        _redoStack.Clear();

        if (_undoStack.Count > MaxUndoDepth)
            TrimUndoStack();

        OnChange?.Invoke();
    }

    public void Undo()
    {
        if (!CanUndo) return;

        var (action, inverse) = _undoStack.Pop();
        Document = inverse.Apply(Document);
        _redoStack.Push((inverse, action));
        OnChange?.Invoke();
    }

    public void Redo()
    {
        if (!CanRedo) return;

        var (action, inverse) = _redoStack.Pop();
        Document = action.Apply(Document);
        _undoStack.Push((action, inverse));
        OnChange?.Invoke();
    }

    private void TrimUndoStack()
    {
        var items = _undoStack.ToArray();
        _undoStack.Clear();
        for (int i = 0; i < MaxUndoDepth; i++)
            _undoStack.Push(items[i]);
    }
}
