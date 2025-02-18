using Npgsql;

namespace SqlShot;

public static class NpgsqlConnectionExtensions
{
    public static Task<int> ExecuteAsync(this NpgsqlConnection connection, ref SqlInterpolatedStringHandler handler, CancellationToken cancellationToken = default)
    {
        var (commandText, parameters) = handler.GetPreparedAndClear();
        return AwaitedExecuteAsync(connection, commandText, parameters, cancellationToken);

        static async Task<int> AwaitedExecuteAsync(NpgsqlConnection connection, string commandText, NpgsqlParameter[] parameters, CancellationToken cancellationToken)
        {
            await using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddRange(parameters);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
