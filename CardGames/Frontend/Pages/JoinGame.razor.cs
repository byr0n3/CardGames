using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Pages
{
	public sealed partial class JoinGame : ComponentBase
	{
		[Inject] public required GameManager GameManager { get; init; }

		private string displayName = "Player";
		private string code = "";
		private bool error;
		private BaseGame<BasePlayer>? game;
		private BasePlayer? player;

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
