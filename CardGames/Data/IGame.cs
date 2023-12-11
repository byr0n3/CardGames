namespace CardGames.Data
{
	public interface IGame
	{
		public string Code { get; }
		public System.Guid Host { get; }

		public int MinPlayers { get; }
		public int MaxPlayers { get; }
		public int PlayerCount { get; }

		public bool TryJoin(out System.Guid guid);
		public bool TryLeave(System.Guid guid);
	}
}
