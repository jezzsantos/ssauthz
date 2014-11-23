using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using Moq.Language.Flow;

namespace Testing.Common
{
    /// <summary>
    ///     Extensions to the <see cref="Mock" /> class.
    /// </summary>
    public static class MoqExtensions
    {
        /// <summary>
        ///     Setup an expectation to return a sequence of values
        /// </summary>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup,
            params TResult[] results) where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }

        /// <summary>
        ///     Setup an expectation to return a sequence of values
        /// </summary>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup,
            params object[] results) where T : class
        {
            var queue = new Queue(results);
            setup.Returns(() =>
            {
                object result = queue.Dequeue();
                if (result is Exception)
                {
                    throw result as Exception;
                }
                return (TResult) result;
            });
        }
    }
}