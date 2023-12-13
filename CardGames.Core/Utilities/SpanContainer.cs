using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	[StructLayout(LayoutKind.Sequential)]
	public readonly unsafe struct SpanContainer<T> : System.IEquatable<SpanContainer<T>> where T : unmanaged
	{
		private readonly T* ptr;
		private readonly int length;

		public SpanContainer()
		{
			this.ptr = null;
			this.length = 0;
		}

		public SpanContainer(System.ReadOnlySpan<T> span)
		{
			this.ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
			this.length = span.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.ReadOnlySpan<T> AsSpan(int size = 0)
		{
			if (size <= 0)
			{
				size = this.length;
			}

			return new System.ReadOnlySpan<T>(this.ptr, size);
		}

		public bool Equals(SpanContainer<T> other) =>
			System.MemoryExtensions.SequenceEqual(this.AsSpan(), other.AsSpan());

		public override bool Equals(object? @object)
		{
			return @object is SpanContainer<T> other && this.Equals(other);
		}

		// @todo Refactor
		public override int GetHashCode() =>
			System.HashCode.Combine(unchecked((int)(long)this.ptr), this.length);

		public static bool operator ==(SpanContainer<T> left, SpanContainer<T> right) =>
			left.Equals(right);

		public static bool operator !=(SpanContainer<T> left, SpanContainer<T> right) =>
			!left.Equals(right);

		public static implicit operator SpanContainer<T>(System.ReadOnlySpan<T> value) =>
			new(value);

		public static implicit operator System.ReadOnlySpan<T>(SpanContainer<T> value) =>
			value.AsSpan();
	}
}
