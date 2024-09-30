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
          new Column("Title", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Description", DbType.String.WithSize(1000), ColumnProperty.Null),
          new Column("RowVersion", DbType.Guid, ColumnProperty.NotNull, "gen_random_uuid()")
        );

        Database.AddTable("TeamUser",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'TeamUser')"),
          new Column("TeamId", DbType.Guid, ColumnProperty.NotNull),
          new Column("UserId", DbType.Guid, ColumnProperty.NotNull),
          new Column("IsAdmin", DbType.Boolean, ColumnProperty.NotNull),
          new Column("CreatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
          new Column("UpdatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
          new Column("CreatedById", DbType.Guid, ColumnProperty.NotNull),
          new Column("UpdatedById", DbType.Guid, ColumnProperty.NotNull)
        );

        Database.AddForeignKey("FK_TeamUser_UserId", "TeamUser", "UserId", "User", "Id");
        Database.AddForeignKey("FK_TeamUser_TeamId", "TeamUser", "TeamId", "Team", "Id");
        Database.AddForeignKey("FK_TeamUser_CreatedById", "TeamUser", "CreatedById", "User", "Id");
        Database.AddForeignKey("FK_TeamUser_UpdatedById", "TeamUser", "UpdatedById", "User", "Id");
    }

    public override void Revert()
    {
        Database.RemoveTable("Team");
        Database.RemoveTable("TeamUser");
    }
}
