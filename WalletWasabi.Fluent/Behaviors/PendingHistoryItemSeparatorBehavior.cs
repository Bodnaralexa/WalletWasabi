using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using WalletWasabi.Fluent.ViewModels.Wallets.Home.History.HistoryItems;

namespace WalletWasabi.Fluent.Behaviors;

public class PendingHistoryItemSeparatorBehavior : AttachedToVisualTreeBehavior<TreeDataGridRowsPresenter>
{
	private const string ClassName = "separator";

	protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
	{
		if (AssociatedObject is { })
		{
			AssociatedObject.LayoutUpdated += AssociatedObjectOnLayoutUpdated;
		}
	}

	protected override void OnDetachedFromVisualTree()
	{
		if (AssociatedObject is { })
		{
			AssociatedObject.LayoutUpdated -= AssociatedObjectOnLayoutUpdated;
		}
	}

	private void AssociatedObjectOnLayoutUpdated(object? sender, EventArgs e)
	{
		if (AssociatedObject is not { } presenter)
		{
			return;
		}

		var children = AssociatedObject.GetVisualChildren();
		foreach (var child in children)
		{
			if (child is { })
			{
				InvalidateSeparator((Control)child, presenter);
			}
		}
	}

	private void InvalidateSeparator(Control control, TreeDataGridRowsPresenter presenter)
	{
		if (control.DataContext is not HistoryItemViewModelBase currentHistoryItem)
		{
			return;
		}

		if (currentHistoryItem.IsConfirmed)
		{
			if (control.Classes.Contains(ClassName))
			{
				control.Classes.Set(ClassName, false);
			}
		}
		else
		{
			if (IsSeparator(presenter, presenter.GetChildIndex(control)))
			{
				control.Classes.Set(ClassName, true);
			}
			else
			{
				if (control.Classes.Contains(ClassName))
				{
					control.Classes.Set(ClassName, false);
				}
			}
		}

		static bool IsSeparator(TreeDataGridRowsPresenter presenter, int index)
		{
			return presenter.Items is { } items
				   && items.Count > index + 1
				   && presenter.Items[index + 1].Model is HistoryItemViewModelBase { IsConfirmed: true };
		}
	}
}
