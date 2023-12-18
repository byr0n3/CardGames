using JetBrains.Annotations;

namespace CardGames.Core.Uno.Extensions
{
	[PublicAPI]
	public static class CardExtensions
	{
		public static bool CanPlay(this Card @this, Card card)
		{
			// +2 and +4 cards are stackable
			if ((@this.Value is CardValue.DrawTwo or CardValue.DrawFour) &&
				(card.Value is CardValue.DrawTwo or CardValue.DrawFour))
			{
				return true;
			}

			return (@this.Color == CardColor.Wild) || (card.Color == CardColor.Wild) ||
				   (@this.Color == card.Color) || (@this.Value == card.Value);
		}
	}
}
