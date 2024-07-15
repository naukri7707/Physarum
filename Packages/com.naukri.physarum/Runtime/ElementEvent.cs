namespace Naukri.Physarum
{
    public interface IElementEvent { }

    public static class ElementEvents
    {
        public record Initialize : IElementEvent
        {
            private static Initialize _default;

            public static Initialize Default => _default ??= new();
        }

        public record Enable : IElementEvent
        {
            private static Enable _default;

            public static Enable Default => _default ??= new();
        }

        public record Disable : IElementEvent
        {
            private static Disable _default;

            public static Disable Default => _default ??= new();
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
