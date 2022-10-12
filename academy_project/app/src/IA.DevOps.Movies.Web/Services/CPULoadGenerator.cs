using System.Diagnostics;

namespace IA.DevOps.Movies.Web.Services
{
    internal class CPULoadGenerator
    {
        private const int WantedLoadPercentage = 99;
        private readonly Stopwatch MainStopWatch = new();
        private readonly List<Thread> Threads = new();

        public CPULoadGenerator()
        { }

        public void Run(int? duration)
        {
            if (!duration.HasValue) { return; }

            MainStopWatch.Start();

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var thread = new Thread(() =>
                {
                    Stopwatch watch = new();
                    watch.Start();

                    while (true)
                    {
                        // Make the loop go on for "percentage" milliseconds then sleep the 
                        // remaining percentage milliseconds. So 40% utilization means work 40ms and sleep 60ms
                        if (watch.ElapsedMilliseconds > WantedLoadPercentage)
                        {
                            Thread.Sleep(100 - WantedLoadPercentage);
                            watch.Reset();
                            watch.Start();

                            // Stop thread once load duration expires
                            if (MainStopWatch.ElapsedMilliseconds > duration) { break; }
                        }
                    }
                });

                thread.Start();
                Threads.Add(thread);
            }

            Threads.ForEach(x => x.Join());
            Threads.Clear();
            MainStopWatch.Stop();
        }
    }
}
