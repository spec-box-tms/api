using System.Data;
using ThinkingHome.Migrator.Framework;

namespace SpecBox.Migrations;

[Migration(12)]
public class Migration_012_ProjectTimeStamps : Migration
{
    public override void Apply()
    {
        Database.AddColumn("Project", new Column("CreatedAt", DbType.DateTime, ColumnProperty.Null));
        Database.AddColumn("Project", new Column("UpdatedAt", DbType.DateTime, ColumnProperty.Null));
    }
}
