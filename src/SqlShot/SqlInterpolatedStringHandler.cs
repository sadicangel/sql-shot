using System.Runtime.CompilerServices;
using System.Text;
using Npgsql;

namespace SqlShot;

[InterpolatedStringHandler]
public ref struct SqlInterpolatedStringHandler(int literalLength, int formattedCount)
{
    private readonly StringBuilder _builder = new(literalLength + formattedCount * 3);
    private readonly Span<NpgsqlParameter> _parameters = new NpgsqlParameter[formattedCount];
    private int _paramIndex = 0;

    public readonly void AppendLiteral(ReadOnlySpan<char> literal) => _builder.Append(literal);

    public void AppendFormatted<T>(T value)
    {
        _parameters[_paramIndex++] = new NpgsqlParameter<T> { TypedValue = value };
        _builder.Append($"${_paramIndex}");
    }

    public override readonly string ToString() => _builder.ToString();

    public readonly (string CommandText, NpgsqlParameter[] Parameters) GetPreparedAndClear()
    {
        var result = (ToString(), _parameters.ToArray());
        _builder.Clear();
        _parameters.Clear();
        return result;
    }
}
