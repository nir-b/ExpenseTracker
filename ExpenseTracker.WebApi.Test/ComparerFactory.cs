using System;
using System.Collections.Generic;

namespace ExpenseTracker.WebApi.Test
{
    public static class ComparerFactory
    {
        public static Comparer<T> GetComparer<T>(Func<T, T, bool> comparerFunc)
        {
            return new Comparer<T>(comparerFunc);
        }
    }

    public class Comparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparerFunc;

        public Comparer(Func<T, T, bool> comparerFunc)
        {
            _comparerFunc = comparerFunc;
        }

        public bool Equals(T x, T y)
        {
            return _comparerFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}