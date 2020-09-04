namespace Dissonance.Extensions
{
    internal static class Int32Extensions
    {
        internal static int WrappedDelta(this int a, int b)
        {
            return a.WrappedDelta(b, int.MaxValue);
        }

        internal static int WrappedDelta(this int a, int b, int max)
        {
            var delta = b - a;

            if (delta < 0)
                delta += max;

            if (delta > max / 2)
                delta -= max;

            return delta;
        }
    }
}
