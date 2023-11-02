using Microsoft.EntityFrameworkCore.Storage;
using Repositories;

namespace Services;
public class BaseService
{
    protected static StackExchange.Redis.IDatabase redisDatabase = RedisConnection.Conn.redisDatabase;
}