using System;
using System.Diagnostics;

namespace TerrainMapLibrary.Utils
{
    public sealed class StepCounter
    {
        private Stopwatch refreshTimer;

        private long stepPerRefresh;


        public long Step { get; private set; }

        public long StepLength { get; private set; }

        public long TicksLeft { get; private set; }

        public long RefreshInterval { get; private set; }

        public long ScopeInterval { get; private set; }

        public Action<StepCounter> RefreshAction { get; set; }


        public StepCounter(Action<StepCounter> refreshAction = null,
            long refreshInterval = 500, long scopeInterval = 5000)
        {
            if (refreshInterval <= 0)
            { throw new Exception("refreshInterval must be more than 0."); }

            if (scopeInterval <= 0)
            { throw new Exception("scopeInterval must be more than 0."); }

            refreshTimer = new Stopwatch();
            stepPerRefresh = 0;
            Step = 0;
            StepLength = 0;
            TicksLeft = 0;
            RefreshAction = refreshAction;
            RefreshInterval = refreshInterval;
            ScopeInterval = scopeInterval;
        }


        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return refreshTimer.GetHashCode() + stepPerRefresh.GetHashCode() 
                + Step.GetHashCode() + StepLength.GetHashCode() + TicksLeft.GetHashCode() 
                + RefreshInterval.GetHashCode() + ScopeInterval.GetHashCode();
        }

        public override string ToString()
        {
            return $"Step: {Step}, StepLength: {StepLength}";
        }


        public void Reset(long stepLength = 0, long step = 0)
        {
            refreshTimer.Stop();
            stepPerRefresh = 0;
            Step = step;
            StepLength = stepLength;
            TicksLeft = 0;

            if (RefreshAction != null) { RefreshAction.Invoke(this); }
        }

        public bool AddStep()
        {
            Step += 1;

            if (Step >= StepLength)
            {
                // stop refresh timer, time left shold be 0, and step is equal with step length
                Reset(StepLength, StepLength);
                return false;
            }

            // start refresh timer if needed
            if (refreshTimer.IsRunning == false) { refreshTimer.Restart(); }

            stepPerRefresh += 1;

            // update the ticks left per refresh interval if needed
            long refreshTicks = RefreshInterval * TimeSpan.TicksPerMillisecond;
            long scopeTicks = ScopeInterval * TimeSpan.TicksPerMillisecond;
            if (refreshTimer.ElapsedTicks >= refreshTicks)
            {
                refreshTimer.Stop();

                long newTicksLeft = (StepLength - Step) / stepPerRefresh * refreshTicks;

                // update ticks left if needed
                if (TicksLeft == 0 || Math.Abs(TicksLeft - newTicksLeft) >= scopeTicks)
                { TicksLeft = newTicksLeft; }
                else { TicksLeft -= refreshTicks; }

                if (TicksLeft < refreshTicks) { TicksLeft = refreshTicks; }

                stepPerRefresh = 0;
                refreshTimer.Restart();
            }

            if (RefreshAction != null) { RefreshAction.Invoke(this); }

            return true;
        }

        public string TimeLeft()
        {
            var timeSpan = new TimeSpan(TicksLeft);
            string content = $"{timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')}";
            if (timeSpan.Days > 0) { content = $"{timeSpan.Days} days {content}"; }

            return content;
        }
    }
}
