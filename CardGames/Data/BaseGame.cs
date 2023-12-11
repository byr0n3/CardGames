namespace CardGames.Data
{
	// @todo Replace System.Guid with custom impl
	public class BaseGame : IGame
	{
		public string Code { get; }

		public int MinPlayers { get; }
		public int MaxPlayers { get; }
		public int PlayerCount { get; private set; }

		private readonly System.Guid[] players;

		public System.Guid Host =>
			this.players[0];

		public BaseGame(string code, int minPlayers, int maxPlayers)
		{
			this.Code = code;

			this.MinPlayers = minPlayers;
			this.MaxPlayers = maxPlayers;
			this.PlayerCount = 0;

			this.players = new System.Guid[this.MaxPlayers];
			this.players[this.PlayerCount++] = System.Guid.NewGuid();
		}

		public bool TryJoin(out System.Guid guid)
		{
			if (this.PlayerCount >= this.MaxPlayers)
			{
				guid = default;
				return false;
			}

			guid = System.Guid.NewGuid();

			this.players[this.PlayerCount++] = guid;
			return true;
		}

		public bool TryLeave(System.Guid guid)
		{
			if (!this.GetPlayerIndex(guid, out var idx))
			{
				return false;
			}

			if (idx == 0)
			{
				// @todo Player is host, delete game
				return true;
			}

			if (idx == this.PlayerCount - 1)
			{
				this.players[idx] = default;
				this.PlayerCount--;

				return true;
			}

			for (var i = idx; i < this.PlayerCount; i++)
			{
				this.players[idx] = this.players[idx + 1];
			}

			return true;
		}

		private bool GetPlayerIndex(System.Guid guid, out int idx)
		{
			for (var i = 0; i < this.PlayerCount; i++)
			{
				if (this.players[i] == guid)
				{
					idx = i;
					return true;
				}
			}

			idx = default;
			return false;
		}
	}
}
