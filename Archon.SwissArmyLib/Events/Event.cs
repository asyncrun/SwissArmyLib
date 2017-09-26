﻿using System;
using System.Collections.ObjectModel;
using Archon.SwissArmyLib.Collections;
using UnityEngine;

namespace Archon.SwissArmyLib.Events
{
    /// <summary>
    /// A simple event handler that uses interfaces instead of delegates to avoid the garbage generated by them.
    /// 
    /// This is the parameterless version. 
    /// See <see cref="Event{T}"/> if you need to send data with the event.
    /// 
    /// Listeners are required to implement the <see cref="IEventListener"/> interface.
    /// 
    /// Events are differentiated by an integer. You are expected to create constants to define your events and make them unique.
    /// </summary>
    public class Event
    {
        private readonly DelayedList<PrioritizedItem<IEventListener>> _listeners;
        private bool _isIterating;

        /// <summary>
        /// Gets the ID of this event.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets whether listener exceptions should be logged. 
        /// </summary>
        public bool SuppressExceptions { get; set; }

        /// <summary>
        /// Gets a readonly collection of current listeners.
        /// </summary>
        public ReadOnlyCollection<PrioritizedItem<IEventListener>> Listeners
        {
            get
            {
                if (!_isIterating)
                    _listeners.ProcessPending();
                return _listeners.BackingList;
            }
        }

        /// <summary>
        /// Creates a new Event with the specified ID.
        /// </summary>
        /// <param name="id">The id of the event.</param>
        public Event(int id)
        {
            Id = id;
            _listeners = new DelayedList<PrioritizedItem<IEventListener>>(new PrioritizedList<IEventListener>());
        }

        /// <summary>
        /// Adds a listener for the event with an optional call-order priority.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <param name="priority">The priority of the listener compared to other listeners. Controls whether the listener is called before or after other listeners.</param>
        public void AddListener(IEventListener listener, int priority = 0)
        {
            _listeners.Add(new PrioritizedItem<IEventListener>(listener, priority));
        }

        /// <summary>
        /// Removes a listener from the event.
        /// </summary>
        /// <param name="listener">The listener to remove</param>
        public void RemoveListener(IEventListener listener)
        {
            _listeners.Remove(new PrioritizedItem<IEventListener>(listener, 0));
        }

        /// <summary>
        /// Checks whether the specified listener is currently listening to this event.
        /// </summary>
        /// <param name="listener">The listener to check.</param>
        /// <returns>True if listening, otherwise false.</returns>
        public bool HasListener(IEventListener listener)
        {
            if (!_isIterating)
                _listeners.ProcessPending();

            for (var i = 0; i < _listeners.Count; i++)
            {
                var current = _listeners[i];
                if (current.Item == listener)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Notifies all listeners that the event occured.
        /// </summary>
        public void Invoke()
        {
            _listeners.ProcessPending();

            _isIterating = true;
            for (var i = 0; i < _listeners.Count; i++)
            {
                // gotta wrap it up so one guy doesn't spoil it for everyone
                try
                {
                    _listeners[i].Item.OnEvent(Id);
                }
                catch (Exception e)
                {
                    if (!SuppressExceptions)
                        Debug.LogError(e);
                }
            }
            _isIterating = false;
        }

        /// <summary>
        /// Clears all listeners
        /// </summary>
        public void Clear()
        {
            _listeners.Clear();
        }
    }

    /// <summary>
    /// A simple event handler that uses interfaces instead of delegates to avoid the garbage generated by them.
    /// 
    /// This is the parameterized version. 
    /// See <see cref="Event"/> if you don't need to send data with the event.
    /// 
    /// Listeners are required to implement the <see cref="IEventListener{T}"/> interface.
    /// 
    /// Events are differentiated by an integer. You are expected to create constants to define your events and make them unique.
    /// </summary>
    public class Event<T>
    {
        private readonly DelayedList<PrioritizedItem<IEventListener<T>>> _listeners;
        private bool _isIterating;

        /// <summary>
        /// Gets the ID of this event.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets whether listener exceptions should be logged. 
        /// </summary>
        public bool SuppressExceptions { get; set; }

        /// <summary>
        /// Gets a readonly collection of current listeners.
        /// </summary>
        public ReadOnlyCollection<PrioritizedItem<IEventListener<T>>> Listeners
        {
            get
            {
                if (!_isIterating)
                    _listeners.ProcessPending();
                return _listeners.BackingList;
            }
        }

        /// <summary>
        /// Creates a new Event with the specified ID.
        /// </summary>
        /// <param name="id">The id of the event.</param>
        public Event(int id)
        {
            Id = id;
            _listeners = new DelayedList<PrioritizedItem<IEventListener<T>>>(new PrioritizedList<IEventListener<T>>());
        }

        /// <summary>
        /// Adds a listener for the event with an optional call-order priority.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <param name="priority">The priority of the listener compared to other listeners. Controls whether the listener is called before or after other listeners.</param>
        public void AddListener(IEventListener<T> listener, int priority = 0)
        {
            _listeners.Add(new PrioritizedItem<IEventListener<T>>(listener, priority));
        }

        /// <summary>
        /// Removes a listener from the event.
        /// </summary>
        /// <param name="listener">The listener to remove</param>
        public void RemoveListener(IEventListener<T> listener)
        {
            _listeners.Remove(new PrioritizedItem<IEventListener<T>>(listener, 0));
        }

        /// <summary>
        /// Checks whether the specified listener is currently listening to this event.
        /// </summary>
        /// <param name="listener">The listener to check.</param>
        /// <returns>True if listening, otherwise false.</returns>
        public bool HasListener(IEventListener<T> listener)
        {
            if (!_isIterating)
                _listeners.ProcessPending();

            for (var i = 0; i < _listeners.Count; i++)
            {
                var current = _listeners[i];
                if (current.Item == listener)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Notifies all listeners that the event occured.
        /// </summary>
        public void Invoke(T args)
        {
            _listeners.ProcessPending();

            _isIterating = true;
            for (var i = 0; i < _listeners.Count; i++)
            {
                // gotta wrap it up so one guy doesn't spoil it for everyone
                try
                {
                    _listeners[i].Item.OnEvent(Id, args);
                }
                catch (Exception e)
                {
                    if (!SuppressExceptions)
                        Debug.LogError(e);
                }
            }
            _isIterating = false;
        }

        /// <summary>
        /// Clears all listeners
        /// </summary>
        public void Clear()
        {
            _listeners.Clear();
        }
    }
}