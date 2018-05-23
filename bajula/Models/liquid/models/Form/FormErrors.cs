using System.Collections;
using System.Collections.Generic;
using DotLiquid;

namespace tradelr.Models.liquid.models.Form
{
    public class FormErrors : Drop, IList<string>
    {
        private readonly List<string> errorFields;
        public Dictionary<string, string> messages { get; private set; }

        public FormErrors()
        {
            errorFields = new List<string>();
            messages = new Dictionary<string, string>();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return errorFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string item)
        {
            errorFields.Add(item);
        }

        public void Clear()
        {
            errorFields.Clear();
        }

        public bool Contains(string item)
        {
            return errorFields.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string item)
        {
            return errorFields.Remove(item);
        }

        public int Count
        {
            get { return errorFields.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(string item)
        {
            return errorFields.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            errorFields.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            errorFields.RemoveAt(index);
        }

        public string this[int index]
        {
            get { return errorFields[index]; }
            set { errorFields[index] = value; }
        }
    }
}