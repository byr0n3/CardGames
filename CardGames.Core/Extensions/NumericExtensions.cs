using System.Runtime.CompilerServices;

namespace CardGames.Core.Extensions
{
	public static class NumericExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetCharSize(this int @this) =>
			@this switch
			{
				>= 1000 => 4,
				>= 100  => 3,
				>= 10   => 2,
				_       => 1,
			};
	}
}
