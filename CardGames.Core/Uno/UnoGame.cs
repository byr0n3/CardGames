using System.Diagnostics;
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

		private UnoGameFlags flags;

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

			this.flags = UnoGameFlags.None;
		}

		protected override void OnNextTurn()
		{
			if ((this.flags & UnoGameFlags.DrawNextPlayer) == UnoGameFlags.None)
			{
				return;
			}

			var drawAmount = ((this.flags & UnoGameFlags.DrawTwo) != UnoGameFlags.None) ? 2 : 4;
			var player = this.Players[this.CurrentPlayerIndex];

			// @todo Valid check
			Debug.Assert(player is not null);

			for (var i = 0; i < drawAmount; i++)
			{
				this.AddCardToPlayer(player);
			}

			this.flags &= ~UnoGameFlags.DrawNextPlayer;

			this.NextTurn(UnoGame.GetNextPlayerModifier(this.flags));
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

			if (player.CardCount == 0)
			{
				this.EndGame();

				this.OnGameStateChanged?.Invoke();

				return;
			}

			this.HandleCardEffects(card.Value);

			// Don't go to the next turn if the player has to pick a color
			if ((this.flags & UnoGameFlags.PickColor) == UnoGameFlags.None)
			{
				this.NextTurn(UnoGame.GetNextPlayerModifier(this.flags));
			}

			this.flags &= ~UnoGameFlags.SkipNext;

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

			this.flags &= ~UnoGameFlags.PickColor;

			this.NextTurn(UnoGame.GetNextPlayerModifier(this.flags));

			this.OnGameStateChanged?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanPlayCard(UnoPlayer player, Card card) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.Players.GetIndex(player) == this.CurrentPlayerIndex) &&
			// Player doesn't have to pick a color (Wild color or +4)
			((this.flags & UnoGameFlags.PickColor) == UnoGameFlags.None) &&
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
			// Player doesn't have to pick a color (Wild color or +4)
			((this.flags & UnoGameFlags.PickColor) == UnoGameFlags.None) &&
			// Player doesn't have a playable card
			!player.HasPlayableCard(this.TopCard);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanPickColor(UnoPlayer player) =>
			// Game is in progress
			(this.State == GameState.InProgress) &&
			// It's the player's turn
			(this.Players.GetIndex(player) == this.CurrentPlayerIndex) &&
			// Game is waiting for player to pick a color
			((this.flags & UnoGameFlags.PickColor) != UnoGameFlags.None);

		private void HandleCardEffects(CardValue card)
		{
			switch (card)
			{
				case CardValue.Reverse:
					// Toggle reversed flag
					this.flags ^= UnoGameFlags.Reversed;
					break;

				case CardValue.Skip:
					this.flags |= UnoGameFlags.SkipNext;
					break;

				case CardValue.Wild:
					this.flags |= UnoGameFlags.PickColor;
					break;

				case CardValue.DrawTwo or CardValue.DrawFour:
				{
					this.flags |= (card == CardValue.DrawTwo) ? UnoGameFlags.DrawTwo : UnoGameFlags.DrawFour;

					if (card == CardValue.DrawFour)
					{
						this.flags |= UnoGameFlags.PickColor;
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
