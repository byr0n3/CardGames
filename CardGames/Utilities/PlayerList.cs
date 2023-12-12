using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CardGames.Data;

namespace CardGames.Utilities
{
	public sealed class PlayerList<TPlayer> : IEnumerable<TPlayer> where TPlayer : class, IPlayer<TPlayer>
	{
		public int Max { get; }
		public int Current { get; private set; }

		private readonly TPlayer[] array;

		private int nextIdx;

		public TPlayer this[int index] =>
			this.array[index];

		public PlayerList(int max)
		{
			this.Max = max;
			this.Current = 0;

			this.array = new TPlayer[this.Max];
		}

		public bool TryJoin(System.ReadOnlySpan<char> name, [NotNullWhen(true)] out TPlayer? player)
		{
			if (this.Current >= this.Max)
			{
				player = null;
				return false;
			}

			player = TPlayer.Create(this.nextIdx++, name);

			this.array[this.Current++] = player;
			return true;
		}

		public bool TryLeave(TPlayer player, out bool wasHost)
		{
			if (!this.GetPlayerIndex(player, out var idx))
			{
				wasHost = false;
				return false;
			}

			// Player we want to remove is at the start or at the end, just set it to null
			if ((idx == 0) || (idx == this.Current - 1))
			{
				this.array[idx] = null;
				this.Current--;

				wasHost = (idx == 0);
				return true;
			}

			// Move every element after the one we want to remove one element to the left
			for (var i = idx; i < this.Current; i++)
			{
				this.array[i] = this.array[i + 1];
			}

			this.Current--;

			wasHost = false;
			return true;
		}

		private bool GetPlayerIndex(TPlayer player, out int idx)
		{
			var i = 0;

			foreach (var item in this)
			{
				if (item.Equals(player))
				{
					idx = i;
					return true;
				}

				i++;
			}

			idx = default;
			return false;
		}

		public IEnumerator<TPlayer> GetEnumerator()
		{
			for (var i = 0; i < this.Current; i++)
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
