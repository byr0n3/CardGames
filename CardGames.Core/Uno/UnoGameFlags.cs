namespace CardGames.Core.Uno
{
	[System.Flags]
	public enum UnoGameFlags
	{
		None = 0,
		Reversed = 1 << 0,
		SkipNext = 1 << 1,
		DrawTwo = 1 << 2,
		DrawFour = 1 << 3,
		PickColor = 1 << 4,

		DrawNextPlayer = UnoGameFlags.DrawTwo | UnoGameFlags.DrawFour,
		ResetOnNextTurn = UnoGameFlags.SkipNext | UnoGameFlags.PickColor | UnoGameFlags.DrawNextPlayer,
	}
}
