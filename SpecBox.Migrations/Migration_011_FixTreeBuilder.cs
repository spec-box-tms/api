using System.Data;
using ThinkingHome.Migrator.Framework;

namespace SpecBox.Migrations;

[Migration(11)]
public class Migration_011_FixTreeBuilder : Migration
{
    public override void Apply()
    {
        Database.ExecuteFromResource(GetType().Assembly, "SpecBox.Migrations.Resources.011_BuildTree.sql");
    }
}
