namespace CardGames.Data
{
	public interface IGame<TPlayer> where TPlayer : IPlayer<TPlayer>
	{
		// @todo Custom Code struct
		public string Code { get; }
		public TPlayer Host { get; }

		public int MinPlayers { get; }

		public bool TryJoin(System.ReadOnlySpan<char> name, out TPlayer? player);
		public bool TryLeave(TPlayer player, out bool wasHost);
	}
}
