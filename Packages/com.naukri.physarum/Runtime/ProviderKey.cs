namespace Naukri.Physarum
{
    public record ProviderKey
    {
        private protected static int uniqueId = 0;

        public static ProviderKey Unique()
        {
            return new ProviderKey<int>(uniqueId++);
        }

        public static ProviderKey Create<T>(T key)
        {
            return new ProviderKey<T>(key);
        }
    }

    internal record ProviderKey<T>(T Key) : ProviderKey;

    public record ProviderKeyOf<TProvider> : ProviderKey
        where TProvider : IProvider
    {
        public static new ProviderKeyOf<TProvider> Unique()
        {
            return new ProviderKeyOf<TProvider, int>(uniqueId++);
        }

        public static new ProviderKeyOf<TProvider> Create<T>(T key)
        {
            return new ProviderKeyOf<TProvider, T>(key);
        }

        public static implicit operator ProviderKeyOf<TProvider>(int key)
        {
            return new ProviderKeyOf<TProvider, int>(key);
        }

        public static implicit operator ProviderKeyOf<TProvider>(string key)
        {
            return new ProviderKeyOf<TProvider, string>(key);
        }
    }

    internal record ProviderKeyOf<TProvider, T>(T Key) : ProviderKeyOf<TProvider>
        where TProvider : IProvider;
}
