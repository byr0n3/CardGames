using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public abstract class BaseGame<TPlayer> where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public delegate void VoidEvent();

		public event VoidEvent? OnLobbyStateChanged;
		public event VoidEvent? OnGameDestroyed;

		public readonly GameCode Code;

		public GameState State { get; private set; }
		protected int CurrentPlayerIndex { get; private set; }

		public readonly PlayerList<TPlayer> Players;
		private readonly int minPlayers;

		public TPlayer Host =>
			this.GetPlayer(0);

		public bool CanStart =>
			(this.State == GameState.Lobby) && (this.Players.Length >= this.minPlayers);

		protected BaseGame(GameCode code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.minPlayers = minPlayers;
			this.State = GameState.Lobby;

			this.Players = new PlayerList<TPlayer>(maxPlayers);
		}

		/// <summary>
		/// Get the <see cref="TPlayer"/> that is currently allowed to play
		/// </summary>
		/// <returns>The <see cref="TPlayer"/> that is currently allowed to play.</returns>
		/// <remarks>This function asserts that the <see cref="TPlayer"/> always exists and is valid.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPlayer GetCurrentPlayer() =>
			this.GetPlayer(this.CurrentPlayerIndex);

		/// <summary>
		/// Get the <see cref="TPlayer"/> at the given index.
		/// </summary>
		/// <param name="index">The index of the targeted <see cref="TPlayer"/>.</param>
		/// <returns>The <see cref="TPlayer"/> at the given index.</returns>
		/// <remarks>This function asserts that the <see cref="TPlayer"/> always exists and is valid.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPlayer GetPlayer(int index)
		{
			var player = this.Players[index];

			Debug.Assert(player is not null);

			return player;
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

		public bool TryLeave(TPlayer player)
		{
			if (!this.Players.TryLeave(player))
			{
				return false;
			}

			// Game is empty, no need to update any clients
			if (this.Players.Length == 0)
			{
				return true;
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

			this.OnNextTurn();
		}

		protected virtual void OnGameStarted()
		{
		}

		protected virtual void OnGameEnded()
		{
		}

		protected virtual void OnNextTurn()
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
