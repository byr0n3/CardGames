using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Pages
{
	public sealed partial class HostGame : ComponentBase
	{
		[Inject] public required GameManager GameManager { get; init; }

		private string displayName = "Player";
		private bool error;
		private BaseGame<BasePlayer>? game;
		private BasePlayer? player;

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
