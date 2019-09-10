namespace Boxing.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Reactive;

    public class ObservableTests
    {
        [Test]
        public void CreateWithNull()
        {
            AssertThrows.ArgumentNullException("subscriptionHandler", () =>
                Observable.Create<int>(null));
        }

        [Test]
        public void NullObserverDuringSubscription()
        {
            var q = Observable.Create(BreakingFunc.Of<IObserver<int>, IDisposable>());

            AssertThrows.ArgumentNullException("observer", () =>
                q.Subscribe(null));
        }

        [Test]
        public void SubscribeWithNullSource()
        {
            AssertThrows.ArgumentNullException("source", () =>
                Observable.Subscribe(null, BreakingAction.Of<int>(),
                                           BreakingAction.Of<Exception>(),
                                           BreakingAction.OfNone));
        }

        [Test]
        public void SubscribeWithNullOnNext()
        {
            var q = Observable.Create(BreakingFunc.Of<IObserver<int>, IDisposable>());

            AssertThrows.ArgumentNullException("onNext", () =>
                q.Subscribe(null, BreakingAction.Of<Exception>(), BreakingAction.OfNone));
        }

        [Test]
        public void SubscribeWithNullOnError()
        {
            Observable
                .Create<int>(observer => throw new ApplicationException())
                .AssertThat(error: null,
                            completed: Is.False,
                            observations: Is.Empty);
        }

        [Test]
        public void SubscribeWithNullOnCompleted()
        {
            Observable
                .Create<int>(observer =>
                {
                    observer.OnCompleted();
                    return Disposable.Nop;
                })
                .AssertThat(error: Is.Null,
                            completed: null,
                            observations: Is.Empty);
        }

        [Test]
        public void SubscriptionError()
        {
            var errorToThrow = new ApplicationException();

            Observable
                .Create<int>(observer => throw errorToThrow)
                .AssertThat(error: Is.SameAs(errorToThrow),
                            completed: Is.False,
                            observations: Is.Empty);
        }

        [Test]
        public void SubscriptionExtensionError()
        {
            var errorToThrow = new ApplicationException();

            var source = new ErroneousObservable<int>(errorToThrow);
            source.AssertThat(error: Is.SameAs(errorToThrow),
                              completed: Is.False,
                              observations: Is.Empty);
        }

        sealed class ErroneousObservable<T> : IObservable<T>
        {
            readonly Exception _subscriptionError;

            public ErroneousObservable(Exception subscriptionError) =>
                _subscriptionError = subscriptionError;

            public IDisposable Subscribe(IObserver<T> observer) =>
                throw _subscriptionError;
        }
    }
}
