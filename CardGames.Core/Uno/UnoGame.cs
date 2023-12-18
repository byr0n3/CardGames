using CardGames.Core.Uno.Extensions;
using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoGame : BaseGame<UnoPlayer>, IGame<UnoGame, UnoPlayer>
	{
		internal const int StartingCardCount = 7;

		private readonly CardDeck deck;

		public Card TopCard { get; private set; }

		private int currentPlayerKey;

		public event VoidEvent? OnGameStateChanged;

		private UnoGame(GameCode code, int minPlayers, int maxPlayers) : base(code, minPlayers, maxPlayers)
		{
			this.deck = new CardDeck();
		}

		protected override void OnGameStarted()
		{
			foreach (var player in this.Players)
			{
				for (var i = 0; i < UnoGame.StartingCardCount; i++)
				{
					player.AddCard(this.deck.Draw());
				}
			}

			this.TopCard = this.deck.Draw();

			// @todo Start at random player
			this.currentPlayerKey = this.Host.Key;
		}

		protected override void OnPlayerLeft(UnoPlayer player)
		{
			if (this.Players.Current == 0)
			{
				// @todo Set winner

				this.EndGame();
			}
		}

		public void PlayCard(UnoPlayer player, Card card)
		{
			if ((player.Key != this.currentPlayerKey) || card.IsDefault ||
				!this.TopCard.CanPlay(card) || !player.HasCard(card))
			{
				return;
			}

			this.TopCard = card;

			player.RemoveCard(card);

			this.NextTurn(player);

			if (player.CardCount == 0)
			{
				// @todo Set winner
			}

			this.OnGameStateChanged?.Invoke();
		}

		public void DrawCard(UnoPlayer player)
		{
			if (player.Key != this.currentPlayerKey)
			{
				return;
			}

			var card = this.deck.Draw();

			player.AddCard(card);

			if (this.deck.CardsLeft == 0)
			{
				this.RefillDeck();
			}

			this.NextTurn(player);

			this.OnGameStateChanged?.Invoke();
		}

		private void RefillDeck()
		{
			this.deck.Fill();

			this.deck.Shuffle();
		}

		// @todo Refactor
		[System.Obsolete("Refactor")]
		private void NextTurn(UnoPlayer previousPlayer)
		{
			var isNext = false;

			foreach (var player in this.Players)
			{
				if (isNext)
				{
					this.currentPlayerKey = player.Key;
					return;
				}

				if (player != previousPlayer)
				{
					continue;
				}

				isNext = true;
			}

			this.currentPlayerKey = this.Players[0].Key;
		}

		public static UnoGame Create(GameCode code, int minPlayers, int maxPlayers) =>
			new(code, minPlayers, maxPlayers);
	}
}
