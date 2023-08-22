using System;
using System.Timers;

namespace DystirWeb.Services
{
    public class TimeService
    {
        public EventHandler<EventArgs> OnTimerElapsed;
        public EventHandler<EventArgs> OnSponsorsTimerElapsed;
        private Timer _timer;
        private Timer _sponsorsTimer;

        public TimeService()
        {
            SetTimer();
            SetSponsorsTimer();
        }

        public void SetTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        public void SetSponsorsTimer()
        {
            _sponsorsTimer = new Timer(10000);
            _sponsorsTimer.Elapsed += NotifySponsorsTimerElapsed;
            _sponsorsTimer.Enabled = true;
        }

        public void StartSponsorsTime()
        {
            OnSponsorsTimerElapsed?.Invoke(this, new EventArgs());
            _sponsorsTimer.Start();
        }

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            OnTimerElapsed?.Invoke(this, new EventArgs());
        }

        private void NotifySponsorsTimerElapsed(object source, ElapsedEventArgs e)
        {
            OnSponsorsTimerElapsed?.Invoke(this, new EventArgs());
        }
    }
}
