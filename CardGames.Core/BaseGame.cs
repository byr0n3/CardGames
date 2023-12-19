using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public abstract class BaseGame<TPlayer> where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public delegate void VoidEvent();

		public readonly GameCode Code;

		public GameState State { get; private set; }

		public int CurrentPlayerIndex { get; private set; }

		public readonly PlayerList<TPlayer> Players;
		private readonly int minPlayers;

		public event VoidEvent? OnLobbyStateChanged;
		public event VoidEvent? OnGameDestroyed;

		// We can assume the player isn't null here,
		// the game would've been deleted if so.
		public TPlayer Host =>
			this.Players[0]!;

		public bool CanStart =>
			(this.State == GameState.Lobby) && (this.Players.Length >= this.minPlayers);

		protected BaseGame(GameCode code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.minPlayers = minPlayers;
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
				wasHost = false;
				return false;
			}

			this.OnPlayerLeft(player);

			if (this.State == GameState.Lobby)
			{
				this.OnLobbyStateChanged?.Invoke();
			}

			return true;
		}

		public bool TryStart(TPlayer player)
		{
			if (!this.CanStart || (this.Host != player))
			{
				return false;
			}

			this.State = GameState.InProgress;

			// @todo Start at random player
			this.CurrentPlayerIndex = 0;

			this.OnGameStarted();

			this.OnLobbyStateChanged?.Invoke();

			return true;
		}

		public void CancelGame()
		{
			if (this.Players.Length != 0)
			{
				return;
			}

			this.OnGameDestroyed?.Invoke();
		}

		protected void EndGame()
		{
			this.State = GameState.Finished;

			this.OnGameEnded();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void NextTurn(int skipAmount = 1)
		{
			var mod = (this.CurrentPlayerIndex + skipAmount) % this.Players.Length;

			this.CurrentPlayerIndex = mod < 0 ? mod + this.Players.Length : mod;
		}

		protected virtual void OnGameStarted()
		{
		}

		protected virtual void OnGameEnded()
		{
		}

		protected virtual void OnPlayerJoined(TPlayer _)
		{
		}

		protected virtual void OnPlayerLeft(TPlayer _)
		{
		}
	}
}
