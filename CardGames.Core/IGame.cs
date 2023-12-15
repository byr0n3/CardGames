using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public interface IGame<out TSelf, TPlayer> where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public GameCode Code { get; }
		public TPlayer Host { get; }

		public int MinPlayers { get; }

		public bool TryJoin(System.ReadOnlySpan<char> name, out TPlayer? player);
		public bool TryLeave(TPlayer player, out bool wasHost);

		static abstract TSelf Create(GameCode code, int minPlayers, int maxPlayers);
	}
}
