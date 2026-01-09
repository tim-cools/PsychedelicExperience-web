using System;
using System.Globalization;
using System.Text;

namespace PsychedelicExperience.Common.Tests
{
    public class Test : IDisposable
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private bool _errors;
        private int _indention;

        private Test()
        {
        }

        public static void All(Action<Test> testHandler)
        {
            if (testHandler == null) throw new ArgumentNullException("testHandler");

            using (var context = new Test())
            {
                testHandler(context);
            }
        }

        private void Verify()
        {
            var summary = _stringBuilder.ToString();
            if (_errors)
            {
                throw new InvalidOperationException(summary);
            }

            Console.WriteLine(summary);
        }

        public void Dispose()
        {
            Verify();
        }

        public Test Fail(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);
            _stringBuilder.AppendLine(">> " + message);
            _errors = true;
            return this;
        }

        public Test Action(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);
            _stringBuilder.AppendLine(message);
            return this;
        }

        public Test AreEqual(bool expected, bool actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual<T>(T expected, T actual, string message = null) where T : IComparable
        {
            return LogAssert(expected.CompareTo(actual) == 0, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual<T>(T? expected, T? actual, string message = null) where T : struct, IComparable
        {
            return LogAssert(
                (expected == null && actual == null)
             || (expected != null && actual != null && expected.Value.CompareTo(actual.Value) == 0), message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(bool? expected, bool? actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(Guid expected, Guid actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(string expected, string actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(int expected, int actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(int? expected, int? actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(decimal expected, decimal actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(DateTime expected, DateTime actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreEqual(DateTime? expected, DateTime? actual, string message = null)
        {
            return LogAssert(expected == actual, message, "- AreEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreNotEqual(string expected, string actual, string message = null)
        {
            return LogAssert(expected != actual, message, "- AreNotEqual Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test AreSame<T>(T expected, T actual, string message = null)
        {
            return LogAssert(ReferenceEquals(expected, actual), message, "- AreSame Failed '{0}' != '{1}': ", expected, actual);
        }

        public Test IsEmpty(string value, string message = null)
        {
            return LogAssert(value == string.Empty, message, "- IsEmpty Failed '{0}': ", value);
        }

        public Test IsNotNull(object value, string message = null)
        {
            return LogAssert(value != null, message, "- IsNotNull Failed '{0}': ", value);
        }

        public Test IsNotNull(object value, Action<Test> subContext, string message = null)
        {
            var valid = value != null;
            if (valid)
            {
                _indention++;
                subContext(this);
                _indention--;

                return this;
            }

            return LogAssert(false, message, "- IsNotNull Failed '': ");
        }

        public Test IsNotNull<T>(T value, Action<T, Test> subContext, string message = null) where T : class
        {
            var valid = value != null;
            if (valid)
            {
                _indention++;
                subContext(value, this);
                _indention--;

                return this;
            }

            return LogAssert(false, message, "- IsNotNull Failed '': ");
        }

        public Test IsNull(object value, string message = null)
        {
            return LogAssert(value == null, message, "- IsNull Failed '{0}': ", value);
        }

        public Test IsTrue(bool value, string message = null)
        {
            return LogAssert(value, message, "- IsTrue Failed '{0}': ", value);
        }

        public Test IsTrue(bool? value, string message = null)
        {
            return LogAssert(value == true, message, "- IsTrue Failed '{0}': ", value);
        }

        public Test IsFalse(bool value, string message = null)
        {
            return LogAssert(!value, message, "- IsFalse Failed '{0}': ", value);
        }

        public Test IsFalse(bool? value, string message = null)
        {
            return LogAssert(value == false, message, "- IsFalse Failed '{0}': ", value);
        }

        public Test Assert(Action assertAction, string message = null)
        {
            assertAction();
            return this;
        }

        public Test AssertThrowsException<T>(Action assertAction, string message = null) where T : Exception
        {
            try
            {
                assertAction();

                LogAssert(false, message, "- AssertThrowsException Failed: No Exception thorwn");

                return this;
            }
            catch (Exception exception)
            {
                LogAssert(exception.GetType() == typeof(T), message, "- AssertThrowsException Failed: Wrong exception type: '{0}'", exception.GetType());

                return this;
            }
        }

        public Test IsOfType<TExpected>(object actual, Action<Test, TExpected> subContext, string message = null) where TExpected : class
        {
            var subInstance = actual as TExpected;
            var valid = subInstance != null;
            if (valid)
            {
                _indention++;
                subContext(this, subInstance);
                _indention--;

                return this;
            }

            return LogAssert(false, message, "- IsOfType<{0}> Failed '{1}': ", typeof(TExpected), actual != null ? actual.GetType().ToString() : "<null>");
        }

        internal Test LogAssert(bool valid, string message, string title, params object[] args)
        {
            if (valid) return this;

            if (_indention > 0)
            {
                _stringBuilder.Append(new string(' ', _indention * 3));
            }

            var titleFormat = string.Format(title, args);
            _stringBuilder.AppendLine(titleFormat + message);
            _errors = true;
            return this;
        }
    }
}
