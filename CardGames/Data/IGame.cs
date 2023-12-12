namespace CardGames.Data
{
	public interface IGame<TPlayer> where TPlayer : IPlayer<TPlayer>
	{
		// @todo Custom Code struct
		public string Code { get; }
		public TPlayer Host { get; }

		public int MinPlayers { get; }
		public int MaxPlayers { get; }
		public int PlayerCount { get; }

		public bool TryJoin(out TPlayer? player);
		public bool TryLeave(TPlayer player);
	}
}
