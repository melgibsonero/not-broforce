using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    /// <summary>
    /// A tool for time-based events.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// How much time is left (milliseconds)
        /// </summary> 
        private float timeRemaining;

        /// <summary>
        /// The amount of time which passes before
        /// an event occurs (Seconds)
        /// </summary>
        private float duration;

        /// <summary>
        /// Is the timer currently in use
        /// </summary>
        private bool active;

        /// <summary>
        /// Is the timer unusable
        /// </summary>
        private bool locked;

        /// <summary>
        /// Creates a new Timer which keeps record of when it was activated
        /// and how much time should pass before something happens.
        /// </summary>
        /// <param name="duration">the amount of time which has to pass
        /// before an event occurs (milliseconds)</param>
        public Timer(float duration)
        {
            this.duration = duration;
            active = false;
            locked = false;
        }

        /// <summary>
        /// Starts the timer if it's available.
        /// </summary>
        public void Start()
        {
            if (!locked)
            {
                timeRemaining = duration;
                active = true;
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            active = false;
        }

        /// <summary>
        /// Updates the timer.
        /// </summary>
        public void Update()
        {
            timeRemaining -= Time.deltaTime;
        }

        /// <summary>
        /// Checks if the timer is finished.
        /// </summary>
        /// <param name="oneshot">is the timer checked only once, when the time runs out</param>
        /// <returns>returns true if the timer is finished</returns>
        public bool Check(bool oneshot)
        {
            if (active)
            {
                Update();

                if (timeRemaining > 0f)
                {
                    return false;
                }
                else
                {
                    Stop();
                    return true;
                }
            }
            else if (!oneshot)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether the timer is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return active;
            }
        }

        /// <summary>
        /// Sets the timer's lock.
        /// </summary>
        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                if (locked)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Gets the timer's duration.
        /// </summary>
        public float Duration
        {
            get { return duration; }
        }

        /// <summary>
        /// Returns the remaining time on the timer
        /// or, if the timer is not even activated, -1.
        /// </summary>
        public float TimeLeft()
        {
            // Returns -1 if the timer is not active
            if (!active)
            {
                return -1f;
            }
            // Otherwise returns the remaining time
            else
            {
                return timeRemaining;
            }
        }
    }
}
