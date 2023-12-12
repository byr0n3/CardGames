using CardGames.Utilities;

namespace CardGames.Data
{
	public class BaseGame<TPlayer> : IGame<TPlayer> where TPlayer : class, IPlayer<TPlayer>
	{
		public string Code { get; }

		public int MinPlayers { get; }

		private readonly PlayerList<TPlayer> players;

		public int MaxPlayers =>
			this.players.Max;

		public int PlayerCount =>
			this.players.Current;

		public TPlayer Host =>
			this.players[0];

		public BaseGame(string code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.MinPlayers = minPlayers;

			this.players = new PlayerList<TPlayer>(maxPlayers);
		}

		public bool TryJoin(out TPlayer? player) =>
			this.players.TryJoin(out player);

		public bool TryLeave(TPlayer player) =>
			this.players.TryLeave(player);
	}
}
