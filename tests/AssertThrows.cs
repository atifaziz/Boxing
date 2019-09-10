namespace Boxing.Tests
{
    using System;
    using NUnit.Framework;

    static class AssertThrows
    {
        public static void ArgumentNullException(string name, TestDelegate action)
        {
            var e = Assert.Throws<ArgumentNullException>(action);
            Assert.That(e.ParamName, Is.EqualTo(name));
        }
    }
}