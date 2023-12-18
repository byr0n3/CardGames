using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoPlayer : BasePlayer, IPlayer<UnoPlayer>
	{
		private readonly List<Card> hand;

		public int CardCount =>
			this.hand.Count;

		private UnoPlayer(int key, SpanContainer<char> name) : base(key, name)
		{
			this.hand = new List<Card>(UnoGame.StartingCardCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddCard(Card card) =>
			this.hand.Add(card);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RemoveCard(Card card) =>
			this.hand.Remove(card);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool HasCard(Card card) =>
			this.hand.Contains(card);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public List<Card>.Enumerator CardsEnumerator() =>
			this.hand.GetEnumerator();

		public static UnoPlayer Create(int key, System.ReadOnlySpan<char> name) =>
			new(key, name);
	}
}
