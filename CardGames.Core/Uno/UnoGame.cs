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
			if (!this.CanPlayCard(player, card))
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
			if (!this.CanDrawCard(player))
			{
				return;
			}

			Card card = default;

			while (!this.TopCard.CanPlay(card))
			{
				card = this.deck.Draw();

				player.AddCard(card);

				if (this.deck.CardsLeft == 0)
				{
					this.deck.Fill();
				}
			}

			// Don't go to the next turn here, player has to play the playable card

			this.OnGameStateChanged?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanPlayCard(UnoPlayer player, Card card) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.Players.GetIndex(player) == this.CurrentPlayerIndex) &&
			// Card can be played onto the current card
			this.TopCard.CanPlay(card) &&
			// Security check; player has the card they want to play
			player.HasCard(card);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanDrawCard(UnoPlayer player) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.Players.GetIndex(player) == this.CurrentPlayerIndex) &&
			// Player doesn't have a playable card
			!player.HasPlayableCard(this.TopCard);

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
