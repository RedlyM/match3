using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchThree.Utils
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = new Random();

        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source as IList<T> ?? source.ToList();

            if (list.Count == 0)
            {
                throw new InvalidOperationException("Sequence is empty.");
            }

            int index = _random.Next(list.Count);
            return list[index];
        }
    }

}