using System;
using System.ComponentModel;
using System.Threading;

namespace BIAI
{
    public class AbortableWorker : BackgroundWorker
    {
        private Thread thread;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                thread = Thread.CurrentThread;
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true;
            }
            
        }

        public void Abort()
        {
            thread.Abort();
        }
    }
}