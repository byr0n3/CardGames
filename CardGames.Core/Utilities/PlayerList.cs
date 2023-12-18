using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	public sealed class PlayerList<TPlayer> : IEnumerable<TPlayer> where TPlayer : BasePlayer, IPlayer<TPlayer>
	{
		public int Capacity { get; }
		public int Length { get; private set; }

		private readonly TPlayer?[] array;

		private int nextIdx;

		public TPlayer? this[int index] =>
			this.array[index];

		public PlayerList(int capacity)
		{
			this.Capacity = capacity;
			this.Length = 0;

			this.array = new TPlayer[this.Capacity];
		}

		public bool TryJoin(System.ReadOnlySpan<char> name, [NotNullWhen(true)] out TPlayer? player)
		{
			if (this.Length >= this.Capacity)
			{
				player = null;
				return false;
			}

			player = TPlayer.Create(this.nextIdx++, name);

			this.array[this.Length++] = player;
			return true;
		}

		public bool TryLeave(TPlayer player, out bool wasHost)
		{
			var idx = this.GetIndex(player);

			if (idx == -1)
			{
				wasHost = false;
				return false;
			}

			// Player we want to remove is at the start or at the end, just set it to null
			if ((idx == 0) || (idx == this.Length - 1))
			{
				this.array[idx] = null;
				this.Length--;

				wasHost = (idx == 0);
				return true;
			}

			// Move every element after the one we want to remove one element to the left
			for (var i = idx; i < this.Length; i++)
			{
				this.array[i] = this.array[i + 1];
			}

			this.Length--;

			wasHost = false;
			return true;
		}

		public int GetIndex(TPlayer player)
		{
			var i = 0;

			foreach (var item in this)
			{
				if (item == player)
				{
					return i;
				}

				i++;
			}

			return -1;
		}

		public IEnumerator<TPlayer> GetEnumerator()
		{
			for (var i = 0; i < this.Length; i++)
			{
				var item = this.array[i];

				// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
				if (item is not null)
				{
					yield return item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			this.GetEnumerator();
	}
}
