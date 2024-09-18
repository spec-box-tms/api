using System.Data;
using SpecBox.Migrations.Utils;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(18)]
public class Migration_018_Team : Migration
{
    public override void Apply()
    {
        Database.AddAuditableEntity("Team",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'Team')"),
          new Column("Code", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Title", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Description", DbType.String.WithSize(1000), ColumnProperty.Null)
        );

        Database.AddTable("TeamUser",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'TeamUser')"),
          new Column("TeamId", DbType.Guid, ColumnProperty.NotNull),
          new Column("UserId", DbType.Guid, ColumnProperty.NotNull),
          new Column("IsAdmin", DbType.Boolean, ColumnProperty.NotNull)
        );

        Database.AddForeignKey("FK_TeamUser_UserId", "TeamUser", "UserId", "User", "Id");
        Database.AddForeignKey("FK_TeamUser_TeamId", "TeamUser", "TeamId", "Team", "Id");
    }

    public override void Revert()
    {
        Database.RemoveConstraint("TeamUser", "FK_TeamUser_UserId");
        Database.RemoveConstraint("TeamUser", "FK_TeamUser_TeamId");
        Database.RemoveTable("Team");
        Database.RemoveTable("TeamUser");
    }
}
