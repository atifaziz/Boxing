#region Copyright (c) 2018 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace Boxing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static partial class Box
    {
        public static Box<T> Return<T>(T value) => new Box<T>(value);

        public static T Value<T>(Box<T> box) => box.Value;

        public static TResult RunMap<T, TResult>(T arg, Func<Box<T>, Box<TResult>> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            return mapper(arg).Value;
        }

        public static Box<TResult> Bind<T, TResult>(this Box<T> box, Func<T, Box<TResult>> function)
        {
            if (function == null) throw new ArgumentNullException(nameof(function));
            return function(box.Value);
        }

        public static Box<TResult> Map<T, TResult>(this Box<T> box, Func<T, TResult> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            return box.Bind(x => Return(mapper(x)));
        }

        public static Box<TResult>
            FlatMap<TFirst, TSecond, TResult>(
                this Box<TFirst> box,
                Func<TFirst, Box<TSecond>> mapper,
                Func<TFirst, TSecond, TResult> resultor)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            if (resultor == null) throw new ArgumentNullException(nameof(resultor));
            return box.Bind(a => mapper(a).Map(b => resultor(a, b)));
        }

        public static Box<Func<T>> Defer<T>(Func<T> function) =>
            Return(function ?? throw new ArgumentNullException(nameof(function)));

        public static Box<Func<T, TResult>> Defer<T, TResult>(Func<T, TResult> function) =>
            Return(function ?? throw new ArgumentNullException(nameof(function)));

        public static Box<Func<T1, T2, TResult>> Defer<T1, T2, TResult>(Func<T1, T2, TResult> function) =>
            Return(function ?? throw new ArgumentNullException(nameof(function)));

        public static Box<T> Apply<T>(this Box<Func<T>> function) => function.Bind(f => Return(f()));

        public static Box<TResult> Apply<T, TResult>(this Box<Func<T, TResult>> function, Box<T> box) =>
            function.Bind(f => Return(f(box.Value)));

        public static Box<TResult> Apply<T1, T2, TResult>(this Box<Func<T1, T2, TResult>> function, Box<T1> a, Box<T2> b) =>
            function.Bind(f => Return(f(a.Value, b.Value)));

        public static IEnumerable<T> ToEnumerable<T>(this Box<T> box) =>
            Enumerable.Repeat(box.Value, 1);
    }

    readonly partial struct Box<T> : IEquatable<Box<T>>
    {
        public readonly T Value;

        public Box(T value) => Value = value;

        public override bool Equals(object obj)
            => obj is Box<T> other && Equals(other);

        public bool Equals(Box<T> other) =>
            EqualityComparer<T>.Default.Equals(Value, other.Value);

        public override int GetHashCode() =>
            EqualityComparer<T>.Default.GetHashCode(Value);

        public override string ToString() => $"{Value}";

        public static implicit operator Box<T>(T value) => new Box<T>(value);
        public static implicit operator T(Box<T> box) => box.Value;

        public static bool operator ==(Box<T> left, Box<T> right) => left.Equals(right);
        public static bool operator !=(Box<T> left, Box<T> right) => !left.Equals(right);
    }
}
