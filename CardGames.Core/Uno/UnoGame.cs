using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoGame : BaseGame<UnoPlayer>, IGame<UnoGame, UnoPlayer>
	{
		internal const int StartingCardCount = 7;

		private readonly CardDeck deck;

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
		}

		public void DrawCard(UnoPlayer player)
		{
			var card = this.deck.Draw();

			player.AddCard(card);

			// @todo Fire event

			if (this.deck.CardsLeft == 0)
			{
				this.RefillDeck();
			}
		}

		private void RefillDeck()
		{
			this.deck.Fill();

			this.deck.Shuffle();

			// @todo Fire event
		}

		public static UnoGame Create(GameCode code, int minPlayers, int maxPlayers) =>
			new(code, minPlayers, maxPlayers);
	}
}
