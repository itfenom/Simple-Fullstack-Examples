using System;
using System.Collections.Generic;

namespace Playground.Core.Utilities
{
    /// <summary>
    /// The Profiler class.  Intended for measuring the running time of different activities.
    /// </summary>
    /// <remarks>
    /// Use this class to clock the running time of different activities.
    /// You can record the number of times each activity is run and the total time spent on each activity.
    /// I used this primarily for determining the best way to execute large numbers of database write operations.
    /// 
    /// Instructions:
    /// 1. Use the constructor to establish which activities you want to time.
    /// 2. Use the start() method when you want to start timing.
    /// 3. Use one of the record() methods to record how much time was spent on an activity since the last
    ///    call to start() or record()
    /// 4. Call report() to read the results of the experiment.
    /// </remarks>
    public class Profiler
    {
        private class TimeInfo
        {
            public TimeSpan TimeSpan;
            public int TimesRun;
        }

        private DateTime _lastTime;
        private bool _hasStarted;
        private readonly SortedDictionary<string, TimeInfo> _activities;
        private const string Misc = "Miscellaneous";

        /// <summary>
        /// Example: new Profiler("connecting", "writing to DB", "disconnecting");
        /// </summary>
        /// <param name="activities">the names of the activities to be timed</param>
        public Profiler(params string[] activities)
        {
            _activities = new SortedDictionary<string, TimeInfo>();
            foreach (string activity in activities)
            {
                if (activity.Equals(Misc))
                {
                    throw new ArgumentException("Cannot use \"" + Misc + "\" as an activity name");
                }
                _activities.Add(activity, new TimeInfo());
            }
            _activities.Add(Misc, new TimeInfo());
        }
        /// <summary>
        /// start the timer for the profiler
        /// </summary>
        public void Start()
        {
            foreach (string key in _activities.Keys)
            {
                TimeInfo timeInfo = _activities[key];
                timeInfo.TimeSpan = new TimeSpan();
                timeInfo.TimesRun = 0;
            }

            _lastTime = DateTime.Now;

            _hasStarted = true;
        }
        /// <summary>
        /// record time under the miscellaneous activity
        /// </summary>
        public void Record()
        {
            Record(Misc);
        }
        /// <summary>
        /// record time for a specific activity
        /// </summary>
        /// <param name="activity">the activity to record time for</param>
        public void Record(string activity)
        {
            if (!_hasStarted)
            {
                throw new InvalidOperationException("You must call Profiler.start() before calling Profiler.record()");
            }

            DateTime now = DateTime.Now;
            TimeInfo timeInfo;

            if (_activities.ContainsKey(activity))
            {
                timeInfo = _activities[activity];
            }
            else
            {
                timeInfo = new TimeInfo {TimeSpan = new TimeSpan(), TimesRun = 0};
                _activities.Add(activity, timeInfo);
            }

            timeInfo.TimeSpan += (now - _lastTime);
            timeInfo.TimesRun++;
            _lastTime = DateTime.Now;
        }
        /// <summary>
        /// get a string reporting the time spent on each activity
        /// </summary>
        /// <returns>the report, as a string</returns>
        public string Report()
        {
            double totalTime = 0;
            foreach (string key in _activities.Keys)
            {
                totalTime += _activities[key].TimeSpan.TotalSeconds;
            }

            string result = "Total Time: " + totalTime;
            foreach (string key in _activities.Keys)
            {
                TimeInfo timeInfo = _activities[key];
                result += "\n\n" + key
                    + "\nTotal time spent on this: " + timeInfo.TimeSpan.TotalSeconds
                    + "\nNumber of times recorded: " + timeInfo.TimesRun
                    + "\nAverage duration: " + (timeInfo.TimeSpan.TotalSeconds / timeInfo.TimesRun)
                    + "\nPercentage of total time: " + ((100 * timeInfo.TimeSpan.TotalSeconds) / totalTime);
            }

            return result;
        }
    }
}
