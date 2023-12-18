using System.Runtime.CompilerServices;
using CardGames.Core.Utilities;

namespace CardGames.Core.Extensions
{
	public static class SpanContainerExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Str(this SpanContainer<char> @this) =>
			new(@this.AsSpan());
	}
}
