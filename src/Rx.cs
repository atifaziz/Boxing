#region Copyright 2017 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace Boxing.Reactive
{
    using System;

    static class Observable
    {
        public static IObservable<T> Create<T>(Func<IObserver<T>, IDisposable> subscriptionHandler) =>
            new Observable<T>(subscriptionHandler);

        public static IDisposable Subscribe<T>(this IObservable<T> source,
            Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            // NOTE! Observer created is a struct so it won't cause a heap allocation
            // and so there is no chance of an OutOfMemoryException being thrown.
            // Moreover, the observer is created outside of the following try/catch
            // so that argument errors are propagated up the stack instead of to the
            // observer.

            var observer = Observer.Create(onNext, onError, onCompleted);

            try
            {
                return source.Subscribe(observer);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
                return Disposable.Nop;
            }
        }
    }

    static class Disposable
    {
        public static readonly IDisposable Nop = new NopDisposable();
        sealed class NopDisposable : IDisposable { public void Dispose() {} }
    }

    static class Observer
    {
        public static IObserver<T> Create<T>(Action<T> onNext,
                                             Action<Exception> onError = null,
                                             Action onCompleted = null) =>
            new Observer<T>(onNext, onError, onCompleted);
    }

    sealed class Observable<T> : IObservable<T>
    {
        readonly Func<IObserver<T>, IDisposable> _subscriptionHandler;

        public Observable(Func<IObserver<T>, IDisposable> subscriptionHandler) =>
            _subscriptionHandler = subscriptionHandler
                                   ?? throw new ArgumentNullException(nameof(subscriptionHandler));

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            try
            {
                return _subscriptionHandler(observer);
            }
            catch (Exception e)
            {
                observer.OnError(e);
                return Disposable.Nop;
            }
        }
    }

    readonly struct Observer<T> : IObserver<T>
    {
        readonly Action<T> _onNext;
        readonly Action<Exception> _onError;
        readonly Action _onCompleted;

        public Observer(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnCompleted() => _onCompleted?.Invoke();
        public void OnError(Exception error) => _onError?.Invoke(error);
        public void OnNext(T value) => _onNext(value);
    }
}
