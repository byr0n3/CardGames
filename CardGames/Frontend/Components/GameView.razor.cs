using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Components
{
	public sealed partial class GameView<TGame, TPlayer> : ComponentBase, System.IDisposable
		where TGame : BaseGame<TPlayer>, IGame<TGame, TPlayer>
		where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		[Inject] public required GameManager<TGame, TPlayer> GameManager { get; init; }

		[Parameter] public required TGame Game { get; set; }

		[Parameter] public required TPlayer CurrentPlayer { get; set; }

		[Parameter] public required System.Action OnLeaveGame { get; set; }

		protected override void OnInitialized()
		{
			// Subscribe to the game lobby events
			this.Game.OnPlayerJoined += this.Refresh;
			this.Game.OnPlayerLeft += this.Refresh;
			this.Game.OnGameStart += this.Refresh;
			this.Game.OnGameDestroyed += this.OnGameDestroyed;
		}

		// @todo Await?
		private void Refresh(BasePlayer _) =>
			this.InvokeAsync(this.StateHasChanged);

		// @todo Await?
		private void Refresh() =>
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
			this.Game.OnGameStart -= this.Refresh;
			this.Game.OnGameDestroyed -= this.OnGameDestroyed;

			// Signal the game that we're leaving
			this.GameManager.Leave(this.Game, this.CurrentPlayer);

			// Tell the parent component we left the game
			this.OnLeaveGame.Invoke();
		}

		private void StartGame() =>
			this.GameManager.TryStart(this.Game, this.CurrentPlayer);
	}
}
