using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.Migrations;

[Migration(10)]
public class Migration_010_Versioning : Migration
{
    public override void Apply()
    {
        Database.AddColumn("Project", new Column("Version", DbType.String, ColumnProperty.Null));
        
        Database.RemoveConstraint("Project", "UK_Project_Code");
    }
}
