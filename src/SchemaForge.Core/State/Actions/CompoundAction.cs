using SchemaForge.Core.Models;

namespace SchemaForge.Core.State.Actions;

public record CompoundAction(List<ISchemaAction> Actions) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc)
    {
        foreach (var action in Actions)
            doc = action.Apply(doc);
        return doc;
    }

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var inverses = new List<ISchemaAction>();
        var current = doc;
        foreach (var action in Actions)
        {
            inverses.Add(action.CreateInverse(current));
            current = action.Apply(current);
        }
        inverses.Reverse();
        return new CompoundAction(inverses);
    }
}
