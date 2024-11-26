using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyPoetry.Library.Models;
using DailyPoetry.Library.Services;
using System.Linq.Expressions;
using TheSalLab.MauiInfiniteScrolling;

namespace DailyPoetry.Library.ViewModels;

public class ResultPageViewModel : ObservableObject
{
    private Expression<Func<Poetry, bool>> _where;
    public Expression<Func<Poetry, bool>> Where 
    {
        get =>_where;
        set
        {
            if (value != _where)
            {
                _canLoadMore = true;
            }
            SetProperty(ref _where, value);
        }
    }

    // 类似ObservableCollection做的封装，属性值的改变可以通知到前台，并且支持无限滚动
    public MauiInfiniteScrollCollection<Poetry> Poetries { get; }

    public ResultPageViewModel(IPoetryStorage poetryStorage)
    {
        Poetries = new MauiInfiniteScrollCollection<Poetry>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Status = Loading;
                var poetries = 
                    (await poetryStorage.GetPoetriesAsync(Where, Poetries.Count, PageSize)).ToList();

                if (poetries.Count < PageSize)
                {
                    // 没有更多结果
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }
                if (Poetries.Count == 0 && poetries.Count == 0)
                {
                    // 没有满足条件的结果
                    _canLoadMore = false;
                    Status = NoResult;
                }
                return poetries;
            }
        };
    }

    public const int PageSize = 20;

    private string _status;
    public string Status
    {
        get => _status; 
        set => SetProperty(ref _status, value);
    }

    private bool _canLoadMore;
    public const string Loading = "正在加载";
    public const string NoResult = "没有满足条件的结果";
    public const string NoMoreResult = "没有更多结果";

    private RelayCommand _navigatedToCommand;

    public RelayCommand NavigatedToCommand =>
        _navigatedToCommand ??= new RelayCommand(async () =>
            {
                Poetries.Clear();
                await Poetries.LoadMoreAsync();
            });
}
