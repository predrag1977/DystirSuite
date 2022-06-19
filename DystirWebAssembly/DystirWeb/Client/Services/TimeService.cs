using System;
using System.Timers;

namespace DystirWeb.Services
{
    public class TimeService
    {
        public EventHandler<EventArgs> OnTimerElapsed;
        public EventHandler<EventArgs> OnSponsorsTimerElapsed;
        private Timer _sponsorsTimer;

        public TimeService()
        {
            SetTimer();
            SetSponsorsTimer();
        }

        public void SetTimer()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += NotifyTimerElapsed;
            timer.Enabled = true;
            timer.Start();
        }

        public void SetSponsorsTimer()
        {
            _sponsorsTimer = new Timer(10000);
            _sponsorsTimer.Elapsed += NotifySponsorsTimerElapsed;
            _sponsorsTimer.Enabled = true;
        }

        public void StartSponsorsTime()
        {
            OnTimerElapsed?.Invoke(this, new EventArgs());
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
