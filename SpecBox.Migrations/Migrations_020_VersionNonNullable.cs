using System.Data;
using SpecBox.Migrations.Utils;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(20)]
public class Migration_020_VersionNonNullable : Migration
{
    public override void Apply()
    {
        Database.ChangeDefaultValue("Project", "Version", "'default'");
        Database.ExecuteNonQuery(@"UPDATE public.""Project"" SET ""Version""='default' WHERE ""Version"" is NULL;");
        Database.ChangeColumn("Project", "Version", DbType.String, true);
    }

    public override void Revert()
    {
        Database.ChangeDefaultValue("Project", "Version", "NULL");
        Database.ChangeColumn("Project", "Version", DbType.String, false);
    }
}
