using CardGames.Core.Utilities;

namespace CardGames.Core.Uno
{
	public sealed class UnoPlayer : BasePlayer, IPlayer<UnoPlayer>
	{
		public UnoPlayer(int key, SpanContainer<char> name) : base(key, name)
		{
		}

		public static UnoPlayer Create(int key, System.ReadOnlySpan<char> name) =>
			new(key, name);
	}
}
