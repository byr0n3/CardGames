using System.Globalization;
using System.Numerics;

namespace CardGames.Extensions
{
	internal static class NumericExtensions
	{
		public static string Str<T>(this T @this) where T : INumber<T> =>
			@this.ToString(null, NumberFormatInfo.InvariantInfo);
	}
}
