using Stopwatch = System.Diagnostics.Stopwatch;
using Console = System.Console;
using Random = System.Random;

namespace Aquila
{
    public class Test
    {
        public static void T1()
        {
            for (int i = 1; i < 10000; i++)
            {
                Random r = new Random(i);

                float sum1 = 0.0f;
                float sum2 = 0.0f;
                for (int j = 0; j < 100; j++)
                {
                    float x = Math.Pow((float)r.NextDouble(), (float)r.NextDouble() * 5.0f);
                    float y = Math.Pow((float)r.NextDouble(), (float)r.NextDouble() * 5.0f);

                    sum1 += x / y;
                    sum2 += x * (1.0f / y);
                }

                if ((sum1 - sum2) != 0.0)
                {
                    Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} Sum1-Sum2:{2}", sum1, sum2, sum1 - sum2));
                }
            }
        }

        public static void T2()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            float a = 123.456789f;
            float b = 1.0f / a;
            float sum1 = 0.0f;
            float sum2 = 0.0f;

            sw1.Start();

            for (float c = 0.0f; c < 1000.0f; c += 0.0001f)
            {
                sum1 += c / a;
            }

            sw1.Stop();

            sw2.Start();

            for (float c = 0.0f; c < 1000.0f; c += 0.0001f)
            {
                sum2 += c * b;
            }

            sw2.Stop();

            Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} t1:{2}[ms] t2:{3}[ms]",
                sum1, sum2, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds));
        }

        public static void T3()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            float sum1 = 0;
            float sum2 = 0;

            sw1.Start();

            for (float i = -10.0f; i < 10.0f; i += 0.000001f)
            {
                sum1 += System.Math.Max(System.Math.Min(i, 1.0f), 0.0f);
            }

            sw1.Stop();

            sw2.Start();

            for (float i = -10.0f; i < 10.0f; i += 0.000001f)
            {
                sum2 += Math.Saturate(i);
            }

            sw2.Stop();

            Console.WriteLine(string.Format("Sum1:{0} Sum2:{1} t1:{2}[ms] t2:{3}[ms]",
                sum1, sum2, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds));
        }

        private struct SimpleVector4
        {
            public double a;
            public double b;
            public double c;
            public double d;

            public SimpleVector4(double a, double b, double c, double d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }
        }

        public static void T4()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();
            Stopwatch sw4 = new Stopwatch();

            int w = 640;
            int h = 480;
            int wh = w * h;

            Vector4[,] a1 = new Vector4[h, w];
            Vector4[] a2 = new Vector4[h * w];
            Vector4[,] a3 = new Vector4[w, h];
            SimpleVector4[,] a4 = new SimpleVector4[h, w];
            Vector4 clear = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            SimpleVector4 testClear = new SimpleVector4(1.0, 2.0, 3.0, 4.0);

            sw1.Start();

            for (int i = 0; i < 10; i++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        a1[y, x] = clear;
                    }
                }
            }

            sw1.Stop();

            sw2.Start();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < wh; j++)
                {
                    a2[j] = clear;
                }
            }

            sw2.Stop();

            sw3.Start();

            for (int i = 0; i < 10; i++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        a3[x, y] = clear;
                    }
                }
            }

            sw3.Stop();

            sw4.Start();

            for (int i = 0; i < 10; i++)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        a4[y, x] = testClear;
                    }
                }
            }

            sw4.Stop();

            Console.WriteLine(string.Format("t1:{0}[ms] t2:{1}[ms] t3:{2}[ms] t4:{3}[ms]",
                sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds, sw3.ElapsedMilliseconds, sw4.ElapsedMilliseconds));
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
