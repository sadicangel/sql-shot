using System.Runtime.CompilerServices;
using System.Text;
using Npgsql;

namespace SqlShot;

[InterpolatedStringHandler]
public ref struct SqlInterpolatedStringHandler(int literalLength, int formattedCount)
{
    private readonly StringBuilder _builder = new(literalLength + formattedCount * 3);
    private int _paramIndex = 0;

    internal readonly NpgsqlParameter[] Parameters { get; } = new NpgsqlParameter[formattedCount];

    public readonly void AppendLiteral(ReadOnlySpan<char> literal) => _builder.Append(literal);

    public void AppendFormatted<T>(T value)
    {
        Parameters[_paramIndex] = new NpgsqlParameter { Value = value };
        _builder.Append($"${_paramIndex++}");
    }

    public override readonly string ToString() => "";

    public readonly string ToStringAndClear()
    {
        var result = ToString();
        _builder.Clear();
        Array.Clear(Parameters);
        return result;
    }
}

public static class SqlInterpolatedStringHandlerExtensions
{
    public static Task ExecuteAsync(this NpgsqlConnection connection, ref SqlInterpolatedStringHandler handler, CancellationToken cancellationToken = default)
    {
        var commandText = handler.ToString();
        var parameters = handler.Parameters;
        return AwaitedExecuteAsync(connection, commandText, parameters, cancellationToken);

        static async Task AwaitedExecuteAsync(NpgsqlConnection connection, string commandText, NpgsqlParameter[] parameters, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(parameters);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public static async Task ExampleUsage(NpgsqlDataSource dataSource, CancellationToken cancellationToken)
    {
        var value1 = 42;
        var value2 = "hello";

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync($"""
            INSERT INTO table_name (column1, column2)
            VALUES ({value1}, {value2})
            """,
            cancellationToken);
    }
}
