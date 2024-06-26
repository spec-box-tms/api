using Npgsql;
using NpgsqlTypes;

namespace SpecBox.Domain.BulkCopy;

public class BulkWriterAssertion : BulkWriter
{
    private const string COMMAND =
        "COPY \"ExportAssertion\" (\"ExportId\", \"FeatureCode\",\"GroupTitle\",\"GroupOrder\",\"Title\",\"AssertionOrder\",\"Description\", \"IsAutomated\") FROM STDIN (FORMAT BINARY)";

    public BulkWriterAssertion(NpgsqlConnection connection) : base(COMMAND, connection)
    {
    }

    public async Task AddAssertion(
        Guid exportId,
        string featureCode,
        string groupTitle,
        int groupOrder,
        string title,
        int assertionOrder,
        string? description,
        bool isAutomated)
    {
        await Writer.StartRowAsync();
        await Writer.WriteAsync(exportId, NpgsqlDbType.Uuid);
        await Writer.WriteAsync(featureCode, NpgsqlDbType.Text);
        await Writer.WriteAsync(groupTitle, NpgsqlDbType.Text);
        await Writer.WriteAsync(groupOrder, NpgsqlDbType.Integer);
        await Writer.WriteAsync(title, NpgsqlDbType.Text);
        await Writer.WriteAsync(assertionOrder, NpgsqlDbType.Integer);
        await Writer.WriteAsync(description, NpgsqlDbType.Text);
        await Writer.WriteAsync(isAutomated, NpgsqlDbType.Boolean);
    }
}
