using Stopwatch = System.Diagnostics.Stopwatch;
using Console = System.Console;

namespace Aquila
{
    public class Test
    {
        public static void T1()
        {
            System.Random r = new System.Random(123456789);

            double sum1 = 0.0;
            double sum2 = 0.0;
            for (int i = 0; i < 10; i++)
            {
                double x = System.Math.Pow(r.NextDouble(), r.NextDouble() * 100.0);
                double y = System.Math.Pow(r.NextDouble(), r.NextDouble() * 100.0);

                sum1 += x / y;
                sum2 += x * (1.0 / y);
            }

            Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} Sum1-Sum2:{2}", sum1, sum2, sum1 - sum2));
        }

        public static void T2()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            double a = 123.456789;
            double b = 1.0 / a;
            double sum1 = 0.0;
            double sum2 = 0.0;

            sw1.Start();

            for (double c = 0.0; c < 10000.0; c += 0.0001)
            {
                sum1 += c / a;
            }

            sw1.Stop();

            sw2.Start();

            for (double c = 0.0; c < 10000.0; c += 0.0001)
            {
                sum2 += c * b;
            }

            sw2.Stop();

            Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} t1:{2}[ms] t2:{3}[ms]", sum1, sum2, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds)); 
        }

        public static void T3()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            double sum1 = 0;
            double sum2 = 0;

            sw1.Start();

            for (double i = -10.0; i < 10.0; i += 0.000001)
            {
                sum1 += System.Math.Max(System.Math.Min(i, 1.0), 0.0);
            }

            sw1.Stop();

            sw2.Start();

            for (double i = -10.0; i < 10.0; i += 0.000001)
            {
                sum2 += Math.Saturate(i);
            }

            sw2.Stop();

            Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} t1:{2}[ms] t2:{3}[ms]", sum1, sum2, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds));
        }

        public static void T4()
        {
        }

        public static void Main()
        {
            T1();
            T2();
            T3();
            T4();


#if DEBUG
            Console.WriteLine("...");
            Console.ReadKey();
#endif
        }
    }
}
