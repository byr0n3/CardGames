using CardGames.Core.Utilities;

namespace CardGames.Core
{
	public class BasePlayer : System.IEquatable<BasePlayer>
	{
		public int Key { get; }

		public SpanContainer<char> Name { get; }

		protected BasePlayer(int key, SpanContainer<char> name)
		{
			this.Key = key;
			this.Name = name;
		}

		public bool Equals(BasePlayer? other) =>
			other is not null && (this.Key == other.Key);

		public override bool Equals(object? @object) =>
			@object is BasePlayer player && this.Equals(player);

		public override int GetHashCode() =>
			this.Key;

		public static bool operator ==(BasePlayer? left, BasePlayer? right) =>
			left?.Equals(right) == true;

		public static bool operator !=(BasePlayer? left, BasePlayer? right) =>
			left?.Equals(right) == false;
	}
}
