using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	[StructLayout(LayoutKind.Sequential)]
	[DebuggerTypeProxy(typeof(CharStringBuilder.DebugView))]
	public struct CharStringBuilder
	{
		private readonly SpanContainer<char> span;

		private int position;

		[System.Obsolete("Don't use default constructor", true)]
		public CharStringBuilder()
		{
		}

		public CharStringBuilder(scoped System.Span<char> span)
		{
			this.span = new SpanContainer<char>(span);

			this.position = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char value) =>
			this.Take(1)[0] = value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char value, int count) =>
			this.Take(count).Fill(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(scoped System.ReadOnlySpan<char> value) =>
			value.CopyTo(this.Take(value.Length));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append<T>(T value) where T : unmanaged, INumber<T>, System.ISpanFormattable =>
			this.AppendFormattable(value, default, NumberFormatInfo.InvariantInfo);

		public void AppendFormattable<T>(T value,
										 scoped System.ReadOnlySpan<char> format = default,
										 System.IFormatProvider? provider = default)
			where T : unmanaged, System.ISpanFormattable
		{
			var slice = this.span.AsWritableSpan(this.position, this.span.Length - this.position);

			if (!value.TryFormat(slice, out var written, format, provider ?? CultureInfo.InvariantCulture))
			{
				return;
			}

			this.Advance(written);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private readonly System.Span<char> Slice(int length)
		{
			Debug.Assert(this.span.Length >= this.position + length);

			return this.span.AsWritableSpan(this.position, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private System.Span<char> Take(int length)
		{
			var slice = this.Slice(length);

			this.Advance(length);

			return slice;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Advance(int length) =>
			this.position += length;

		public readonly override string ToString() =>
			new(this.span);

		private sealed class DebugView
		{
			public readonly string String;

			public DebugView(CharStringBuilder @this)
			{
				this.String = @this.ToString();
			}
		}
	}
}
