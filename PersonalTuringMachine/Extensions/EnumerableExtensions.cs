using System.Collections.Generic;

namespace PersonalTuringMachine.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToDelimitedList<T>(this IEnumerable<T> list, char delimiter = ',')
        {
            string delimitedList = string.Empty;
            foreach (T obj in list)
            {
                string str = obj.ToString();
                delimitedList = delimitedList + str + delimiter;
            }
            if (delimitedList.Length > 0)
            {
                delimitedList = delimitedList.TrimEnd(delimiter);
            }

            return delimitedList;
        }
    }
}
