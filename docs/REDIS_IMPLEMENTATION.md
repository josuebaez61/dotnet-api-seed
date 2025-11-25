# Redis Implementation for Refresh Tokens

## Implementation Summary

A complete Redis cache solution has been implemented to handle refresh tokens, eliminating the issue with the in-memory dictionary that was lost when restarting the application.

## What was implemented?

### 1. **Redis Cache Service**

- **ICacheService**: Interface with methods to handle cache operations
- **RedisCacheService**: Implementation using `IDistributedCache` and `StackExchange.Redis`

### 2. **Redis Configuration**

- Added to `docker-compose.yml` with production configuration
- Configuration in `appsettings.json` with connection string and expiration time
- Service registration in `DependencyInjection.cs`

### 3. **Cache Model**

- **CachedRefreshTokenInfo**: Specific model for cached tokens with additional metadata

### 4. **Updated AuthService**

- Removed in-memory dictionary `_refreshTokens`
- Implemented Redis distributed cache
- Tokens persist between application restarts

## Configuration

### Redis Settings in appsettings.json

```json
{
  "RedisSettings": {
    "ConnectionString": "localhost:6379,password=redis_password",
    "InstanceName": "CleanArchitecture",
    "Database": 0,
    "RefreshTokenCacheExpiration": "7.00:00:00"
  }
}
```

### Docker Compose

```yaml
redis:
  image: redis:7-alpine
  container_name: cleanarch-redis
  command: redis-server --requirepass redis_password --appendonly yes
  ports:
    - "6379:6379"
  volumes:
    - redis_data:/data
  restart: unless-stopped
```

## Cache Service Features

- **GetAsync<T>**: Get data from cache
- **SetAsync<T>**: Save data to cache with optional expiration
- **RemoveAsync**: Remove a specific key
- **ExistsAsync**: Check if a key exists
- **RemoveByPatternAsync**: Remove multiple keys by pattern
- **SetIfNotExistsAsync**: Save only if key doesn't exist

## Benefits of the new implementation

1. **Persistence**: Refresh tokens survive application restarts
2. **Scalability**: Works in distributed environments with multiple instances
3. **Performance**: Redis is very fast for cache operations
4. **Configurability**: Configurable expiration time
5. **Automatic cleanup**: Redis handles expiration automatically
6. **Monitoring**: Can monitor Redis usage externally

## Refresh Tokens Flow

1. **Login**: JWT + refresh token generated, stored in Redis with key `refresh_token:{token}`
2. **Refresh**: Token searched in Redis, if exists and is valid, new token pair is generated
3. **Logout**: Refresh token removed from Redis
4. **Expiration**: Redis automatically removes expired tokens

## For local development

```bash
# Start services
docker-compose up -d

# Verify Redis is working
docker exec -it cleanarch-redis redis-cli
# Inside Redis CLI:
# auth redis_password
# ping
# keys *
```

## For production

1. Configure Redis on dedicated server or cloud service (AWS ElastiCache, Azure Redis, etc.)
2. Update connection string in environment variables
3. Configure SSL/TLS if necessary
4. Implement monitoring and alerts

## Testing

To test that it works correctly:

1. Login → should generate tokens
2. Restart the application
3. Use refresh token → should work (previously failed)
4. In Redis CLI verify keys exist: `keys refresh_token:*`

The implementation completely resolves the issue of refresh tokens being lost when restarting the application.
