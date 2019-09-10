namespace Boxing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Linq;

    public class BoxTests
    {
        [Test]
        public void Return()
        {
            Assert.That(Box.Return(42).Value, Is.EqualTo(42));
        }

        [Test]
        public void Value()
        {
            Assert.That(Box.Value(new Box<int>(42)), Is.EqualTo(42));
        }

        [Test]
        public void RunMapWithNullMapper()
        {
            var e = Assert.Throws<ArgumentNullException>(() =>
                Box.RunMap<int, object>(42, null));
            Assert.That(e.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void RunMap()
        {
            Assert.That(Box.RunMap(40, x => Box.Return(x.Value + 2)), Is.EqualTo(42));
        }

        [Test]
        public void ImplicitConversionFrom()
        {
            Box<int> x = 42;
            Assert.That(x.Value, Is.EqualTo(42));
        }

        [Test]
        public void ImplicitConversionTo()
        {
            int x = Box.Return(42);
            Assert.That(x, Is.EqualTo(42));
        }

        [Test]
        public void HashCode()
        {
            const int x = 42;
            Assert.That(Box.Return(x).GetHashCode(), Is.EqualTo(x.GetHashCode()));
        }

        [Test]
        public void Equality()
        {
            Assert.That(Box.Return(42).Equals(Box.Return(42)), Is.True);
            Assert.That(Box.Return(42).Equals((object) Box.Return(42)), Is.True);
            Assert.That(Box.Return(42) == Box.Return(42), Is.True);
        }

        [Test]
        public void Inequality()
        {
            Assert.That(Box.Return(4).Equals(Box.Return(2)), Is.False);
            Assert.That(Box.Return(4).Equals((object) Box.Return(2)), Is.False);
            Assert.That(Box.Return(4) != Box.Return(2), Is.True);
        }

        [Test]
        public void NotEqualToNull()
        {
            Assert.That(Box.Return(42).Equals(null), Is.False);
        }

        [Test]
        public void NotEqualAnotherType()
        {
            Assert.That(Box.Return(42).Equals((object) 42), Is.False);
            Assert.That(Box.Return(42).Equals(Box.Return("foobar")), Is.False);
            Assert.That(Box.Return(42).Equals(new object()), Is.False);
        }

        [Test]
        public void StringRepresentation()
        {
            Assert.That(Box.Return(42).ToString(), Is.EqualTo("42"));
        }

        [Test]
        public void Bind()
        {
            var result = Box.Return(40).Bind(x => Box.Return(x + 2));
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void BindWithNullFunction()
        {
            AssertThrowsNullArgumentException("function", () =>
                Box.Return(0).Bind<int, object>(null));
        }

        [Test]
        public void Map()
        {
            var result = Box.Return(40).Map(x => x + 2);
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void MapWithNullMapper()
        {
            AssertThrowsNullArgumentException("mapper", () =>
                Box.Return(0).Map<int, object>(null));
        }

        [Test]
        public void FlatMap()
        {
            var result = Box.Return(20)
                            .FlatMap(x => Box.Return(x + 2), (x, y) => x + y);
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void FlatMapWithNullMapper()
        {
            AssertThrowsNullArgumentException("mapper", () =>
                Box.Return(0)
                   .FlatMap<int, object, object>(null, (x, y) => throw new NotImplementedException()));
        }

        [Test]
        public void FlatMapWithNullResultor()
        {
            AssertThrowsNullArgumentException("resultor", () =>
                Box.Return(0)
                   .FlatMap<int, object, object>(_ => throw new NotImplementedException(), null));
        }

        [Test]
        public void DeferWithNull()
        {
            AssertThrowsNullArgumentException("function", () => Box.Defer<object>(null));
        }

        [Test]
        public void Defer1WithNull()
        {
            AssertThrowsNullArgumentException("function", () => Box.Defer<object, object>(null));
        }

        [Test]
        public void Defer2WithNull()
        {
            AssertThrowsNullArgumentException("function", () => Box.Defer<object, object, object>(null));
        }

        [Test]
        public void Apply()
        {
            var result = Box.Defer(() => 42).Apply();
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void Apply1()
        {
            var result = Box.Defer((int x) => x + 2).Apply(40);
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void Apply2()
        {
            var result = Box.Defer((int x, int y) => x + y).Apply(40, 2);
            Assert.That(result, Is.EqualTo(Box.Return(42)));
        }

        [Test]
        public void ToEnumerable()
        {
            var result = Box.Return(42).ToEnumerable();
            Assert.That(result, Is.EqualTo(new[] { 42 }));
        }

        [Test]
        public void ToObservable()
        {
            var result = new List<int>();
            var error = (Exception)null;
            var completed = false;

            Box.Return(42)
               .ToObservable()
               .Subscribe(x => result.Add(x), e => error = e, () => completed = true)
               .Dispose();

            Assert.That(result, Is.EqualTo(new[] { 42 }));
            Assert.That(completed, Is.True);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void Select()
        {
            var result =
                from x in Box.Return(40)
                select x + 2;

            Assert.That(result.Value, Is.EqualTo(42));
        }

        [Test]
        public void SelectWithNullSelector()
        {
            AssertThrowsNullArgumentException("selector", () =>
                Box.Return(0).Select<int, object>(null));
        }

        [Test]
        public void SelectMany()
        {
            var result =
                from x in Box.Return(40)
                from y in Box.Return(2)
                select x + y;

            Assert.That(result.Value, Is.EqualTo(42));
        }

        [Test]
        public void SelectManyBoxWithNullSecondSelector()
        {
            AssertThrowsNullArgumentException("secondSelector", () =>
                Box.Return(0)
                   .SelectMany((Func<int, Box<object>>)null, BreakingFunc.Of<int, object, object>()));
        }

        [Test]
        public void SelectManyBoxWithNullResultSelector()
        {
            AssertThrowsNullArgumentException("resultSelector", () =>
                Box.Return(0)
                   .SelectMany(BreakingFunc.Of<int, Box<object>>(),
                               (Func<int, object, object>)null));
        }

        [Test]
        public void SelectManyWithSequence()
        {
            var result =
                from x in Box.Return(42)
                from y in Enumerable.Range(1, 3)
                select x * y;

            Assert.That(result, Is.EqualTo(new[]
            {
                42 * 1,
                42 * 2,
                42 * 3,
            }));
        }

        [Test]
        public void SelectManySequenceWithNullSecondSelector()
        {
            AssertThrowsNullArgumentException("secondSelector", () =>
                Box.Return(0)
                   .SelectMany((Func<int, IEnumerable<object>>)null,
                               BreakingFunc.Of<int, object, object>()));
        }

        [Test]
        public void SelectManySequenceWithNullResultSelector()
        {
            AssertThrowsNullArgumentException("resultSelector", () =>
                Box.Return(0)
                   .SelectMany(BreakingFunc.Of<int, IEnumerable<object>>(),
                               (Func<int, object, object>)null));
        }

        static void AssertThrowsNullArgumentException(string name, TestDelegate action)
        {
            var e = Assert.Throws<ArgumentNullException>(action);
            Assert.That(e.ParamName, Is.EqualTo(name));
        }
    }
}
