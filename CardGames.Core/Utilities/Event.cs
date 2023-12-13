using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	public sealed class Event
	{
		public delegate void EventHandler();

		private EventHandler? callback;

		public void Invoke() =>
			this.callback?.Invoke();

		public void Subscribe(EventHandler handler) =>
			this.callback += handler;

		public void Unsubscribe(EventHandler handler) =>
			this.callback -= handler;
	}

	[PublicAPI]
	public sealed class Event<T>
	{
		public delegate void EventHandler(T arg);

		private EventHandler? callback;

		public void Invoke(T arg) =>
			this.callback?.Invoke(arg);

		public void Subscribe(EventHandler handler) =>
			this.callback += handler;

		public void Unsubscribe(EventHandler handler) =>
			this.callback -= handler;
	}
}
