using CardGames.Core.Uno;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Components
{
	public sealed partial class UnoGameView : ComponentBase, System.IDisposable
	{
		[Parameter] public required UnoGame Game { get; set; }

		[Parameter] public required UnoPlayer CurrentPlayer { get; set; }

		[Parameter] public required System.Action OnLeaveGame { get; set; }

		protected override void OnInitialized()
		{
		}

		// @todo Await?
		private void Refresh() =>
			_ = this.InvokeAsync(this.StateHasChanged);

		public void Dispose()
		{
		}
	}
}
