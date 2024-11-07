using DailyPoetry.Models;
using System.Linq.Expressions;

namespace DailyPoetry.Services;

public class PoetryStorage : IPoetryStorage
{
    public bool IsInitialized
    {
        get
        {
            // Preferences.Get()获取当前数据库版本
            if (Preferences.Get(PoetryStorageConstant.VersionKey, 0) 
                == PoetryStorageConstant.Version)
                return true;
            else
                return false;
        }
    }

    public async Task InitializedAsync()
    {
        throw new NotImplementedException();
    }


    public async Task<Poetry> GetPoetryAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take){
        throw new NotImplementedException();
    }
}

// 存储诗词相关常量类，不能new
public static class PoetryStorageConstant
{
    // 程序正确运行所必须的数据库版本号
    public const int Version = 1;

    // 用偏好存储存app当前的数据库版本
    //public const string VersionKey = "PoetryStorageConstant.Version";
    public const string VersionKey = 
        nameof(PoetryStorageConstant) + "." + nameof(Version);

    // 当版本号不一致，说明需要更新当前软件的数据库版本
}
