using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Common.Reflection;

namespace Testing.Common
{
    public static class FluentValidation<TTarget>
    {
        public static string NotNullErrorMessage<TResult>(Expression<Func<TTarget, TResult>> property)
        {
            string parameterName = Reflector<TTarget>.GetPropertyName(property);

            return NotNullErrorMessage(parameterName);
        }

        public static string NotEmptyErrorMessage<TResult>(Expression<Func<TTarget, TResult>> property)
        {
            string parameterName = Reflector<TTarget>.GetPropertyName(property);

            return NotEmptyErrorMessage(parameterName);
        }

        private static string NotNullErrorMessage(string parameterName)
        {
            string paramName = GetSentence(parameterName);

            return "'{0}' must not be empty".FormatWith(paramName);
        }

        private static string NotEmptyErrorMessage(string parameterName)
        {
            string paramName = GetSentence(parameterName);

            return "'{0}' should not be empty".FormatWith(paramName);
        }

        private static string GetSentence(string text)
        {
            int counter = 0;
            var sentence = new StringBuilder();
            text.ToList()
                .ForEach(c =>
                {
                    if (counter++ > 0
                        && char.IsUpper(c))
                    {
                        sentence.AppendFormat(" {0}", c);
                    }
                    else
                    {
                        sentence.Append(c);
                    }
                });

            return sentence.ToString();
        }
    }
}