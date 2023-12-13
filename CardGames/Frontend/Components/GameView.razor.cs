using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Components
{
	public sealed partial class GameView : ComponentBase, System.IDisposable
	{
		[Inject] public required GameManager GameManager { get; init; }

		[Parameter] public required BaseGame<BasePlayer> Game { get; set; }

		[Parameter] public required BasePlayer CurrentPlayer { get; set; }

		[Parameter] public required System.Action OnLeaveGame { get; set; }

		protected override void OnInitialized()
		{
			// Subscribe to the game lobby events
			this.Game.OnPlayerJoined += this.Refresh;
			this.Game.OnPlayerLeft += this.Refresh;
			this.Game.OnGameDestroyed += this.OnGameDestroyed;
		}

		// @todo Await?
		private void Refresh(BasePlayer _) =>
			this.InvokeAsync(this.StateHasChanged);

		private void OnGameDestroyed() =>
			this.LeaveGame();

		public void Dispose() =>
			this.LeaveGame();

		private void LeaveGame()
		{
			// Unsubscribe from the game lobby events
			this.Game.OnPlayerJoined -= this.Refresh;
			this.Game.OnPlayerLeft -= this.Refresh;
			this.Game.OnGameDestroyed -= this.OnGameDestroyed;

			// Signal the game that we're leaving
			this.GameManager.Leave(this.Game, this.CurrentPlayer);

			// Tell the parent component we left the game
			this.OnLeaveGame.Invoke();
		}
	}
}
