namespace CardGames.Core
{
	public interface IPlayer<out TSelf> where TSelf : IPlayer<TSelf>
	{
		static abstract TSelf Create(int key, System.ReadOnlySpan<char> name);
	}
}
