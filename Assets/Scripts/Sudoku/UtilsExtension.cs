using System;
using System.Collections.Generic;

namespace Sudoku
{
    public static class UtilsExtension
    {
        public static IEnumerable<T> SelectAll<T>(this T[,] array)
        {
            foreach (var item in array)
            {
                yield return item;
            }
        }
        
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            if(random == null)
                random = new Random();
            
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = random.Next(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}