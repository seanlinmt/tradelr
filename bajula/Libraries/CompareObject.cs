using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using tradelr.Common.Models.currency;
using tradelr.Library;

namespace tradelr.Libraries
{
    public class CompareObject
    {
        readonly Dictionary<string, Pair<object, object>> changes = new Dictionary<string, Pair<object, object>>();

        public Dictionary<string, Pair<object, object>> Compare<T>(T original, T changed, Currency currency = null)
        {
            var properties = typeof(T).GetProperties();
            CompareRecursive(properties, original, changed);
            return changes;
        }

        private void CompareRecursive<P>(IEnumerable<PropertyInfo> properties, P original, P changed)
        {
            foreach (var info in properties)
            {
                var name = info.Name;
                var originalValue = info.GetValue(original, null);
                var newValue = info.GetValue(changed, null);
                if (!info.PropertyType.IsValueType && // not values
                    info.PropertyType != typeof(string) &&  // not string
                    !typeof(IList).IsAssignableFrom(info.PropertyType))  // not list
                {
                    continue;
                }

                if (typeof(IList).IsAssignableFrom(info.PropertyType))
                {
                    if (originalValue == null && newValue == null)
                    {
                        // do nothing
                    }
                    else if (originalValue == null)
                    {
                        changes.Add(info.Name, new Pair<object, object>{Second = "added"});
                    }
                    else if (newValue == null)
                    {
                        changes.Add(info.Name, new Pair<object, object>{Second = "deleted"});
                    }
                    else 
                    {
                        var oldList = (IList)originalValue;
                        var newList = (IList)newValue;
                        if (oldList.Count > newList.Count)
                        {
                            changes.Add(info.Name, new Pair<object, object>{Second = "deleted"});
                        }
                        else if (oldList.Count < newList.Count)
                        {
                            changes.Add(info.Name, new Pair<object, object>{Second = "added"});
                        }
                        else
                        {
                            var memberType = info.PropertyType.GetGenericArguments()[0];
                            for (int i = 0; i < newList.Count; i++)
                            {
                                var oldEntry = oldList[i];
                                var newEntry = newList[i];
                                CompareRecursive(memberType.GetProperties(), oldEntry, newEntry);
                            }
                        }
                    }
                }
                else
                {
                    if (originalValue == null || newValue == null)
                    {
                        continue;
                    }
                    
                    if (!originalValue.Equals(newValue))
                    {
                        if (!changes.ContainsKey(name))
                        {
                            changes.Add(name, new Pair<object, object> { First = originalValue, Second = newValue });
                        }
                    }
                }
            }

        }
    }
}
