using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NBitcoin;
using ReactiveUI;
using WalletWasabi.Blockchain.Analysis.Clustering;
using WalletWasabi.Fluent.Helpers;
using WalletWasabi.Fluent.TreeDataGrid;
using ScriptType = WalletWasabi.Fluent.Models.Wallets.ScriptType;

namespace WalletWasabi.Fluent.ViewModels.CoinControl.Core;

public abstract partial class CoinControlItemViewModelBase : ViewModelBase, ITreeDataGridExpanderItem
{
	private bool? _isSelected;

	[AutoNotify] private bool _isParentSelected;
	[AutoNotify] private bool _isParentPointerOver;
	[AutoNotify] private bool _isControlSelected;
	[AutoNotify] private bool _isControlPointerOver;

	protected CoinControlItemViewModelBase()
	{
		// Temporarily enable the selection no matter what.
		// Should be again restricted once https://github.com/zkSNACKs/WalletWasabi/issues/9972 is implemented.
		// CanBeSelected = !IsCoinjoining;
		CanBeSelected = true;

		this.WhenAnyValue(x => x.IsControlPointerOver)
			.Do(x =>
			{
				foreach (var child in Children)
				{
					child.IsParentPointerOver = x;
				}
			})
			.Subscribe();

		this.WhenAnyValue(x => x.IsControlSelected)
			.Do(x =>
			{
				foreach (var child in Children)
				{
					child.IsParentSelected = x;
				}
			})
			.Subscribe();
	}

	public bool IsPrivate => Labels == CoinPocketHelper.PrivateFundsText;

	public bool IsSemiPrivate => Labels == CoinPocketHelper.SemiPrivateFundsText;

	public bool IsNonPrivate => !IsSemiPrivate && !IsPrivate;

	public IReadOnlyCollection<CoinCoinControlItemViewModel> Children { get; protected set; } = new List<CoinCoinControlItemViewModel>();

	public bool IsConfirmed { get; protected set; }

	public bool IsCoinjoining { get; protected set; }

	public bool IsBanned { get; protected set; }

	public string ConfirmationStatus { get; protected set; } = "";

	public Money Amount { get; protected set; } = Money.Zero;

	public string? BannedUntilUtcToolTip { get; protected set; }

	public int? AnonymityScore { get; protected set; }

	public LabelsArray Labels { get; protected set; } = LabelsArray.Empty;

	public DateTimeOffset? BannedUntilUtc { get; protected set; }

	public bool IsExpanded { get; set; }

	public bool IsChild { get; set; }

	public bool IsLastChild { get; set; }

	public bool CanBeSelected { get; protected set; }

	public bool? IsSelected
	{
		get => _isSelected;
		set
		{
			if (!CanBeSelected && value == true)
			{
				return;
			}

			this.RaiseAndSetIfChanged(ref _isSelected, value);
		}
	}

	public ScriptType? ScriptType { get; protected set; }

	public virtual bool HasChildren() => Children.Count != 0;
}
