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

		public void OnJoinGame()
		{
			this.error = !this.GameManager.TryJoin(this.code, this.displayName, out this.game, out this.player);
		}
	}
}
