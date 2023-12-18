using CardGames.Core.Uno;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Pages
{
	public sealed partial class HostGame : ComponentBase
	{
		[Inject] public required GameManager<UnoGame, UnoPlayer> GameManager { get; init; }

		private string displayName = "Host";
		private bool error;
		private UnoGame? game;
		private UnoPlayer? player;

		private void OnHostGame()
		{
			this.error = !this.GameManager.TryHost(this.displayName, out this.game, out this.player);
		}

		private void OnLeaveGame()
		{
			this.error = false;

			this.game = null;
			this.player = null;

			// @todo Await?
			_ = this.InvokeAsync(this.StateHasChanged);
		}
	}
}
