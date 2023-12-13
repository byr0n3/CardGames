using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Components
{
	public sealed partial class GameView : ComponentBase, System.IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

		[Inject] public required NavigationManager NavigationManager { get; init; }

		[Parameter] public required BaseGame<BasePlayer> Game { get; set; }

		[Parameter] public required BasePlayer CurrentPlayer { get; set; }

		protected override void OnInitialized()
		{
			this.Game.OnPlayerJoined += this.Refresh;
			this.Game.OnPlayerLeft += this.Refresh;
			this.Game.OnGameDestroyed += this.OnGameDestroyed;
		}

		// @todo Await sync?
		private void Refresh(BasePlayer _) =>
			this.InvokeAsync(this.StateHasChanged);

		// @todo Update UI instead of refresh
		private void OnGameDestroyed() =>
			this.NavigationManager.Refresh(true);

		// @todo Update UI instead of refresh
		private void LeaveGame() =>
			this.NavigationManager.Refresh(true);

		public void Dispose()
		{
			this.Game.OnPlayerJoined -= this.Refresh;
			this.Game.OnPlayerLeft -= this.Refresh;
			this.Game.OnGameDestroyed -= this.OnGameDestroyed;

			this.GameManager.Leave(this.Game, this.CurrentPlayer);
		}
	}
}
