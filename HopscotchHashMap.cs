using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopscotchHashMap
{
    public sealed class HopscotchHashMap<TKey, TValue>
    {
        struct Lock
        {
            public void Init() { }
        }

        struct Bucket
        {
            public volatile short firstDelta;
            public volatile short nextDelta;
            public volatile uint hash;
            public volatile TKey key;
            public volatile TValue data;

            public void Init() { }
        }

        struct Segment
        {
            public volatile uint timestamp;
		    Lock _lock;

            public void Init()
            {
                timestamp = 0;
                _lock.Init();
            }
        }

        volatile uint _segmentShift;
        volatile uint _segmentMask;
        volatile uint _bucketMask;
        volatile Segment[] _segments;
        volatile Bucket[] _table;

	    readonly int _cache_mask;
        readonly bool _is_cacheline_alignment;

        const uint InsertRange = 1024 * 4;
        const uint ResizeFactor = 2;
        const short NullDelta = short.MinValue;

        private void RemoveKey(ref Segment segment, ref Bucket fromBucket, ref Bucket keyBucket, ref Bucket prevKeyBucket, uint hash) 
	    {
            keyBucket.hash = 0;
            keyBucket.key = default(TKey);
            keyBucket.data = default(TValue);

		    if (prevKeyBucket == null)
            {
			    if (keyBucket.nextDelta == NullDelta)
				    fromBucket.firstDelta = NullDelta;
			    else 
				    fromBucket.firstDelta = checked((short)(fromBucket.firstDelta + keyBucket.nextDelta));
		    } else {
			    if (keyBucket.nextDelta == NullDelta)
				    prevKeyBucket.nextDelta = NullDelta;
			    else 
				    prevKeyBucket.nextDelta = checked((short)(prevKeyBucket.nextDelta + keyBucket.nextDelta));
		    }

		    ++(segment.timestamp); //TODO: atomic?
		    keyBucket.nextDelta = NullDelta;
	    }

        private void AddKeyToBeginingOfList(ref Bucket keysBucket, ref Bucket freeBucket, uint hash, TKey key, TValue data) 
	    {
		    freeBucket.data = data;
		    freeBucket.key = key;
		    freeBucket.hash = hash;

		    if (keysBucket.firstDelta == 0)
            {
			    if(keysBucket.nextDelta == NullDelta)
				    freeBucket.nextDelta = NullDelta;
			    else
				    freeBucket.nextDelta = (short)((keysBucket +  keysBucket.nextDelta) -  freeBucket);
			    keys_bucket->_next_delta = (short)(free_bucket - keys_bucket);
		    }
            else
            {
			    if(keysBucket.firstDelta == NullDelta)
				    freeBucket.nextDelta = NullDelta;
			    else
				    freeBucket.nextDelta = (short)((keysBucket +  keysBucket.firstDelta) -  freeBucket);
			    keysBucket.firstDelta = (short)(free_bucket - keys_bucket);
		    }
	    }
    }
}
