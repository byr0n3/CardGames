using System.Collections.Generic;
using JetBrains.Annotations;

namespace CardGames.Core.Utilities
{
	[PublicAPI]
	public sealed class Event
	{
		public delegate void EventHandler();

		private readonly List<EventHandler> listeners;

		public Event()
		{
			this.listeners = new List<EventHandler>();
		}

		public Event(params EventHandler[] listeners)
		{
			this.listeners = new List<EventHandler>(listeners.Length);
			this.listeners.AddRange(listeners);
		}

		public void Invoke()
		{
			for (var i = this.listeners.Count - 1; i >= 0; i--)
			{
				this.listeners[i].Invoke();
			}
		}

		public void Subscribe(EventHandler handler) =>
			this.listeners.Add(handler);

		public void Unsubscribe(EventHandler handler) =>
			this.listeners.Remove(handler);
	}

	[PublicAPI]
	public sealed class Event<T>
	{
		public delegate void EventHandler(T var);

		private readonly List<EventHandler> listeners;

		public Event()
		{
			this.listeners = new List<EventHandler>();
		}

		public Event(params EventHandler[] listeners)
		{
			this.listeners = new List<EventHandler>(listeners.Length);
			this.listeners.AddRange(listeners);
		}

		public void Invoke(T var)
		{
			for (var i = this.listeners.Count - 1; i >= 0; i--)
			{
				this.listeners[i].Invoke(var);
			}
		}

		public void Subscribe(EventHandler handler) =>
			this.listeners.Add(handler);

		public void Unsubscribe(EventHandler handler) =>
			this.listeners.Remove(handler);
	}
}
