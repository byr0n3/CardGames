using System.Runtime.CompilerServices;
using CardGames.Core.Uno.Extensions;
using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoGame : BaseGame<UnoPlayer>, IGame<UnoGame>
	{
		internal const int StartingCardCount = 7;

		private readonly CardDeck deck;

		public Card TopCard { get; private set; }

		private bool reversed;

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

			this.reversed = false;
		}

		protected override void OnPlayerLeft(UnoPlayer _)
		{
			if (this.Players.Length == 1)
			{
				this.EndGame();

				this.OnGameStateChanged?.Invoke();
			}

			// If the game has finished and a player leaves,
			// there's no point in refreshing the UI
			if (this.State != GameState.Finished)
			{
				this.OnGameStateChanged?.Invoke();
			}
		}

		public void PlayCard(UnoPlayer player, Card card)
		{
			if ((this.State != GameState.InProgress) || (this.Players.GetIndex(player) != this.CurrentPlayerIndex) ||
				(card.IsDefault) || !this.TopCard.CanPlay(card) || !player.HasCard(card))
			{
				return;
			}

			this.TopCard = card;

			player.RemoveCard(card);

			if (card.Value == CardValue.Reverse)
			{
				this.reversed = !this.reversed;
			}

			if (player.CardCount == 0)
			{
				this.EndGame();
			}
			else
			{
				this.NextTurn(UnoGame.GetNextPlayerValue(card.Value, this.reversed));
			}

			this.OnGameStateChanged?.Invoke();
		}

		public void DrawCard(UnoPlayer player)
		{
			if ((this.State != GameState.InProgress) || (this.Players.GetIndex(player) != this.CurrentPlayerIndex))
			{
				return;
			}

			var card = this.deck.Draw();

			player.AddCard(card);

			if (this.deck.CardsLeft == 0)
			{
				this.RefillDeck();
			}

			this.NextTurn(UnoGame.GetNextPlayerValue(CardValue.None, this.reversed));

			this.OnGameStateChanged?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RefillDeck() =>
			this.deck.Fill();

		private static int GetNextPlayerValue(CardValue card, bool reversed)
		{
			const int @default = 1;

			var result = (reversed ? -@default : @default);

			if (card == CardValue.Skip)
			{
				result *= 2;
			}

			return result;
		}

		public static UnoGame Create(GameCode code, int minPlayers, int maxPlayers) =>
			new(code, minPlayers, maxPlayers);
	}
}
