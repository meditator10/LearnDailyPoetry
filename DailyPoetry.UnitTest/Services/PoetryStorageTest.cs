using DailyPoetry.Models;
using DailyPoetry.Services;
using Moq;
using System.Linq.Expressions;

namespace DailyPoetry.UnitTest.Services;

public class PoetryStorageTest : IDisposable
{
    // 当你需要某个业务在每一个单元测试中都跑一遍，那么放在构造函数里就好了
    public PoetryStorageTest() {
        File.Delete(PoetryStorage.PoetryDbPath);
    }

    public void Dispose(){
        File.Delete(PoetryStorage.PoetryDbPath);
    }

    [Fact]  // xUnit
    public void TestInitialized_defaulted()
    {
        // Arrange
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock
            .Setup(p => p.Get(PoetryStorageConstant.VersionKey, 0))
            .Returns(PoetryStorageConstant.Version);
        // 最终mock的对象
        var MockPreferenceStorage = preferenceStorageMock.Object;

        //  Act
        var poetryStorage = new PoetryStorage(MockPreferenceStorage);
        // Assert
        Assert.True(poetryStorage.IsInitialized);
    }

    [Fact]
    public async Task TestInitiatedAsync_defaulted()
    {
        // Arrange
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        // Act
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        // Assert
        // 验证文件存在
        Assert.False(File.Exists(PoetryStorage.PoetryDbPath));
        await poetryStorage.InitializedAsync();
        Assert.True(File.Exists(PoetryStorage.PoetryDbPath));

        // 验证函数是否被正常调用
        preferenceStorageMock.Verify(
            p => p.Set(PoetryStorageConstant.VersionKey, PoetryStorageConstant.Version)
            , Times.Once());
    }

    [Fact]
    public async Task TestGetPoetryAsync_default()
    {
        // Arrange
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        // Act
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        await poetryStorage.InitializedAsync();
        var poetry = await poetryStorage.GetPoetryAsync(10002);
        // Assert
        // 这里单元测试会报错，因为在业务场景中SqLite数据库是内嵌于程序中，无需考虑手动关闭数据库
        Assert.Equal("江城子 · 密州出猎", poetry.Name);
        
        // 手动关闭数据库，才可以在析构中删除数据库
        await poetryStorage.CloseAsync();
    }

    [Fact]
    public async Task TestGetPoetriesAsync_default()
    {
        // Arrange
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        // Act
        var poetryStorage = new PoetryStorage(mockPreferenceStorage);
        await poetryStorage.InitializedAsync();
        var poetries = await poetryStorage.GetPoetriesAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true)
            ,Expression.Parameter(typeof(Poetry), "p")), 0, int.MaxValue);
        // Assert
        Assert.Equal(30, poetries.Count());

        await poetryStorage.CloseAsync();
    }
}
