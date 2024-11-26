using DailyPoetry.Library.Services;
using DailyPoetry.Library.ViewModels;

namespace DailyPoetry;

public class ServiceLocator
{
    private IServiceProvider _serviceProvider;

    public IPoetryStorage PoetryStorage =>
        _serviceProvider.GetService<IPoetryStorage>();

    public ResultPageViewModel ResultPageViewModel =>
        _serviceProvider.GetService<ResultPageViewModel>();

    public ServiceLocator()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IPoetryStorage, PoetryStorage>();

        serviceCollection.AddSingleton<ResultPageViewModel>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
