namespace Block2D.Common
{
    public class DefaultLogger : Block2DLogger
    {
        public static readonly DefaultLogger Instance;

        private DefaultLogger()
            : base("Default") { }

        static DefaultLogger()
        {
            Instance ??= new();
        }
    }
}
