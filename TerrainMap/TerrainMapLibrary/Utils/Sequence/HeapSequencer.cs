using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainMapLibrary.Utils.Sequence
{
    public sealed class HeapSequencer : Sequencer
    {
        public HeapSequencer(ISequence sequence, Func<byte[], byte[], int> comparer)
            : base(sequence, comparer)
        { }


        public override void Sort()
        {
            // build max heap
            for (long index = Sequence.Count / 2 - 1; index >= 0; index--) { Heapify(index); }
        }

        public override void InsertSort(ISequence result)
        {
            throw new NotSupportedException();
        }


        private void Heapify(long topIndex)
        {
            // max element is top element
            var topElement = Sequence.GetElement(topIndex);
            long maxIndex = topIndex;
            var maxElement = topElement;

            // compare max element with left element if needed
            long leftIndex = topIndex * 2 + 1;
            if (leftIndex < Sequence.Count)
            {
                var leftElement = Sequence.GetElement(leftIndex);
                if (Comparer(leftElement, maxElement) > 1)
                {
                    // set max element to left element
                    maxIndex = leftIndex;
                    maxElement = leftElement;
                }
            }

            // compare max element with right element if needed
            long rightIndex = topIndex * 2 + 2;
            if (rightIndex < Sequence.Count)
            {
                var rightElement = Sequence.GetElement(rightIndex);
                if (Comparer(rightElement, maxElement) > 1)
                {
                    // set max element to right element
                    maxIndex = rightIndex;
                    maxElement = rightElement;
                }
            }

            if (maxIndex != topIndex)
            {
                // need to swap max and top
                Sequence.Update(topIndex, maxElement);
                Sequence.Update(maxIndex, topElement);

                // compare the swaped node
                Heapify(maxIndex);
            }
        }
    }
}
