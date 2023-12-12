using System.Collections;
using System.Collections.Generic;
using CardGames.Data;

namespace CardGames.Utilities
{
	internal sealed class PlayerList<TPlayer> : IEnumerable<TPlayer> where TPlayer : class, IPlayer<TPlayer>
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

		public bool TryJoin(out TPlayer? player)
		{
			if (this.Current >= this.Max)
			{
				player = null;
				return false;
			}

			player = TPlayer.Create(this.nextIdx++);

			this.array[this.Current++] = player;
			return true;
		}

		public bool TryLeave(TPlayer player)
		{
			if (!this.GetPlayerIndex(player, out var idx))
			{
				return false;
			}

			// @todo Player is host, delete game
			if (idx == 0)
			{
				return true;
			}

			// Player we want to remove is at the end, just set it to null
			if (idx == this.Current - 1)
			{
				this.array[idx] = null;
				this.Current--;

				return true;
			}

			// Move every element after the one we want to remove one element to the left
			for (var i = idx; i < this.Current; i++)
			{
				this.array[i] = this.array[i + 1];
			}

			this.Current--;

			return true;
		}

		private bool GetPlayerIndex(TPlayer player, out int idx)
		{
			for (var i = 0; i < this.Current; i++)
			{
				if (this.array[i].Equals(player))
				{
					idx = i;
					return true;
				}
			}

			idx = default;
			return false;
		}

		public IEnumerator<TPlayer> GetEnumerator()
		{
			for (var i = 0; i < this.Current; i++)
			{
				yield return this.array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			this.GetEnumerator();
	}
}
