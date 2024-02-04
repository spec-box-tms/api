using System.Data;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;

namespace SpecBox.TestRun.Migrations;

[Migration(13)]
public class Migration_013_TestRuns : Migration
{
  public override void Apply()
  {
    Database.AddTable("TestRun",
        new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "gen_random_uuid()"),
        new Column("Title", DbType.String.WithSize(255), ColumnProperty.NotNull),
        new Column("Description", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
        new Column("CreatedAt", DbType.DateTime, ColumnProperty.Null),
        new Column("StartedAt", DbType.DateTime, ColumnProperty.Null),
        new Column("CompletedAt", DbType.DateTime, ColumnProperty.Null),
        new Column("ProjectId", DbType.Guid, ColumnProperty.NotNull));

    Database.AddForeignKey("FK_TestRun_ProjectId", "TestRun", "ProjectId", "Project", "Id");
    
     Database.AddTable("TestResult",
        new Column("Id", DbType.Guid, ColumnProperty.PrimaryKey, "gen_random_uuid()"),
        new Column("Status", DbType.String.WithSize(255), ColumnProperty.NotNull),
        new Column("Report", DbType.String.WithSize(int.MaxValue), ColumnProperty.Null),
        new Column("CompletedAt", DbType.DateTime, ColumnProperty.Null),
        new Column("AssertionId", DbType.Guid, ColumnProperty.NotNull),
        new Column("TestRunId", DbType.Guid, ColumnProperty.NotNull));

    Database.AddForeignKey("FK_TestResult_TestRunId", "TestResult", "TestRunId", "TestRun", "Id");
    Database.AddForeignKey("FK_TestResult_AssertionId", "TestResult", "AssertionId", "Assertion", "Id");
  }

  public override void Revert()
  {
    Database.RemoveConstraint("TestRun", "FK_TestRun_ProjectId");
    Database.RemoveTable("TestRun");

    Database.RemoveConstraint("TestResult", "FK_TestResult_TestRunId");
    Database.RemoveConstraint("TestResult", "FK_TestResult_AssertionId");
    Database.RemoveTable("TestResult");
  }
}
