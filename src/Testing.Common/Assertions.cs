using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Common
{
    /// <summary>
    ///     Hook interface for all assertion extension methods we might ever need :).
    /// </summary>
    public interface IAssertion
    {
    }

    /// <summary>
    ///     Basic implementation of an assertion, which provides some helper methods that extensions
    ///     can use.
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public class Assertion : IAssertion
    {
        /// <summary>
        ///     If a custom message is provided, it's formatted with the given arguments (if any)
        ///     and prepended to the fixed message, with a new-line separating them.
        /// </summary>
        public static string FormatMessage(string fixedMessage, string customMessage, params object[] args)
        {
            if (!customMessage.HasValue())
            {
                return fixedMessage;
            }
            return customMessage.FormatWith(args) + Environment.NewLine + fixedMessage;
        }
    }

    [DebuggerStepThrough]
    [DebuggerNonUserCode]
    public static class BasicAssertions
    {
        public static void Contains<T>(this IAssertion assertion, T expected, IEnumerable<T> collection)
        {
            CollectionAssert.Contains(new List<T>(collection), expected);
        }

        public static void OfType<T>(this IAssertion assertion, T value, Type expectedType)
        {
            Assert.IsInstanceOfType(value, expectedType);
        }

        public static void OfType<T>(this IAssertion assertion, T value, Type expectedType, string message,
            params object[] args)
        {
            Assert.IsInstanceOfType(value, expectedType, message, args);
        }

        public static void Contains<T>(this IAssertion assertion, T expected, IEnumerable<T> collection, string message,
            params object[] args)
        {
            CollectionAssert.Contains(new List<T>(collection), expected, message, args);
        }

        public static void Equal<T>(this IAssertion assertion, T expected, T actual)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void Equal<T>(this IAssertion assertion, T expected, T actual, string message,
            params object[] args)
        {
            Assert.AreEqual(expected, actual, message, args);
        }

        public static void NotEqual<T>(this IAssertion assertion, T expected, T actual)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void NotEqual<T>(this IAssertion assertion, T expected, T actual, string message,
            params object[] args)
        {
            Assert.AreNotEqual(expected, actual, message, args);
        }

        public static void Null(this IAssertion assertion, object @object)
        {
            Assert.IsNull(@object);
        }

        public static void Null(this IAssertion assertion, object @object, string message, params object[] args)
        {
            Assert.IsNull(@object, message, args);
        }

        public static void NotNull(this IAssertion assertion, object @object)
        {
            Assert.IsNotNull(@object);
        }

        public static void NotNull(this IAssertion assertion, object @object, string message, params object[] args)
        {
            Assert.IsNotNull(@object, message, args);
        }

        public static void Same(this IAssertion assertion, object expected, object actual)
        {
            Assert.AreSame(expected, actual);
        }

        public static void Same(this IAssertion assertion, object expected, object actual, string message,
            params object[] args)
        {
            Assert.AreSame(expected, actual, message, args);
        }

        public static void NotSame(this IAssertion assertion, object notExpected, object actual)
        {
            Assert.AreNotSame(notExpected, actual);
        }

        public static void NotSame(this IAssertion assertion, object notExpected, object actual, string message,
            params object[] args)
        {
            Assert.AreNotSame(notExpected, actual, message, args);
        }

        public static void Fail(this IAssertion assertion)
        {
            Assert.Fail();
        }

        public static void Fail(this IAssertion assertion, string message, params object[] args)
        {
            Assert.Fail(message, args);
        }

        public static void True(this IAssertion assertion, bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void True(this IAssertion assertion, bool condition, string message, params object[] args)
        {
            Assert.IsTrue(condition, message, args);
        }

        public static void False(this IAssertion assertion, bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void False(this IAssertion assertion, bool condition, string message, params object[] args)
        {
            Assert.IsFalse(condition, message, args);
        }

        public static void Inconclusive(this IAssertion assertion)
        {
            Assert.Inconclusive();
        }

        public static void Inconclusive(this IAssertion assertion, string message, params object[] parameters)
        {
            Assert.Inconclusive(message, parameters);
        }

        public static Exception Throws<T>(this IAssertion assertion, string exceptionContains, Action action)
            where T : Exception
        {
            return Throws<T>(assertion, exceptionContains, action, null, new object[0]);
        }

        public static Exception Throws<T>(this IAssertion assertion, string exceptionContains, Action action,
            string message,
            params object[] args)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (!(ex is T))
                {
                    throw BuildThrowsException(ex, Assertion.FormatMessage(
                        string.Format("expected exception of type {0} but was {1}.", typeof (T), ex.GetType()),
                        message, args));
                }

                if (!ex.Message.IsFormattedFrom(exceptionContains))
                {
                    throw BuildThrowsException(ex, Assertion.FormatMessage(
                        string.Format("throw exception did not match message regex match pattern '{0}'.",
                            exceptionContains),
                        message, args));
                }

                return ex;
            }

            throw new AssertFailedException(Assertion.FormatMessage("Assert.Throws() failure", message, args));
        }

        public static T Throws<T>(this IAssertion assertion, Action action)
            where T : Exception
        {
            return Throws<T>(assertion, action, null, new object[0]);
        }

        public static T Throws<T>(this IAssertion assertion, Action action, string message, params object[] args)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (!(ex is T))
                {
                    throw BuildThrowsException(ex,
                        string.Format("expected exception of type {0} but was {1}.", typeof (T),
                            ex.GetType()));
                }

                return (T) ex;
            }

            throw new AssertFailedException(Assertion.FormatMessage(
                "Assert.Throws() did not throw any exception. Expected " + typeof (T).Name + ".", message, args));
        }

        #region StackTrace Hacks

        internal static Exception BuildThrowsException(Exception ex, string message)
        {
            PreserveStackTrace(ex);
            Exception result = new AssertFailedException("Assert.Throws() failure: " + message + "\r\n" + ex.Message);

            FieldInfo remoteStackTraceString = typeof (Exception)
                .GetField("_remoteStackTraceString",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            string currentMethod = MethodBase.GetCurrentMethod().Name;
            string stackTrace = String.Join(Environment.NewLine,
                ex.StackTrace
                    .Split(new[]
                    {
                        Environment.NewLine
                    }, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .Where(frame => !frame.Contains(currentMethod))
                    .ToArray());

            remoteStackTraceString.SetValue(
                result,
                stackTrace + Environment.NewLine);

            // Roundtrip via serialization to cause the full exception data to be persisted, 
            // and to cause the remote stacktrace to be cleaned-up.
            var selector = new ExceptionSurrogateSelector();
            var formatter = new BinaryFormatter(selector, new StreamingContext(StreamingContextStates.All));
            using (var mem = new MemoryStream())
            {
                formatter.Serialize(mem, result);
                mem.Position = 0;
                result = (Exception) formatter.Deserialize(mem);
                PreserveStackTrace(result);
            }

            return result;
        }

        private static void PreserveStackTrace(Exception exception)
        {
            MethodInfo preserveStackTrace = typeof (Exception).GetMethod("InternalPreserveStackTrace",
                BindingFlags.Instance | BindingFlags.NonPublic);
            preserveStackTrace.Invoke(exception, null);
        }

        private class ExceptionSurrogate : ISerializationSurrogate
        {
            private static readonly MethodInfo updateMethod = typeof (SerializationInfo).GetMethod("UpdateValue",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.InvokeMethod);

            private static readonly Action<SerializationInfo, string, object, Type> updateValue;

            static ExceptionSurrogate()
            {
                updateValue = (info, name, value, type) => updateMethod.Invoke(info, new[]
                {
                    name,
                    value,
                    type
                });
            }

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                ((ISerializable) obj).GetObjectData(info, context);

                // Skip our own frames to cleanup the trace.
                string stackTrace = String.Join(Environment.NewLine,
                    info.GetString("RemoteStackTraceString")
                        .Split(new[]
                        {
                            Environment.NewLine
                        },
                            StringSplitOptions.RemoveEmptyEntries)
                        .Where(frame => !frame.Contains("Assertions"))
                        .ToArray());

                updateValue(info, "RemoteStackTraceString", stackTrace, typeof (string));
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
                ISurrogateSelector selector)
            {
                ConstructorInfo serializationCtor = obj.GetType().GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[]
                    {
                        typeof (SerializationInfo),
                        typeof (StreamingContext)
                    },
                    null);

                Debug.Assert(serializationCtor != null, "Serialization constructor not found.", "Object type {0}.",
                    obj.GetType());

                serializationCtor.Invoke(obj, new object[]
                {
                    info,
                    context
                });

                return obj;
            }
        }

        private class ExceptionSurrogateSelector : ISurrogateSelector
        {
            private ISurrogateSelector selector;

            public void ChainSelector(ISurrogateSelector selector)
            {
                this.selector = selector;
            }

            public ISurrogateSelector GetNextSelector()
            {
                return selector;
            }

            public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context,
                out ISurrogateSelector selector)
            {
                if (typeof (Exception).IsAssignableFrom(type))
                {
                    selector = this;

                    return new ExceptionSurrogate();
                }
                selector = null;
                return null;
            }
        }

        #endregion
    }
}