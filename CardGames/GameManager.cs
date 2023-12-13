using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CardGames.Core;
using CardGames.Core.Extensions;
using CardGames.Core.Utilities;
using Microsoft.Extensions.Logging;

namespace CardGames
{
	public sealed class GameManager
	{
		private const int prefillAmount = 2;

		private readonly System.Random rnd;
		private readonly ILogger<GameManager> logger;
		private readonly List<BaseGame<BasePlayer>> games;

		public GameManager(ILogger<GameManager> logger)
		{
			this.logger = logger;
			this.rnd = new System.Random();
			this.games = new List<BaseGame<BasePlayer>>(GameManager.prefillAmount);
		}

		public bool TryHost(System.ReadOnlySpan<char> name,
							[NotNullWhen(true)] out BaseGame<BasePlayer>? game,
							[NotNullWhen(true)] out BasePlayer? player)
		{
			if (!this.TryGenerateUniqueCode(4, out var code))
			{
				game = null;
				player = null;
				return false;
			}

			game = new BaseGame<BasePlayer>(code, 2, 8);

			if (!game.TryJoin(name, out player))
			{
				return false;
			}

			this.games.Add(game);

			this.logger.LogInformation("[{Code}] Game created", game.Code);

			return true;
		}

		public bool TryJoin(System.ReadOnlySpan<char> code,
							System.ReadOnlySpan<char> name,
							[NotNullWhen(true)] out BaseGame<BasePlayer>? game,
							[NotNullWhen(true)] out BasePlayer? player)
		{
			foreach (var g in this.games)
			{
				if ((!g.Code.Equals(code)) || (!g.TryJoin(name, out player)))
				{
					continue;
				}

				this.logger.LogInformation("[{Code}] +{Name} ({Current}/{Max})",
										   new string(code),
										   new string(name),
										   g.Players.Current,
										   g.Players.Max);

				game = g;
				return true;
			}

			game = null;
			player = null;
			return false;
		}

		public void Leave(BaseGame<BasePlayer> game, BasePlayer player)
		{
			if (!game.TryLeave(player, out var wasHost))
			{
				return;
			}

			this.logger.LogInformation("[{Code}] -{Name} ({Current}/{Max})",
									   game.Code,
									   player.Name.Str(),
									   game.Players.Current,
									   game.Players.Max);

			if (!wasHost && (game.Players.Current > 0))
			{
				return;
			}

			this.logger.LogInformation("[{Code}] Game is empty, cleaning up!", game.Code);

			this.games.Remove(game);
			game.Dispose();
		}

		[System.Obsolete("Refactor")]
		private bool TryGenerateUniqueCode(int length, out GameCode result)
		{
			const int maxTries = 10;

			var tries = 0;
			var temp = new GameCode(length);
			this.FillCode(ref temp, length);

			while (this.games.Exists((game) => game.Code == temp) && tries <= maxTries)
			{
				temp.Reset();
				this.FillCode(ref temp, length);

				tries++;
			}

			result = temp;
			return tries <= maxTries;
		}

		[System.Obsolete("Refactor")]
		private void FillCode(ref GameCode code, int length)
		{
			const string map = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

			for (var i = 0; i < length; i++)
			{
				code.Append(map[this.rnd.Next(0, map.Length)]);
			}
		}
	}
}
