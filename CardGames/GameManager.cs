using System.Collections.Generic;
using System.Diagnostics;
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

		public BaseGame<BasePlayer> Host(System.ReadOnlySpan<char> name, [NotNullWhen(true)] out BasePlayer? player)
		{
			// @todo Make sure code isn't in active games already
			var game = new BaseGame<BasePlayer>(this.GenerateCode(4), 2, 8);

			this.games.Add(game);

			Debug.Assert(game.TryJoin(name, out player));

			this.logger.LogInformation("[{Code}] Game created", game.Code);

			return game;
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
		private GameCode GenerateCode(int length)
		{
			const string map = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

			var result = new GameCode(length);

			for (var i = 0; i < length; i++)
			{
				result.Append(map[this.rnd.Next(0, map.Length)]);
			}

			return result;
		}
	}
}
