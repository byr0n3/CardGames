using JetBrains.Annotations;

namespace CardGames.Core
{
	[PublicAPI]
	public enum GameState
	{
		None = 0,
		Lobby,
		InProgress,
		Finished,
	}
}
