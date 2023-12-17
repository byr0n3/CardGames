using System.Runtime.InteropServices;

namespace CardGames.Core.Uno
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct Card : System.IEquatable<Card>
	{
		public readonly CardColor Color;
		public readonly CardValue Value;

		public bool IsDefault =>
			this.Color == CardColor.None || this.Value == CardValue.None;

		public Card(CardColor color, CardValue value)
		{
			this.Color = color;
			this.Value = value;
		}

		public bool Equals(Card other) =>
			this.Color == other.Color && this.Value == other.Value;

		public override bool Equals(object? @object) =>
			@object is Card other && this.Equals(other);

		public override int GetHashCode() =>
			System.HashCode.Combine((int)this.Color, (int)this.Value);

		public static bool operator ==(Card left, Card right) =>
			left.Equals(right);

		public static bool operator !=(Card left, Card right) =>
			!left.Equals(right);
	}
}
