using System.Collections.Generic;

namespace CardGames.Core.Uno
{
	internal sealed class CardDeck
	{
		private const int deckSize = 92;

		private static readonly CardColor[] cardColors = System.Enum.GetValues<CardColor>();
		private static readonly CardValue[] cardValues = System.Enum.GetValues<CardValue>();

		// @todo Custom array with position?
		private readonly List<Card> cards;
		private readonly System.Random rnd;

		public int CardsLeft =>
			this.cards.Count;

		public CardDeck()
		{
			this.rnd = new System.Random();
			this.cards = new List<Card>(CardDeck.deckSize);

			this.Fill();

			this.Shuffle();
		}

		public Card Draw()
		{
			if (this.cards.Count == 0)
			{
				return default;
			}

			var card = this.cards[0];

			this.cards.RemoveAt(0);

			return card;
		}

		public void Fill()
		{
			foreach (var color in CardDeck.cardColors)
			{
				if (color == CardColor.None)
				{
					continue;
				}

				CardDeck.AddCardsForColor(this.cards, color);
			}
		}

		[System.Obsolete("Better random")]
		public void Shuffle()
		{
			var i = this.cards.Count;
			while (i > 1)
			{
				i--;
				var k = this.rnd.Next(i + 1);
				(this.cards[k], this.cards[i]) = (this.cards[i], this.cards[k]);
			}
		}

		private static void AddCardsForColor(List<Card> cards, CardColor color)
		{
			if (color is CardColor.Wild)
			{
				// Add each wild card twice

				cards.Add(new Card(color, CardValue.Wild));
				cards.Add(new Card(color, CardValue.Wild));
				cards.Add(new Card(color, CardValue.DrawFour));
				cards.Add(new Card(color, CardValue.DrawFour));

				return;
			}

			CardDeck.AddValueCards(cards, color);
		}

		private static void AddValueCards(List<Card> cards, CardColor color)
		{
			foreach (var value in CardDeck.cardValues)
			{
				// ReSharper disable once ConvertIfStatementToSwitchStatement
				if (value is CardValue.None or CardValue.Wild or CardValue.DrawFour)
				{
					continue;
				}

				if (value is >= CardValue.One and <= CardValue.Nine)
				{
					cards.Add(new Card(color, value));
				}

				cards.Add(new Card(color, value));
			}
		}
	}
}
