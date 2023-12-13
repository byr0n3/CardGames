using System.Diagnostics.CodeAnalysis;
using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public class BaseGame<TPlayer> : IGame<TPlayer>, System.IDisposable where TPlayer : class, IPlayer<TPlayer>
	{
		public GameCode Code { get; }

		public int MinPlayers { get; }

		public readonly PlayerList<TPlayer> Players;

		public event System.Action<TPlayer>? OnPlayerJoined;
		public event System.Action<TPlayer>? OnPlayerLeft;
		public event System.Action? OnGameDestroyed;

		public TPlayer Host =>
			this.Players[0];

		public BaseGame(GameCode code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.MinPlayers = minPlayers;

			this.Players = new PlayerList<TPlayer>(maxPlayers);
		}

		public bool TryJoin(System.ReadOnlySpan<char> name, [NotNullWhen(true)] out TPlayer? player)
		{
			if (!this.Players.TryJoin(name, out player))
			{
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

		public void Dispose()
		{
			this.OnGameDestroyed?.Invoke();

			System.GC.SuppressFinalize(this);
		}
	}
}
