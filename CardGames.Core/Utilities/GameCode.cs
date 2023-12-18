using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	public struct GameCode : System.IEquatable<GameCode>
	{
		private readonly char[] data;
		private int position;

		public GameCode(int size)
		{
			this.data = new char[size];
			this.position = 0;
		}

		public void Append(char @char)
		{
			if (this.position >= this.data.Length)
			{
				return;
			}

			this.data[this.position++] = @char;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset() =>
			this.position = 0;

		public readonly System.ReadOnlySpan<char> AsSpan(int length = 0)
		{
			if (length <= 0 || length > this.position)
			{
				length = this.position;
			}

			return System.MemoryExtensions.AsSpan(this.data, 0, length);
		}

		public readonly override string ToString() =>
			new(this.AsSpan());

		public readonly bool Equals(GameCode other) =>
			System.MemoryExtensions.SequenceEqual(this.AsSpan(), other.AsSpan());

		public readonly bool Equals(System.ReadOnlySpan<char> other) =>
			System.MemoryExtensions.SequenceEqual(this.AsSpan(), other);

		public readonly override bool Equals(object? @object) =>
			@object is GameCode other && this.Equals(other);

		public readonly override int GetHashCode() =>
			System.HashCode.Combine(this.data, this.position);

		public static implicit operator System.ReadOnlySpan<char>(GameCode value) =>
			value.AsSpan();

		public static bool operator ==(GameCode left, GameCode right) =>
			left.Equals(right);

		public static bool operator !=(GameCode left, GameCode right) =>
			!left.Equals(right);
	}
}
