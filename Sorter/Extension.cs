using System.Collections.Generic;

namespace Sorter
{
    static class ListExtension
    {
        public static T Pop<T>(this List<T> list)
        {
            T elem = list[0];
            list.RemoveAt(0);
            return elem;
        }
    }
}
