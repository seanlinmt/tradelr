﻿namespace tradelr.DBML.Lucene
{
    public class LuceneHit
    {
        public string id { get; set; }
        public float score { get; set; }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as LuceneHit;
            if (other == null)
            {
                return false;
            }
            return id.Equals(other.id);
        }
    }
}