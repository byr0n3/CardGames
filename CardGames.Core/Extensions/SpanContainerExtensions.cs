using CardGames.Core.Utilities;

namespace CardGames.Core.Extensions
{
	public static class SpanContainerExtensions
	{
		public static string Str(this SpanContainer<char> @this) =>
			new(@this.AsSpan());
	}
}
