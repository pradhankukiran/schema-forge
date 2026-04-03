using SchemaForge.Core.DdlParsing;
using SchemaForge.Core.Models;
using SchemaForge.Core.State;
using SchemaForge.Core.State.Actions;

namespace SchemaForge.Core.Tests;

public class StateAndParsingTests
{
    [Fact]
    public void SchemaState_LoadDocument_DoesNotRaiseDirty_ButDispatchDoes()
    {
        var state = new SchemaState();
        var dirtyCount = 0;
        var changeCount = 0;

        state.OnDirty += () => dirtyCount++;
        state.OnChange += () => changeCount++;

        state.LoadDocument(new SchemaDocument { Name = "Imported" });

        Assert.Equal(0, dirtyCount);
        Assert.Equal(1, changeCount);

        state.Dispatch(new AddTableAction(new TableDefinition { Name = "users" }));

        Assert.Equal(1, dirtyCount);
        Assert.Equal(2, changeCount);
    }

    [Fact]
    public void QueryState_UpdateTabContent_RaisesDirtyWithoutUiChange()
    {
        var state = new QueryState();
        var dirtyCount = 0;
        var changeCount = 0;

        state.OnDirty += () => dirtyCount++;
        state.OnChange += () => changeCount++;

        state.UpdateTabContent(0, "select 1;");

        Assert.Equal(1, dirtyCount);
        Assert.Equal(0, changeCount);
        Assert.Equal("select 1;", state.ActiveTab.SqlContent);
    }

    [Fact]
    public void DdlParser_Parse_HandlesSupportedStatements()
    {
        const string sql = """
            -- Basic supported import path
            CREATE TABLE users (
              id INTEGER PRIMARY KEY,
              email VARCHAR(255) NOT NULL UNIQUE
            );

            CREATE TABLE posts (
              id INTEGER PRIMARY KEY,
              user_id INTEGER NOT NULL,
              title TEXT,
              CONSTRAINT fk_posts_users FOREIGN KEY (user_id) REFERENCES users(id)
            );

            CREATE INDEX idx_posts_user_id ON posts (user_id);
            """;

        var document = DdlParser.Parse(sql);

        Assert.Equal(2, document.Tables.Count);
        Assert.Single(document.Relationships);

        var posts = Assert.Single(document.Tables, t => t.Name == "posts");
        Assert.Single(posts.Indexes);
        Assert.Equal("idx_posts_user_id", posts.Indexes[0].Name);
    }
}
