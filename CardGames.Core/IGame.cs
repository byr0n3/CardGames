using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public interface IGame<out TSelf> where TSelf : IGame<TSelf>
	{
		static abstract TSelf Create(GameCode code, int minPlayers, int maxPlayers);
	}
}
