namespace Qsi.Tree.Data
{
    internal sealed class UserDataHolder : IUserDataHolder
    {
        private Bucket _head;

        public T GetData<T>(Key<T> key)
        {
            for (var bucket = _head; bucket != null; bucket = bucket.Next)
            {
                if (bucket.Key == key)
                    return (T)bucket.Value;
            }

            return default;
        }

        public void PutData<T>(Key<T> key, T value)
        {
            if (Equals(value, default(T)))
            {
                Bucket prevBucket = null;

                for (var bucket = _head; bucket != null; bucket = bucket.Next)
                {
                    if (bucket.Key == key)
                    {
                        if (prevBucket == null)
                        {
                            _head = bucket.Next;
                            break;
                        }

                        prevBucket.Next = bucket.Next;
                        break;
                    }

                    prevBucket = bucket;
                }
            }
            else
            {
                for (var bucket = _head; bucket != null; bucket = bucket.Next)
                {
                    if (bucket.Key == key)
                    {
                        bucket.Value = value;
                        return;
                    }
                }

                _head = new Bucket
                {
                    Key = key,
                    Value = value,
                    Next = _head?.Next
                };
            }
        }

        private sealed class Bucket
        {
            public object Key { get; set; }

            public object Value { get; set; }

            public Bucket Next { get; set; }
        }
    }
}
