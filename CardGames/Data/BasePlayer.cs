namespace CardGames.Data
{
	public class BasePlayer : IPlayer<BasePlayer>, System.IEquatable<BasePlayer>
	{
		public int Key { get; }

		public BasePlayer(int key)
		{
			this.Key = key;
		}

		public static BasePlayer Create(int key) =>
			new(key);

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
