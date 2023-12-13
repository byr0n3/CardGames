using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Pages
{
	public sealed partial class HostGame : ComponentBase
	{
		[Inject] public required GameManager GameManager { get; init; }

		private string displayName = "Player";
		private BaseGame<BasePlayer>? game;
		private BasePlayer? player;

		public void OnHostGame()
		{
			this.game = this.GameManager.Host(this.displayName, out this.player);
		}
	}
}
