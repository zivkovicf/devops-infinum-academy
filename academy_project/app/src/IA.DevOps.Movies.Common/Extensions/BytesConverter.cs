namespace IA.DevOps.Movies.Common.Extensions
{
    public static class BytesConverter
    {
        public static double Megabytes(double bytes)
        {
            return bytes / 1024 / 1024;
        }

        public static double Gigabytes(double bytes)
        {
            return bytes / 1024 / 1024 / 1024;
        }
    }
}
