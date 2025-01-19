using System;
namespace ExpendibleHashing
{
	public class Bucket
	{
        public int LocalDepth;
        private List<BucketData> List;
        private int BucketSize;
        public Bucket(int localDepth, int bucketSize)
        {
            this.LocalDepth = localDepth;
            this.BucketSize = bucketSize;
            List = new();
        }

        public bool IsOverFlow
        {
            get
            {
                return this.List.Count == this.BucketSize;
            }
        }

        public void AddItem(string key, object data)
        {
            this.List.Add(new BucketData(key, data));
        }

        public T FindItem<T>(string key)
        {
            BucketData _data = this.List.Find((item) => item.Key == key);

            T result = (T)Convert.ChangeType(_data.Data, typeof(T));
            return result;
        }

        public IEnumerator<BucketData> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }
    }

    public class BucketData
    {
        public string Key;
        public object Data;
        public BucketData(string key, object data)
        {
            Key = key;
            Data = data;
        }
    }
}

