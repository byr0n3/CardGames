using JetBrains.Annotations;

namespace CardGames.Core.Uno
{
	[PublicAPI]
	public enum CardValue
	{
		None = 0,
		Zero,
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		Eight,
		Nine,
		Reverse,
		Skip,
		DrawTwo,
		DrawFour,
		Wild,
	}
}
