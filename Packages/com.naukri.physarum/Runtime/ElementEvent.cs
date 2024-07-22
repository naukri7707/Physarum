namespace Naukri.Physarum
{
    public interface IElementEvent { }

    public static class ElementEvents
    {
        public record Invalidate : IElementEvent
        {
            private static Invalidate _default;

            public static Invalidate Default => _default ??= new();
        }

        public record Refresh : IElementEvent
        {
            private static Refresh _default;

            public static Refresh Default => _default ??= new();
        }

        public record StateChanged : IElementEvent
        {
            private static StateChanged _default;

            public static StateChanged Default => _default ??= new();
        }
    }
}
