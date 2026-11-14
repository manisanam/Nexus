using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Fluently;

/// <summary>
///     A <see cref="UserControl" /> that is the view of <typeparamref name="TViewModel" />.
///     It keeps the view model in <see cref="ViewModel" /> and uses it as its <c>DataContext</c>.
///     Derive from it instead of implementing <see cref="IViewFor{TViewModel}" /> yourself:
/// </summary>
/// <typeparam name="TViewModel">The type of the view model this view shows.</typeparam>
public abstract class ViewFor<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
    public required ITransientCachedServiceProvider LazyServiceProvider { protected get; init; }

    protected ILoggerFactory LoggerFactory =>
        LazyServiceProvider.GetRequiredService<ILoggerFactory>();

    protected ILogger Logger =>
        LazyServiceProvider.GetService<ILogger>(_ =>
            LoggerFactory.CreateLogger(GetType().FullName!)
        );

    public new TViewModel DataContext
    {
        get =>
            base.DataContext as TViewModel
            ?? throw new InvalidCastException(
                $"DataContext is null or not of the expected type '{typeof(TViewModel).FullName}'."
            );
        set => base.DataContext = value;
    }

    /// <inheritdoc />
    public TViewModel ViewModel => DataContext;
}
