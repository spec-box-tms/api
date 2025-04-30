using System.Data;
using SpecBox.Migrations.Utils;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(19)]
public class Migration_019_TeamProjects : Migration
{
    public override void Apply()
    {
        Database.AddColumn("Project", new Column("TeamId", DbType.Guid, ColumnProperty.Null));

        Database.AddForeignKey("FK_Project_TeamId", "Project", "TeamId", "Team", "Id");
    }

    public override void Revert()
    {
        Database.RemoveConstraint("Project", "FK_Project_TeamId");
        Database.RemoveColumn("Project", "TeamId");
    }
}
