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
            var observations = new List<int>();
            var completed = false;

            var q =
                Observable.Create<int>(observer => throw new ApplicationException());

            var subscription =
                q.Subscribe(onCompleted: () => completed = true, onError: null,
                            onNext: observations.Add);

            subscription.Dispose();

            Assert.That(observations, Is.Empty);
            Assert.That(completed, Is.False);
        }

        [Test]
        public void SubscribeWithNullOnCompleted()
        {
            var observations = new List<int>();
            var error = (Exception)null;

            var q =
                Observable.Create<int>(observer =>
                {
                    observer.OnCompleted();
                    return Disposable.Nop;
                });

            var subscription =
                q.Subscribe(onCompleted: null, onError: e => error = e,
                            onNext: observations.Add);

            subscription.Dispose();

            Assert.That(observations, Is.Empty);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void ThrowDuringSubscription()
        {
            var observations = new List<int>();
            var completed = false;
            var error = (Exception)null;
            var errorToThrow = new ApplicationException();

            var q = Observable.Create<int>(observer => throw errorToThrow);

            var subscription =
                q.Subscribe(onCompleted: () => completed = true, onError: e => error = e,
                            onNext: observations.Add);

            subscription.Dispose();

            Assert.That(completed, Is.False);
            Assert.That(error, Is.SameAs(errorToThrow));
            Assert.That(observations, Is.Empty);
        }

        [Test]
        public void ThrowDuringSubscription2()
        {
            var observations = new List<int>();
            var completed = false;
            var error = (Exception)null;
            var errorToThrow = new ApplicationException();

            var q = new ErroneousObservable<int>(errorToThrow);

            var subscription =
                q.Subscribe(onCompleted: () => completed = true, onError: e => error = e,
                            onNext: observations.Add);

            subscription.Dispose();

            Assert.That(completed, Is.False);
            Assert.That(error, Is.SameAs(errorToThrow));
            Assert.That(observations, Is.Empty);
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
