using System.Runtime.CompilerServices;
using CardGames.Core.Uno.Extensions;
using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoGame : BaseGame<UnoPlayer>, IGame<UnoGame>
	{
		internal const int StartingCardCount = 7;

		public event VoidEvent? OnGameStateChanged;

		public Card TopCard { get; private set; }
		public UnoGameFlags Flags { get; private set; }

		private readonly CardDeck deck;

		private int drawAmount;

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

			this.Flags = UnoGameFlags.None;

			this.drawAmount = 0;
		}

		protected override void OnGameEnded() =>
			this.OnGameStateChanged?.Invoke();

		protected override void OnNextTurn()
		{
			// If there wasn't any draw card played we can safely reset the game flags
			if ((this.Flags & UnoGameFlags.DrawNextPlayer) == UnoGameFlags.None)
			{
				this.Flags &= ~UnoGameFlags.ResetOnNextTurn;
				this.drawAmount = 0;

				return;
			}

			this.drawAmount += ((this.Flags & UnoGameFlags.DrawTwo) != UnoGameFlags.None) ? 2 : 4;
			var player = this.GetCurrentPlayer();

			// If the player has a draw card, he can play it to stack it
			if (player.HasDrawCard())
			{
				this.Flags &= ~UnoGameFlags.ResetOnNextTurn;

				return;
			}

			for (var i = 0; i < this.drawAmount; i++)
			{
				this.AddCardToPlayer(player);
			}

			this.Flags &= ~UnoGameFlags.DrawNextPlayer;

			// Skip the next player; if they have to draw, they don't get to play a card
			this.NextTurn(UnoGame.GetNextPlayerModifier(this.Flags));

			this.Flags &= ~UnoGameFlags.ResetOnNextTurn;

			this.drawAmount = 0;
		}

		protected override void OnPlayerLeft(UnoPlayer _)
		{
			// Adjust the current player to be a player that's actually in the game
			// If we don't the game will crash/be infinitely stuck if the current player leaves
			while (this.CurrentPlayerIndex >= this.Players.Length)
			{
				// @todo Add silent flag to not call `OnNextTurn`?
				this.NextTurn();
			}

			if (this.Players.Length == 1)
			{
				this.EndGame();

				return;
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

			// If the current card is a freshly played +2 or a +4
			// and the player doesn't play a +2 or +4 but does have one,
			// he's not allowed to play the card. He has to stack the draw card.
			if ((this.drawAmount > 0) &&
				(this.TopCard.Value is CardValue.DrawTwo or CardValue.DrawFour) &&
				((card.Value != CardValue.DrawTwo) && (card.Value != CardValue.DrawFour)) &&
				player.HasDrawCard())
			{
				return;
			}

			this.TopCard = card;

			player.RemoveCard(card);

			if (player.CardCount == 0)
			{
				this.EndGame();

				return;
			}

			this.HandleCardEffects(card.Value);

			// Don't go to the next turn if the player has to pick a color
			if ((this.Flags & UnoGameFlags.PickColor) == UnoGameFlags.None)
			{
				this.NextTurn(UnoGame.GetNextPlayerModifier(this.Flags));
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
				card = this.AddCardToPlayer(player);
			}

			// Don't go to the next turn here, player has to play the playable card

			this.OnGameStateChanged?.Invoke();
		}

		public void PickColor(UnoPlayer player, CardColor color)
		{
			if (!this.CanPickColor(player))
			{
				return;
			}

			this.TopCard = new Card(color, this.TopCard.Value);

			this.NextTurn(UnoGame.GetNextPlayerModifier(this.Flags));

			this.OnGameStateChanged?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanPlayCard(UnoPlayer player, Card card) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.GetCurrentPlayer() == player) &&
			// Player doesn't have to pick a color (Wild color or +4)
			((this.Flags & UnoGameFlags.PickColor) == UnoGameFlags.None) &&
			// Card can be played onto the current card
			this.TopCard.CanPlay(card) &&
			// Security check; player has the card they want to play
			player.HasCard(card);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanDrawCard(UnoPlayer player) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.GetCurrentPlayer() == player) &&
			// Player doesn't have to pick a color (Wild color or +4)
			((this.Flags & UnoGameFlags.PickColor) == UnoGameFlags.None) &&
			// Player doesn't have a playable card
			!player.HasPlayableCard(this.TopCard);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanPickColor(UnoPlayer player) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.GetCurrentPlayer() == player) &&
			// Game is waiting for player to pick a color
			((this.Flags & UnoGameFlags.PickColor) != UnoGameFlags.None);

		private void HandleCardEffects(CardValue card)
		{
			switch (card)
			{
				case CardValue.Reverse:
					// Toggle reversed flag
					this.Flags ^= UnoGameFlags.Reversed;
					break;

				case CardValue.Skip:
					this.Flags |= UnoGameFlags.SkipNext;
					break;

				case CardValue.Wild:
					this.Flags |= UnoGameFlags.PickColor;
					break;

				case CardValue.DrawTwo or CardValue.DrawFour:
				{
					this.Flags |= (card == CardValue.DrawTwo) ? UnoGameFlags.DrawTwo : UnoGameFlags.DrawFour;

					if (card == CardValue.DrawFour)
					{
						this.Flags |= UnoGameFlags.PickColor;
					}

					break;
				}
			}
		}

		private Card AddCardToPlayer(UnoPlayer player)
		{
			var card = this.deck.Draw();

			player.AddCard(card);

			if (this.deck.CardsLeft == 0)
			{
				this.deck.Fill();
			}

			return card;
		}

		private static int GetNextPlayerModifier(UnoGameFlags flags)
		{
			const int @default = 1;

			var result = ((flags & UnoGameFlags.Reversed) != UnoGameFlags.None ? -@default : @default);

			if ((flags & UnoGameFlags.SkipNext) != UnoGameFlags.None)
			{
				result *= 2;
			}

			return result;
		}

		public static UnoGame Create(GameCode code, int minPlayers, int maxPlayers) =>
			new(code, minPlayers, maxPlayers);
	}
}
