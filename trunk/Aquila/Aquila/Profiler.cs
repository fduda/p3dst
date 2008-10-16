using Stopwatch = System.Diagnostics.Stopwatch;
using Console = System.Console;

// TODO is it possible to only map List<T>?
using System.Collections.Generic;

namespace Aquila
{
    // TODO only an idea for a hierarchical profiler (more or less like pstats in P3D)
    class Profiler
    {
        private string name;
        List<Profiler> profilers = new List<Profiler>();
        Stopwatch sw = new Stopwatch();

        public Profiler(string name)
        {
            this.name = name;
        }

        public void Reset()
        {
            this.sw.Reset();
            foreach (Profiler profiler in this.profilers)
            {
                profiler.Reset();
            }
        }

        public void Start()
        {
            this.sw.Start();
        }

        public void Stop()
        {
            this.sw.Stop();
        }

        public long MilliSeconds()
        {
            return sw.ElapsedMilliseconds;
        }

        public void Print()
        {
            Console.WriteLine(this.name + " " + MilliSeconds());
            foreach (Profiler profiler in this.profilers)
            {
                profiler.Print();
            }
        }

        public Profiler Add(string name)
        {
            Profiler profiler = new Profiler(name);
            this.profilers.Add(profiler);
            return profiler;
        }
    }
}
