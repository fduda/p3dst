namespace Aquila
{
    public class Math
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * 180.0 / Math.PI;
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            T t;
            t = left;
            left = right;
            right = t;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public static double Saturate(double value)
        {
            if (value <= 0.0)
            {
                return 0.0;
            }
            else if (value >= 1.0)
            {
                return 1.0;
            }
            else
            {
                return value;
            }
        }

        public static double Sqrt(double value)
        {
            return System.Math.Sqrt(value);
        }

        public static double Sin(double value)
        {
            return System.Math.Sin(value);
        }

        public static double Cos(double value)
        {
            return System.Math.Cos(value);
        }

        public static double Tan(double value)
        {
            return System.Math.Tan(value);
        }

        public static double Cot(double value)
        {
            return 1.0 / System.Math.Tan(value);
        }

        public static bool IsAlmostEqual(double a, double b, double epsilon)
        {
            double delta = a - b;
            return (delta < epsilon) && (delta > -epsilon);
        }

        public static bool IsAlmostEqual(double a, double b)
        {
            double delta = a - b;
            return (delta < EPSILON) && (delta > -EPSILON);
        }

        public static double PI = System.Math.PI;

        public static double EPSILON = 1.0e-6;
    }
}
