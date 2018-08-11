using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainMapLibrary.Mathematics.Sequencer
{
    public sealed class HeapSequencer<T> : ISequencer<T>
    {
        public HeapSequencer()
        { }


        public void Sort(ISequence<T> sequence, Func<T, T, int> comparer, int autoFlushCount)
        {
            // build max heap
            for (long index = sequence.GetCount() / 2 - 1; index >= 0; index--)
            { Heapify(sequence, comparer, index); }
        }


        private void Heapify(ISequence<T> sequence, Func<T, T, int> comparer, long topIndex)
        {
            // max item is top item
            long maxIndex = topIndex;
            var topItem = sequence.GetItem(topIndex);
            var maxItem = topItem;
            long count = sequence.GetCount();

            // compare max item with left item
            long leftIndex = topIndex * 2 + 1;
            if (leftIndex < count)
            {
                var leftItem = sequence.GetItem(leftIndex);
                if (comparer(leftItem, maxItem) > 1)
                {
                    // set max item to left item
                    maxIndex = leftIndex;
                    maxItem = leftItem;
                }
            }

            // compare max item with right item
            long rightIndex = topIndex * 2 + 2;
            if (rightIndex < count)
            {
                var rightItem = sequence.GetItem(rightIndex);
                if (comparer(rightItem, maxItem) > 1)
                {
                    // set max item to right item
                    maxIndex = rightIndex;
                    maxItem = rightItem;
                }
            }

            if (maxIndex != topIndex)
            {
                // need to swap max and top
                sequence.UpdateItem(topIndex, maxItem);
                sequence.UpdateItem(maxIndex, topItem);

                // compare the swaped node
                Heapify(sequence, comparer, maxIndex);
            }
        }
    }
}
