using DailyPoetry.Models;
using SQLite;
using System.Linq.Expressions;

namespace DailyPoetry.Services;

public class PoetryStorage : IPoetryStorage
{
    #region  数据库操作
    /// <summary>
    /// 数据库定义, 保存数据库路径
    /// </summary>
    public const string DbName = "poetrydb.sqlite3";
    public static readonly string PoetryDbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder
            .LocalApplicationData), DbName);
    //PoetryDbPath不能是const常量，需要使用static readonly变成静态常量
    //static表示一个类中该变量只有一份（类级别），readonly表示只读只能被赋值一次

    /// <summary>
    /// 数据库连接
    /// </summary>
    // 数据库连接影子变量
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(PoetryDbPath);
    #endregion


    private readonly IPreferenceStorage _preferenceStorage;
    public PoetryStorage(IPreferenceStorage preferenceStorage)
    {
        _preferenceStorage = preferenceStorage;
    }

    public bool IsInitialized
    {
        get
        {
            // Preferences.Get()获取当前数据库版本
            if (_preferenceStorage.Get(PoetryStorageConstant.VersionKey, 0)
                == PoetryStorageConstant.Version)
                return true;
            else
                return false;
        }
    }

    // 把app附带的数据库文件copy到用户的目录中
    // 也可以简单的使用Connection.CreateTableAsync<Poetry>()创建数据库表
    public async Task InitializedAsync()
    {
        // 1、打开目标文件
        //var dbFileStream = new FileStream(PoetryDbPath, FileMode.OpenOrCreate);
        //dbFileStream.Close();
        // 微软提供的可自动做文件关闭操作，不用手动close
        await using var dbFileStream =
            new FileStream(PoetryDbPath, FileMode.OpenOrCreate);

        // 2、打开嵌入式资源
        await using var dbAssetStream = 
            typeof(PoetryStorage)
            .Assembly
            .GetManifestResourceStream(DbName);
        if (dbAssetStream == null)
        {
            throw new Exception($"can't find that named {DbName}");
        }
        // 3、把资源拷贝到目标文件
        await dbAssetStream.CopyToAsync(dbFileStream);

        // 存一下版本号
        _preferenceStorage.Set(PoetryStorageConstant.VersionKey, PoetryStorageConstant.Version);
    }

    public async Task<Poetry> GetPoetryAsync(int id) =>
        // 第一种方式尝试直接在数据库执行过滤，这可能需要数据库支持特定的查询优化
        await Connection.Table<Poetry>().FirstOrDefaultAsync(p => p.Id == id);
    // await Connection.Table<Poetry>().Where(p => p.Id == id).FirstOrDefaultAsync();
    // 这种方式是先获取所有记录，然后在应用程序中进行过滤，更通用，但可能会加载更多的数据到内存中

    public async Task<IEnumerable<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take) =>
        await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take).ToListAsync();
    // select * form Poetry where ...
    // skip 0, take 20 => page1
    // skip 20, take 20 => page2

    // 业务需求是不需要关闭数据库的，SqLite数据库是内嵌于程序中，无需考虑手动关闭数据库
    // 此方法只为单元测试使用
    public async Task CloseAsync()=>
        await Connection.CloseAsync();
}

// 存储诗词相关常量类，不能new
public static class PoetryStorageConstant
{
    public const int Version = 1;

    public const string VersionKey =
        nameof(PoetryStorageConstant) + "." + nameof(Version);
}
