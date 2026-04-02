using SchemaForge.Core.Models;

namespace SchemaForge.Core.State;

public interface ISchemaAction
{
    SchemaDocument Apply(SchemaDocument document);
    ISchemaAction CreateInverse(SchemaDocument document);
}
