#region Copyright (c) 2019 Atif Aziz. All rights reserved.
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

namespace Boxing.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Reactive;

    static class TestExtensions
    {
        public static void
            AssertThat<T>(this IObservable<T> source,
                IResolveConstraint error,
                IResolveConstraint completed,
                IResolveConstraint observations)
        {
            var observationList = new List<T>();
            var exception = (Exception)null;
            var hasCompleted = false;

            var subscription =
                source.Subscribe(observationList.Add,
                                 error != null ? new Action<Exception>(e => exception = e) : null,
                                 completed != null ? new Action(() => hasCompleted = true) : null);

            subscription.Dispose();

            if (error != null)
                Assert.That(exception, error);
            if (completed != null)
                Assert.That(hasCompleted, completed);
            Assert.That(observationList, observations);
        }
    }
}
