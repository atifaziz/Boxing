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

namespace Boxing.Linq
{
    using System;

    static partial class BoxQueryExtensions
    {
        public static Box<TResult>
            SelectMany<TFirst, TSecond, TResult>(
                this Box<TFirst> box,
                Func<TFirst, Box<TSecond>> secondSelector,
                Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (secondSelector == null) throw new ArgumentNullException(nameof(secondSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return box.FlatMap(secondSelector, resultSelector);
        }

        public static Box<TResult>Select<T, TResult>(this Box<T> box, Func<T, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return box.Map(selector);
        }
    }
}