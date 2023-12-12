namespace CardGames.Data
{
	public interface IPlayer<out TSelf> where TSelf : IPlayer<TSelf>
	{
		public int Key { get; }

		static abstract TSelf Create(int key);
	}
}
