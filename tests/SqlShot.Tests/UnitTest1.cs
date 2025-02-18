namespace SqlShot.Tests;

public class UnitTest1(DatabaseFixture databaseFixture)
{
    [Fact]
    public async Task Test1()
    {
        var value1 = 42;
        var value2 = "hello";
        await using var connection = await databaseFixture.DataSource.OpenConnectionAsync(TestContext.Current.CancellationToken);
        await connection.ExecuteAsync($"""
            CREATE TABLE table_name(
                column1 int,
                column2 varchar
            );
            """,
            TestContext.Current.CancellationToken);

        await connection.ExecuteAsync($"""
            INSERT INTO table_name(column1, column2)
            VALUES({value1}, {value2})
            """,
            TestContext.Current.CancellationToken);
    }
}
