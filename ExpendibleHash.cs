using System;
using System.Security.Cryptography;

namespace ExpendibleHashing
{
	public class ExpendibleHash
	{
		private int GlobalDepth;
		private static ExpendibleHash instance;
		private List<Bucket> Directory;
		private HashAlgorithm HashFn;
		private BytesConverter bytesConverter;
		private int BucketSize;

        public static ExpendibleHash Instance(HashAlgorithm fn, int globalDepth, int bucketSize)
		{
			if(instance == null)
			{
				instance = new ExpendibleHash(fn, globalDepth, bucketSize);
            }
			return instance;
		}

        private ExpendibleHash(HashAlgorithm fn, int globalDepth, int bucketSize)
		{
			this.HashFn = fn;
			this.bytesConverter = new BytesConverter();
			this.GlobalDepth = globalDepth;
			this.Directory = new();
            this.BucketSize = bucketSize;
            this.InitializeDirectory();
        }

		public void Add(string key, object data)
		{
			int LSB = FindLsb(key);
            if (this.Directory[LSB].IsOverFlow)
			{
				if(this.Directory[LSB].LocalDepth == this.GlobalDepth)
				{
					Console.WriteLine($"Directory level rehashing happening, globalDepth {this.GlobalDepth}");
					// if bucket LocalDepth == Directory global depth then
					// increase the global depth of directory and split the overflowed bucket
                    this.GlobalDepth++;
                    this.InitializeDirectory();
                    this.ReHashDirectory(LSB);
                    Console.WriteLine($"Directory level rehashing Completed, globalDepth {this.GlobalDepth}");
                    this.Add(key, data);
                } else
				{
					Console.WriteLine($"Bucket level rehashing happening: {LSB} LocalDepth {this.Directory[LSB].LocalDepth}");
                    // if bucket locak depth < directory global depth then
                    // split the overflowed bucket and increase the local depth by 1
                    this.ReHashDirectory(LSB);
                }
			}
			else
			{
				this.Directory[LSB].AddItem(key, data);
            }
        }


        public T FindItem<T>(string key)
		{
			int lsb = FindLsb(key);
			return this.Directory[lsb].FindItem<T>(key);
        }

		private void ReHashDirectory(int LSB)
		{
			IEnumerator<BucketData> enumerator = this.Directory[LSB].GetEnumerator();
			Bucket bktToSplit = this.Directory[LSB];

			for(int bkt = 0; bkt < this.Directory.Count; bkt++)
			{
				if (this.Directory[bkt] == bktToSplit)
				{
                    this.Directory[bkt] = new Bucket(bktToSplit.LocalDepth + 1, this.BucketSize);
				}
			}

            while (enumerator.MoveNext())
			{
				BucketData _data = enumerator.Current;
				this.Add(_data.Key, _data.Data);
            }
        }

		// Find the Least Significant Bit
		private int FindLsb(object data)
		{
            byte[] hash = this.ComputeHash(data);
            long value = (long)BitConverter.ToUInt64(hash);
            int LSB = (int)(value & ((1 << this.GlobalDepth) - 1));
            Console.WriteLine($"LSB value for key: {data} is {LSB}");
            return LSB;
        }

        private byte[] ComputeHash(object data)
        {
            byte[] bytes = this.bytesConverter.GetBytes(data);
            byte[] hash = this.HashFn.ComputeHash(bytes);

            return hash;
        }

		private void InitializeDirectory()
		{
			int size = (int)Math.Pow(2, this.GlobalDepth);
			int startIndex = 0;
			if(this.Directory.Count == 0)
			{
				for(int i = 0; i < size; i++)
				{
					this.Directory.Add(new Bucket(this.GlobalDepth, this.BucketSize));
				}

				return;
			}

            for (int i = this.Directory.Count; i < size; i++, startIndex++)
			{
				this.Directory.Add(this.Directory[startIndex]);
            }
		}
    }
}

