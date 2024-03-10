using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(16)]
public class Migration_016_TestRunStats : Migration
{
  public override void Apply()
  {
    Database.AddColumn("TestRun", new Column("TotalCount", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("TestRun", new Column("PassedCount", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("TestRun", new Column("FailedCount", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("TestRun", new Column("BlockedCount", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("TestRun", new Column("InvalidCount", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("TestRun", new Column("SkippedCount", DbType.Int32, ColumnProperty.NotNull, 0));

    Database.AddColumn("TestRun", new Column("Environment", DbType.String.WithSize(200), ColumnProperty.Null));
    Database.AddColumn("TestRun", new Column("Configuration", DbType.String.WithSize(200), ColumnProperty.Null));
  }

  public override void Revert()
  {
    Database.RemoveColumn("TestRun", "TotalCount");
    Database.RemoveColumn("TestRun", "PassedCount");
    Database.RemoveColumn("TestRun", "FailedCount");
    Database.RemoveColumn("TestRun", "BlockedCount");
    Database.RemoveColumn("TestRun", "InvalidCount");
    Database.RemoveColumn("TestRun", "SkippedCount");

    Database.RemoveColumn("TestRun", "Environment");
    Database.RemoveColumn("TestRun", "Configuration");
  }
}
