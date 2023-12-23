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
		public readonly int Length;

		[System.Obsolete("Don't use default constructor", true)]
		public SpanContainer()
		{
		}

		public SpanContainer(System.ReadOnlySpan<T> span)
		{
			this.ptr = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
			this.Length = span.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.ReadOnlySpan<T> AsSpan(int length = 0)
		{
			if (length <= 0)
			{
				length = this.Length;
			}

			return new System.ReadOnlySpan<T>(this.ptr, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.ReadOnlySpan<T> AsSpan(int start, int length)
		{
			if (start < 0)
			{
				start = 0;
			}

			if (length <= 0)
			{
				length = this.Length;
			}

			return new System.ReadOnlySpan<T>(this.ptr, this.Length).Slice(start, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.Span<T> AsWritableSpan(int length = 0)
		{
			if (length <= 0)
			{
				length = this.Length;
			}

			return new System.Span<T>(this.ptr, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public System.Span<T> AsWritableSpan(int start, int length)
		{
			if (start < 0)
			{
				start = 0;
			}

			if (length <= 0)
			{
				length = this.Length;
			}

			return new System.Span<T>(this.ptr, this.Length).Slice(start, length);
		}

		public bool Equals(SpanContainer<T> other) =>
			System.MemoryExtensions.SequenceEqual(this.AsSpan(), other.AsSpan());

		public override bool Equals(object? @object) =>
			@object is SpanContainer<T> other && this.Equals(other);

		// @todo Refactor
		public override int GetHashCode() =>
			System.HashCode.Combine(unchecked((int)(long)this.ptr), this.Length);

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
