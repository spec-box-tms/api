using System.Data;
using System.Linq;
using System.Threading;
using ThinkingHome.Migrator.Framework;
using ThinkingHome.Migrator.Framework.Extensions;
using ThinkingHome.Migrator.Framework.Interfaces;

namespace SpecBox.Migrations.Utils;

public static class AddAuditableEntityExtensions
{
    public static void AddAuditableEntity(this ITransformationProvider database, SchemaQualifiedObjectName name, params Column[] columns)
    {
        var allColumns = columns.Concat([
                    new Column("CreatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
                    new Column("CreatedById", DbType.Guid, ColumnProperty.NotNull),
                    new Column("UpdatedAt", DbType.DateTime.WithSize(3), ColumnProperty.NotNull),
                    new Column("UpdatedById", DbType.Guid, ColumnProperty.NotNull),
                    new Column("DeletedAt", DbType.DateTime.WithSize(3), ColumnProperty.Null),
                    new Column("DeletedById", DbType.Guid, ColumnProperty.Null)
                ]).ToArray();

        database.AddTable(name, allColumns);

        database.AddForeignKey($"FK_{name.Name}_CreatedById", name, "CreatedById", "User", "Id");
        database.AddForeignKey($"FK_{name.Name}_UpdatedById", name, "UpdatedById", "User", "Id");
        database.AddForeignKey($"FK_{name.Name}_DeletedById", name, "DeletedById", "User", "Id");

    }
}