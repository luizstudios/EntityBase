﻿using System;
using System.Timers;
using Tars.Extensions;

namespace Tars.ScheduledEvents.Classes
{
    /// <summary>
    /// Class to create a new event.
    /// </summary>
    public sealed class Event
    {
        /// <summary>
        /// Name of event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description of event.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Code that will be executed when the event is called.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Time interval for the event to be called.
        /// </summary>
        public TimeSpan Interval { get; private set; }

        /// <summary>
        /// Indicates whether the event is active.
        /// </summary>
        public bool IsActived { get; private set; }

        /// <summary>
        /// Indicates if the scheduled event will execute on Debug mode.
        /// </summary>
        public bool ExecuteOnDebugMode { get; }

        private readonly Timer _timer;

        /// <summary>
        /// Build a scheduled event.
        /// </summary>
        /// <param name="name">Name of event.</param>
        /// <param name="action">Code that will be executed when the event is called.</param>
        /// <param name="interval">Time interval for the event to be called.</param>
        /// <param name="description">Description of event.</param>
        /// <param name="executeOnDebugMode">Indicates if this event will run on debug mode.</param>
        public Event(string name, Action action, TimeSpan interval, string description = null, bool executeOnDebugMode = true)
        {
            this.Name = name.IsNullOrEmptyOrWhiteSpace() ? throw new ArgumentException("The Name of scheduled event can be null!") : name;
            this.Action = action ?? throw new ArgumentNullException("The Action can be null!");
            this.Interval = interval;
            this.Description = description;
            this.ExecuteOnDebugMode = executeOnDebugMode;

            this._timer = new Timer()
            {
                Interval = this.Interval.TotalMilliseconds
            };
            this._timer.Elapsed += (s, e) =>
            {
#if DEBUG
                if (this.ExecuteOnDebugMode)
                    this.Action.Invoke();
#else
                this.Action.Invoke();
#endif
            };
            this._timer.Start();

            this.IsActived = true;
        }

        /// <summary>
        /// Activate the event.
        /// </summary>
        public void Activate()
        {
            this._timer.Start();

            if (!this.IsActived)
                this.IsActived = true;
        }

        /// <summary>
        /// Disables the event.
        /// </summary>
        public void Deactivate()
        {
            this._timer.Stop();

            if (this.IsActived)
                this.IsActived = false;
        }

        /// <summary>
        /// Changes the event settings.
        /// </summary>
        /// <param name="newName">New event name.</param>
        /// <param name="newInterval">New time interval for the event to be activated.</param>
        /// <param name="newDescription">New event description.</param>
        public void ChangeSettings(string newName = null, TimeSpan? newInterval = null, string newDescription = null)
        {
            if (!newName.IsNullOrEmptyOrWhiteSpace())
                this.Name = newName;

            if (newInterval.HasValue)
            {
                var newIntervalValue = newInterval.Value;
                this._timer.Interval = newIntervalValue.TotalMilliseconds;
                this.Interval = newIntervalValue;
            }

            if (!newDescription.IsNullOrEmptyOrWhiteSpace())
                this.Description = newDescription;
        }
    }
}