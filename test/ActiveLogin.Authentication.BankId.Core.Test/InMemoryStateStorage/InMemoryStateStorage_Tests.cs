using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Xunit;

using StateStorage = ActiveLogin.Authentication.BankId.Core.InMemoryStateStorage;

namespace ActiveLogin.Authentication.BankId.Core.Test.InMemoryStateStorage;

record TestState<T>(int A, string B, T C);

public class InMemoryStateStorageTests
{
    private readonly StateStorage _storage;

    public InMemoryStateStorageTests()
    {
        var options = Options.Create(new MemoryCacheOptions());
        var memoryCache = new MemoryCache(options);
        _storage = new StateStorage(memoryCache, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task SaveState_SimpleObject_AndGetState_ReturnsValue()
    {
        // Arrange
        var value = "testState";

        // Act
        var key = await _storage.SetAsync(value);
        var storedState = await _storage.GetAsync<string>(key);

        // Assert
        Assert.Equal(value, storedState);
    }

    [Fact]
    public async Task SaveState_ComplexObject_AndGetState_ReturnsValue()
    {
        // Arrange
        TestState<TestState<bool>> value = new(1, "test", new(2, "whatev", true));

        // Act
        var key = await _storage.SetAsync(value);
        var storedState = await _storage.GetAsync<TestState<TestState<bool>>>(key);

        // Assert
        Assert.Equal(value, storedState);
    }

    [Fact]
    public async Task GetStateAsync_ReturnsNull_ForNonExistingKey()
    {
        // Arrange
        var key = "nonExistingKey";

        // Act
        var storedState = await _storage.GetAsync<string>(new(key));

        // Assert
        Assert.Null(storedState);
    }

    [Fact]
    public async Task Eviction_Works()
    {
        const int eviction_time = 20;
        var options = Options.Create(new MemoryCacheOptions());
        var memoryCache = new MemoryCache(options);
        var storage = new StateStorage(memoryCache, TimeSpan.FromMilliseconds(eviction_time));

        // Arrange
        var value = "testState";
        var start = DateTime.Now;
        var key = await storage.SetAsync(value);
        if (!await storage.TryGetAsync<string>(key, out _))
        {
            Assert.Fail("Could not get value");
        }

        // Act
        await Task.Delay(eviction_time*2);
        while (await storage.TryGetAsync<string>(key, out _))
        {
            await Task.Delay(eviction_time*2);
            if ((DateTime.Now - start).TotalMilliseconds > 10 * 1000)
            {
                Assert.Fail($"Eviction did not work {DateTime.Now - start}");
            }
        }

        var time = (DateTime.Now - start).TotalMilliseconds;

        Assert.True(time >= eviction_time);
    }
}
