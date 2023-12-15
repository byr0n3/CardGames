using CardGames.Core.Uno;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Pages
{
	public sealed partial class JoinGame : ComponentBase
	{
		[Inject] public required GameManager<UnoGame, UnoPlayer> GameManager { get; init; }

		private string displayName = "Player";
		private string code = "";
		private bool error;
		private UnoGame? game;
		private UnoPlayer? player;

		private void OnJoinGame()
		{
			this.error = !this.GameManager.TryJoin(this.code, this.displayName, out this.game, out this.player);
		}

		private void OnLeaveGame()
		{
			this.code = "";
			this.error = false;

			this.game = null;
			this.player = null;

			// @todo Await?
			_ = this.InvokeAsync(this.StateHasChanged);
		}
	}
}
