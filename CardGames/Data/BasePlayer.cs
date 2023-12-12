using CardGames.Utilities;

namespace CardGames.Data
{
	public class BasePlayer : IPlayer<BasePlayer>, System.IEquatable<BasePlayer>
	{
		public int Key { get; }

		public SpanContainer<char> Name { get; }

		public BasePlayer(int key, SpanContainer<char> name)
		{
			this.Key = key;
			this.Name = name;
		}

		public static BasePlayer Create(int key, System.ReadOnlySpan<char> name) =>
			new(key, name);

		public bool Equals(IPlayer<BasePlayer>? other) =>
			other is not null && this.Key == other.Key;

		public bool Equals(BasePlayer? other) =>
			this.Equals((IPlayer<BasePlayer>?)other);

		public override bool Equals(object? @object) =>
			@object is IPlayer<BasePlayer> player && this.Equals(player);

		public override int GetHashCode() =>
			this.Key;
	}
}
