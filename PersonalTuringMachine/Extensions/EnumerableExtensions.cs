using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PersonalTuringMachine.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToDelimitedList<T>(this IEnumerable<T> list, string delimiter = ",")
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

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumeration)
        {
            if (enumeration == null) return null;
            ObservableCollection<T> oc = new ObservableCollection<T>();
            foreach(T item in enumeration)
            {
                oc.Add(item);
            }
            return oc;
        }
    }
}
