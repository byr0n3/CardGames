using System.Diagnostics.CodeAnalysis;
using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public abstract class BaseGame<TPlayer> where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public delegate void VoidEvent();

		public GameCode Code { get; }

		public int MinPlayers { get; }

		public GameState State { get; private set; }

		public readonly PlayerList<TPlayer> Players;

		public event VoidEvent? OnLobbyStateChanged;
		public event VoidEvent? OnGameDestroyed;

		public TPlayer Host =>
			this.Players[0];

		public bool CanStart =>
			this.Players.Current >= this.MinPlayers;

		protected BaseGame(GameCode code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.MinPlayers = minPlayers;
			this.State = GameState.Lobby;

			this.Players = new PlayerList<TPlayer>(maxPlayers);
		}

		public bool TryJoin(System.ReadOnlySpan<char> name, [NotNullWhen(true)] out TPlayer? player)
		{
			if ((this.State != GameState.Lobby) || !this.Players.TryJoin(name, out player))
			{
				player = null;
				return false;
			}

			this.OnPlayerJoined(player);

			this.OnLobbyStateChanged?.Invoke();

			return true;
		}

		public bool TryLeave(TPlayer player, out bool wasHost)
		{
			if (!this.Players.TryLeave(player, out wasHost))
			{
				return false;
			}

			this.OnPlayerLeft(player);

			this.OnLobbyStateChanged?.Invoke();

			return true;
		}

		// @todo Abstract class
		public bool TryStart(TPlayer player)
		{
			if (!this.CanStart || (this.Host != player))
			{
				return false;
			}

			this.State = GameState.InProgress;

			this.OnGameStarted();

			this.OnLobbyStateChanged?.Invoke();

			return true;
		}

		public void EndGame()
		{
			this.OnGameDestroyed?.Invoke();
		}

		protected virtual void OnGameStarted()
		{
		}

		protected virtual void OnPlayerJoined(TPlayer player)
		{
		}

		protected virtual void OnPlayerLeft(TPlayer player)
		{
		}
	}
}
