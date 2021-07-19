using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;

namespace StreamChat
{
    internal static class ReflectionHelper
    {
        public static PropertyInfo GetPropertyPortable(this Type type, string propName)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetProperty(propName);
        }

        public static PropertyInfo[] GetPropertiesPortable(this Type type)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetProperties();
        }

        public static MethodInfo GetMethodPortable(this Type type, string methodName)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetMethod(methodName);
        }

        public static MethodInfo GetMethodPortable(this Type type, string methodName, params Type[] types)
        {
            types = types ?? (new Type[0]);

            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetMethod(methodName, types);
        }

        public static Type[] GetInterfacesPortable(this Type type)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetInterfaces();
        }

        public static Expression CreateForEach(ParameterExpression enumerableExp, Func<ParameterExpression, Expression> bodyGenerator)
        {
            var breakLabel = Expression.Label("ForeachBreak");
            var getEnumeratorMethod = enumerableExp.Type.GetMethodPortable(nameof(IEnumerable.GetEnumerator));
            var enumeratorType = getEnumeratorMethod.ReturnType;
            var enumeratorExp = Expression.Parameter(enumeratorType, "enumerator");
            var moveNextMethod = typeof(IEnumerator).GetMethodPortable(nameof(IEnumerator.MoveNext));
            var currentProperty = enumeratorType.GetPropertyPortable(nameof(IEnumerator.Current));
            var currentExpression = Expression.Parameter(currentProperty.PropertyType, "current");

            Expression loopBlock = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(enumeratorExp, moveNextMethod),
                            Expression.Block(
                                new[] { currentExpression },
                                Expression.Assign(
                                    currentExpression,
                                    Expression.Property(enumeratorExp, currentProperty)),
                                bodyGenerator(currentExpression)
                                ),
                            Expression.Break(breakLabel)));

            var diposableType = typeof(IDisposable);

            if (enumeratorType.GetInterfacesPortable().Contains(diposableType))
            {
                var disposeMethod = diposableType.GetMethodPortable(nameof(IDisposable.Dispose));

                loopBlock = Expression.TryFinally(loopBlock, Expression.Call(Expression.Convert(enumerableExp, diposableType), disposeMethod));
            }

            var res = Expression.Block(
                    new[]
                    {
                        enumeratorExp
                    },
                    Expression.Assign(
                        enumeratorExp,
                        Expression.Call(enumerableExp, getEnumeratorMethod)),
                    loopBlock,
                    Expression.Label(breakLabel)
                );

            return res;
        }
    }
}
