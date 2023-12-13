using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public interface IGame<TPlayer> where TPlayer : IPlayer<TPlayer>
	{
		public GameCode Code { get; }
		public TPlayer Host { get; }

		public int MinPlayers { get; }

		public bool TryJoin(System.ReadOnlySpan<char> name, out TPlayer? player);
		public bool TryLeave(TPlayer player, out bool wasHost);
	}
}
