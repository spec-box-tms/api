using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(17)]
public class Migration_017_UserAuth : Migration
{
    public override void Apply()
    {
        Database.ExecuteNonQuery("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"");

        Database.AddTable("User",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'User')"),
          new Column("Login", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Email", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Name", DbType.String.WithSize(255), ColumnProperty.NotNull),
          new Column("Description", DbType.String.WithSize(1000), ColumnProperty.Null),
          new Column("CreatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
          new Column("UpdatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
          new Column("RowVersion", DbType.Guid, ColumnProperty.NotNull, "gen_random_uuid()")
        );

        Database.AddTable("PasswordAuth",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'PasswordAuth')"),
          new Column("UserId", DbType.Guid, ColumnProperty.NotNull),
          new Column("Hash", DbType.String.WithSize(1000), ColumnProperty.NotNull),
          new Column("Salt", DbType.Guid, ColumnProperty.NotNull),
          new Column("UpdatedAt", DbType.DateTime.WithSize(3), ColumnProperty.Null)
        );

        Database.AddTable("RefreshToken",
          new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "uuid_generate_v5(uuid_ns_oid(), 'RefreshToken')"),
          new Column("UserId", DbType.Guid, ColumnProperty.NotNull),
          new Column("Token", DbType.String.WithSize(1000), ColumnProperty.NotNull),
          new Column("ExpireAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
          new Column("UsedAt", DbType.DateTime.WithSize(3), ColumnProperty.Null)
        );

        Database.AddForeignKey("FK_PasswordAuth_UserId", "PasswordAuth", "UserId", "User", "Id");
        Database.AddForeignKey("FK_RefreshToken_UserId", "RefreshToken", "UserId", "User", "Id");

        Database.AddUniqueConstraint("UK_User_Login", "User", "Login");
        Database.AddUniqueConstraint("UK_User_Email", "User", "Email");
        
        Database.AddUniqueConstraint("UK_PasswordAuth_UserId", "PasswordAuth", "UserId");
    }

    public override void Revert()
    {
        Database.RemoveConstraint("User", "UK_User_Login");
        Database.RemoveConstraint("User", "UK_User_Email");
        Database.RemoveConstraint("PasswordAuth", "UK_PasswordAuth_UserId");
        Database.RemoveTable("User");
        Database.RemoveTable("PasswordAuth");
        Database.RemoveTable("RefreshToken");

    }
}
