using System.Data;
using ThinkingHome.Migrator.Framework;

namespace SpecBox.TestRun.Migrations;

[Migration(15)]
public class Migration_015_AssertOrder : Migration
{
  public override void Apply()
  {
    Database.AddColumn("ExportAssertion", new Column("GroupOrder", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("ExportAssertion", new Column("AssertionOrder", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("Assertion", new Column("Order", DbType.Int32, ColumnProperty.NotNull, 0));
    Database.AddColumn("AssertionGroup", new Column("Order", DbType.Int32, ColumnProperty.NotNull, 0));

    Database.ExecuteFromResource(GetType().Assembly, "SpecBox.Migrations.Resources.015_MergeExportedData.sql");
  }

  public override void Revert()
  {
    Database.RemoveColumn("ExportAssertion", "GroupOrder");
    Database.RemoveColumn("ExportAssertion", "AssertionOrder");
    Database.RemoveColumn("Assertion", "Order");
    Database.RemoveColumn("AssertionGroup", "Order");
  }
}
