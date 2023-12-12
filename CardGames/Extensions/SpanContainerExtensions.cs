using CardGames.Utilities;

namespace CardGames.Extensions
{
	internal static class SpanContainerExtensions
	{
		public static string Str(this SpanContainer<char> @this) =>
			new(@this.AsSpan());
	}
}
