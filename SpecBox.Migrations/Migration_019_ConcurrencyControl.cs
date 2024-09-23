using System.Data;
using ThinkingHome.Migrator.Framework;

namespace SpecBox.TestRun.Migrations;

[Migration(19)]
public class Migration_019_Team : Migration
{
    public override void Apply()
    {
        Database.AddColumn("User", new Column("RowVersion", DbType.Guid, ColumnProperty.NotNull, "gen_random_uuid()"));
        Database.AddColumn("Team", new Column("RowVersion", DbType.Guid, ColumnProperty.NotNull, "gen_random_uuid()"));
    }

    public override void Revert()
    {
        Database.RemoveColumn("User", "RowVersion");
        Database.RemoveColumn("Team", "RowVersion");
    }
}
