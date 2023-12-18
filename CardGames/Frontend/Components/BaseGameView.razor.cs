using CardGames.Core;
using Microsoft.AspNetCore.Components;

namespace CardGames.Frontend.Components
{
	public sealed partial class BaseGameView<TGame, TPlayer> : ComponentBase, System.IDisposable
		where TGame : BaseGame<TPlayer>, IGame<TGame, TPlayer>
		where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		[Inject] public required GameManager<TGame, TPlayer> GameManager { get; init; }

		[Parameter] public required TGame Game { get; set; }

		[Parameter] public required TPlayer CurrentPlayer { get; set; }

		[Parameter] public required System.Action OnLeaveGame { get; set; }

		[Parameter] public required RenderFragment ChildContent { get; set; }

		protected override void OnInitialized()
		{
			this.Game.OnLobbyStateChanged += this.Refresh;
			this.Game.OnGameDestroyed += this.OnGameDestroyed;
		}

		// @todo Await?
		private void Refresh() =>
			_ = this.InvokeAsync(this.StateHasChanged);

		private void OnGameDestroyed() =>
			this.LeaveGame();

		public void Dispose() =>
			this.LeaveGame();

		private void StartGame() =>
			this.GameManager.TryStart(this.Game, this.CurrentPlayer);

		private void LeaveGame()
		{
			this.Game.OnLobbyStateChanged -= this.Refresh;
			this.Game.OnGameDestroyed -= this.OnGameDestroyed;

			this.GameManager.Leave(this.Game, this.CurrentPlayer);

			this.OnLeaveGame.Invoke();
		}
	}
}
