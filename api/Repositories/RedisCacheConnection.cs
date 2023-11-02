using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace Repositories;
public sealed class RedisConnection
{
    private static readonly Lazy<RedisConnection> lazy =
        new Lazy<RedisConnection>(() => new RedisConnection());

    public static RedisConnection Conn { get { return lazy.Value; } }

    // public IDatabase DB { get; }
    public StackExchange.Redis.IDatabase redisDatabase { get; }

    private RedisConnection()
    {
        var connection = ConnectionMultiplexer.Connect("localhost");
        redisDatabase = connection.GetDatabase();
    }
}