using System;
using System.Timers;

namespace DystirWeb.Services
{
    public class TimeService
    {
        public EventHandler<EventArgs> OnTimerElapsed;
        public EventHandler<EventArgs> OnSponsorsTimerElapsed;

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
            Timer timer = new Timer(10000);
            timer.Elapsed += NotifySponsorsTimerElapsed;
            timer.Enabled = true;
            timer.Start();
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
