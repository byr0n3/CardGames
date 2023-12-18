using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CardGames.Core;
using CardGames.Core.Extensions;
using CardGames.Core.Utilities;
using CardGames.Extensions;
using Microsoft.Extensions.Logging;

namespace CardGames
{
	public sealed class GameManager<TGame, TPlayer> where TGame : BaseGame<TPlayer>, IGame<TGame, TPlayer>
													where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		private const int prefillAmount = 2;

		private readonly System.Random rnd;
		private readonly ILogger<GameManager<TGame, TPlayer>> logger;
		private readonly List<TGame> games;

		public GameManager(ILogger<GameManager<TGame, TPlayer>> logger)
		{
			this.logger = logger;
			this.rnd = new System.Random();
			this.games = new List<TGame>(GameManager<TGame, TPlayer>.prefillAmount);
		}

		public bool TryHost(System.ReadOnlySpan<char> name,
							[NotNullWhen(true)] out TGame? game,
							[NotNullWhen(true)] out TPlayer? player)

		{
			if (!this.TryGenerateUniqueCode(4, out var code))
			{
				game = null;
				player = null;
				return false;
			}

			game = TGame.Create(code, 2, 8);

			if (!game.TryJoin(name, out player))
			{
				game = null;
				return false;
			}

			this.games.Add(game);

			this.logger.LogInformation("[{Code}] {Game} created", game.Code.ToString(), typeof(TGame).Name);

			return true;
		}

		public bool TryJoin(System.ReadOnlySpan<char> code,
							System.ReadOnlySpan<char> name,
							[NotNullWhen(true)] out TGame? game,
							[NotNullWhen(true)] out TPlayer? player)
		{
			foreach (var g in this.games)
			{
				if ((!g.Code.Equals(code)) || (!g.TryJoin(name, out player)))
				{
					continue;
				}

				game = g;

				this.logger.LogInformation("[{Code}] +{Name} ({Current}/{Max})",
										   game.Code.ToString(),
										   player.Name.Str(),
										   g.Players.Length.Str(),
										   g.Players.Capacity.Str());

				return true;
			}

			game = null;
			player = null;
			return false;
		}

		public void Leave(TGame game, TPlayer player)
		{
			if (!game.TryLeave(player, out var wasHost))
			{
				return;
			}

			this.logger.LogInformation("[{Code}] -{Name} ({Current}/{Max})",
									   game.Code.ToString(),
									   player.Name.Str(),
									   game.Players.Length.Str(),
									   game.Players.Capacity.Str());

			if (!wasHost && (game.Players.Length > 0))
			{
				return;
			}

			this.logger.LogInformation("[{Code}] Game is empty, cleaning up!", game.Code.ToString());

			this.games.Remove(game);
			game.CancelGame();
		}

		public bool TryStart(TGame game, TPlayer player)
		{
			if (!game.TryStart(player))
			{
				return false;
			}

			this.logger.LogInformation("[{Code}] Game has started", game.Code.ToString());

			return true;
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
