using CommunityToolkit.Mvvm.ComponentModel;
using Configuration.Writable;
using Fluently;
using Nexus.Settings;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Nexus.ViewModels;

public abstract partial class ViewModel
    : ObservableObject,
        IUnitOfWorkEnabled,
        IHasExtraProperties,
        ITransientDependency
{
    protected ViewModel()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public ITransientCachedServiceProvider LazyServiceProvider { protected get; set; } = null!;

    protected IServiceProvider ServiceProvider =>
        LazyServiceProvider.GetRequiredService<IServiceProvider>();

    protected ILoggerFactory LoggerFactory =>
        LazyServiceProvider.GetRequiredService<ILoggerFactory>();

    protected ILogger Logger =>
        LazyServiceProvider.GetService<ILogger>(_ =>
            LoggerFactory.CreateLogger(GetType().FullName!)
        );

    protected IUnitOfWorkManager UnitOfWorkManager =>
        LazyServiceProvider.GetRequiredService<IUnitOfWorkManager>();

    protected IClock Clock => LazyServiceProvider.GetRequiredService<IClock>();

    protected IGuidGenerator GuidGenerator =>
        LazyServiceProvider.GetRequiredService<IGuidGenerator>();

    protected ILocalEventBus LocalEventBus =>
        LazyServiceProvider.GetRequiredService<ILocalEventBus>();

    protected IWritableOptions<AppSettings> ApplicationOptions =>
        LazyServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();

    public INavigationService NavigationService =>
        LazyServiceProvider.GetRequiredService<INavigationService>();

    public IDialogService DialogService => LazyServiceProvider.GetRequiredService<IDialogService>();

    public IToastService ToastService => LazyServiceProvider.GetRequiredService<IToastService>();

    public ExtraPropertyDictionary ExtraProperties { get; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string BusyText { get; set; } = string.Empty;

    protected virtual async Task SetBusyAsync(
        Func<Task> func,
        string busyText = "",
        bool showException = true
    )
    {
        IsBusy = true;
        BusyText = busyText;
        try
        {
            await func();
        }
        catch (Exception ex) when (LogException(ex, true, showException))
        {
            // Not Used
        }
        finally
        {
            IsBusy = false;
            BusyText = string.Empty;
        }
    }

    protected bool LogException(Exception? ex, bool shouldCatch = false, bool shouldDisplay = false)
    {
        if (ex is null)
            return shouldCatch;

        Logger.LogException(ex);
        if (shouldDisplay)
        {
            //ToastService.ShowError("Error", ex.Message);
            //GlobalExceptionHandler.Show(ex);
        }

        return shouldCatch;
    }

    protected virtual void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    #region Navigation


    ///// <inheritdoc/>
    ///// <remarks>Called only the first time a view is created and shown. Default implementation does nothing.</remarks>
    //public virtual Task InitializeAsync(NavigationContext context) => Task.CompletedTask;

    ///// <inheritdoc/>
    ///// <remarks>Called every time the view becomes the active view. Default implementation does nothing.</remarks>
    //public virtual Task OnNavigatedToAsync(NavigationContext context) => Task.CompletedTask;

    ///// <inheritdoc/>
    ///// <remarks>Called when navigating away from this view. Default implementation does nothing.</remarks>
    //public virtual Task OnNavigatedFromAsync(NavigationContext context) => Task.CompletedTask;

    ///// <inheritdoc/>
    ///// <remarks>
    ///// Controls whether a cached view instance can be reused for the incoming navigation request.
    ///// Default returns <see langword="true"/>, meaning the cached instance is always reused.
    ///// Override and return <see langword="false"/> to force creation of a new instance.
    ///// </remarks>
    //public virtual Task<bool> IsNavigationTargetAsync(NavigationContext context) =>
    //    Task.FromResult(true);

    ///// <inheritdoc/>
    ///// <remarks>Called when the view is being removed from the region cache. Default implementation does nothing.</remarks>
    //public virtual Task OnUnloadAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    ///// <inheritdoc/>
    ///// <remarks>
    ///// Raise this event to request that the framework proactively removes this view from the region.
    ///// Use <see cref="RequestUnloadAsync"/> as a convenient helper to raise it.
    ///// </remarks>
    //public event AsyncEventHandler<AsyncEventArgs>? AsyncRequestUnloadEvent;

    ///// <summary>
    ///// Raises <see cref="AsyncRequestUnloadEvent"/> to request that the framework remove this view.
    ///// </summary>
    //protected Task RequestUnloadAsync(CancellationToken cancellationToken = default)
    //{
    //    var handler = AsyncRequestUnloadEvent;
    //    if (handler is not null)
    //        return handler(this, new AsyncEventArgs(cancellationToken));
    //    return Task.CompletedTask;
    //}
    #endregion
}
