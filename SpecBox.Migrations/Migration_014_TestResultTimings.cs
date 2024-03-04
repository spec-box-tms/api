using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(14)]
public class Migration_014_TestResultTimings : Migration
{
  public override void Apply()
  {
    Database.AddColumn("TestResult", new Column("UpdatedAt", DbType.DateTime, ColumnProperty.NotNull, "now()"));
    Database.AddColumn("TestResult", new Column("StartedAt", DbType.DateTime, ColumnProperty.Null));
  }

  public override void Revert()
  {
    Database.RemoveColumn("TestResult", "UpdatedAt");
    Database.RemoveColumn("TestResult", "StartedAt");
  }
}
