using DailyPoetry.Models;
using System.Linq.Expressions;

namespace DailyPoetry.Services;

public interface IPoetryStorage
{
    /// <summary>
    /// 诗词存储是否已经初始化
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// 初始化诗词存储
    /// </summary>
    // 因为需要做拷贝文件操作，所以设计为异步方法
    Task InitializedAsync();

    /// <summary>
    /// 按诗词id获取一首诗
    /// </summary>
    /// <param name="id">诗词id。</param>
    Task<Poetry> GetPoetryAsync(int id);

    // 返回一组诗词集合(用于组合查询)
    /// <summary>
    /// 获取满足给定条件的诗词集合。
    /// </summary>
    /// <param name="where">Where条件。</param>
    /// <param name="skip">跳过数量。</param>
    /// <param name="take">获取数量。</param>
    Task<IEnumerable<Poetry>> GetPoetriesAsync(
            Expression<Func<Poetry, bool>> where, int skip, int take);

}
