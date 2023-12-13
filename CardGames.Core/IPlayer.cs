using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public interface IPlayer<out TSelf> where TSelf : IPlayer<TSelf>
	{
		public int Key { get; }

		public SpanContainer<char> Name { get; }

		static abstract TSelf Create(int key, System.ReadOnlySpan<char> name);
	}
}
