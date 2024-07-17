using System;

namespace Naukri.Physarum
{
    public abstract record AsyncValue<TValue>
        where TValue : IEquatable<TValue>
    {
        #region properties
        public abstract TValue Value { get; }
        public abstract bool HasValue { get; }
        public abstract bool IsLoading { get; }
        public abstract bool HasError { get; }
        #endregion

        #region methods

        public void When(Action<TValue> data, Action loading, Action<Exception> error)
        {
            Map(
                dataAsyncValue => data(dataAsyncValue.Value),
                loadingAsyncValue => loading(),
                errorAsyncValue => error(errorAsyncValue.Exception)
            );
        }

        public T When<T>(Func<TValue, T> data, Func<T> loading, Func<Exception, T> error)
        {
            return Map(
                dataAsyncValue => data(dataAsyncValue.Value),
                loadingAsyncValue => loading(),
                errorAsyncValue => error(errorAsyncValue.Exception)
            );
        }

        public T WhenOrDefault<T>(Func<TValue, T> data, T defaultValue = default)
        {
            return Map(
                dataAsyncValue => data(dataAsyncValue.Value),
                loadingAsyncValue => defaultValue,
                errorAsyncValue => defaultValue
            );
        }

        public void WhenOrDefault<T>(Action<TValue> data, Action defaultAction)
        {
            Map(
                dataAsyncValue => data(dataAsyncValue.Value),
                loadingAsyncValue => defaultAction(),
                errorAsyncValue => defaultAction()
            );
        }

        public static AsyncValue<TValue> Data(TValue value)
        {
            return new AsyncData<TValue>(value);
        }

        public static AsyncValue<TValue> Loading()
        {
            return new AsyncLoading<TValue>();
        }

        public static AsyncValue<TValue> Error(Exception exception)
        {
            return new AsyncError<TValue>(exception);
        }

        internal abstract void Map(
            Action<AsyncData<TValue>> data,
            Action<AsyncLoading<TValue>> loading,
            Action<AsyncError<TValue>> error
        );

        internal abstract T Map<T>(
            Func<AsyncData<TValue>, T> data,
            Func<AsyncLoading<TValue>, T> loading,
            Func<AsyncError<TValue>, T> error
        );

        #endregion
    }

    internal record AsyncData<TValue> : AsyncValue<TValue>
        where TValue : IEquatable<TValue>
    {
        public AsyncData(TValue value)
        {
            Value = value;
        }

        #region properties
        public sealed override TValue Value { get; }
        public sealed override bool HasValue => true;
        public sealed override bool IsLoading => false;
        public sealed override bool HasError => false;
        #endregion

        internal override void Map(
            Action<AsyncData<TValue>> data,
            Action<AsyncLoading<TValue>> loading,
            Action<AsyncError<TValue>> error
        )
        {
            data(this);
        }

        internal override TAsyncValue Map<TAsyncValue>(
            Func<AsyncData<TValue>, TAsyncValue> data,
            Func<AsyncLoading<TValue>, TAsyncValue> loading,
            Func<AsyncError<TValue>, TAsyncValue> error
        )
        {
            return data(this);
        }
    }

    internal record AsyncLoading<TValue> : AsyncValue<TValue>
        where TValue : IEquatable<TValue>
    {
        #region properties
        public sealed override TValue Value => default;
        public sealed override bool HasValue => false;
        public sealed override bool IsLoading => true;
        public sealed override bool HasError => false;
        #endregion

        internal override void Map(
            Action<AsyncData<TValue>> data,
            Action<AsyncLoading<TValue>> loading,
            Action<AsyncError<TValue>> error
        )
        {
            loading(this);
        }

        internal override TAsyncValue Map<TAsyncValue>(
            Func<AsyncData<TValue>, TAsyncValue> data,
            Func<AsyncLoading<TValue>, TAsyncValue> loading,
            Func<AsyncError<TValue>, TAsyncValue> error
        )
        {
            return loading(this);
        }
    }

    internal record AsyncError<TValue>(Exception Exception) : AsyncValue<TValue>
        where TValue : IEquatable<TValue>
    {
        #region properties

        public override TValue Value =>
            throw new InvalidOperationException("Cannot access the value while error.");

        public sealed override bool HasValue => false;
        public sealed override bool IsLoading => false;
        public sealed override bool HasError => true;
        #endregion

        internal override void Map(
            Action<AsyncData<TValue>> data,
            Action<AsyncLoading<TValue>> loading,
            Action<AsyncError<TValue>> error
        )
        {
            error(this);
        }

        internal override TAsyncValue Map<TAsyncValue>(
            Func<AsyncData<TValue>, TAsyncValue> data,
            Func<AsyncLoading<TValue>, TAsyncValue> loading,
            Func<AsyncError<TValue>, TAsyncValue> error
        )
        {
            return error(this);
        }
    }
}
