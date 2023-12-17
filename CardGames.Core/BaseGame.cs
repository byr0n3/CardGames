using System.Diagnostics.CodeAnalysis;
using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public abstract class BaseGame<TPlayer> : System.IDisposable where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public delegate void VoidEvent();

		public delegate void PlayerEvent(TPlayer player);

		public GameCode Code { get; }

		public int MinPlayers { get; }

		public GameState State { get; private set; }

		public readonly PlayerList<TPlayer> Players;

		public event PlayerEvent? OnPlayerJoined;
		public event PlayerEvent? OnPlayerLeft;
		public event VoidEvent? OnGameDestroyed;
		public event VoidEvent? OnGameStart;

		public TPlayer Host =>
			this.Players[0];

		public bool CanStart =>
			this.Players.Current >= this.MinPlayers;

		public BaseGame(GameCode code, int minPlayers, int maxPlayers)
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

			this.OnPlayerJoined?.Invoke(player);
			return true;
		}

		public bool TryLeave(TPlayer player, out bool wasHost)
		{
			if (!this.Players.TryLeave(player, out wasHost))
			{
				return false;
			}

			this.OnPlayerLeft?.Invoke(player);
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

			this.OnGameStart?.Invoke();

			return true;
		}

		public void Dispose()
		{
			this.OnGameDestroyed?.Invoke();

			System.GC.SuppressFinalize(this);
		}

		protected abstract void OnGameStarted();
	}
}
