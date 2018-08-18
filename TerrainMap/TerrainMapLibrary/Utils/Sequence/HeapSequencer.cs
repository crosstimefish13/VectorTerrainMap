using System;

namespace TerrainMapLibrary.Utils.Sequence
{
    public sealed class HeapSequencer<T> : Sequencer<T> where T : IElement
    {
        public HeapSequencer(ISequence<T> sequence, Func<T, T, int> comparer, StepCounter counter = null)
            : base(sequence, comparer, counter)
        { }


        public override void Sort()
        {
            if (Counter != null)
            { Counter.Reset(Convert.ToInt64(Sequence.Count * Math.Log(Sequence.Count, 2)), 0, "Sorting"); }

            // build max heap
            for (long index = Sequence.Count / 2 - 1; index >= 0; index--)
            { Heapify(index, Sequence.Count - 1); }

            long heapLength = Sequence.Count;
            while (heapLength > 1)
            {
                // reduce the heap length
                heapLength -= 1;

                // swap the top max element with last heap element
                var maxElement = Sequence.GetElement(0);
                var lastElement = Sequence.GetElement(heapLength);
                Sequence.Update(0, lastElement);
                Sequence.Update(heapLength, maxElement);

                // build a max heap again
                Heapify(0, heapLength);
            }

            Sequence.Flush();

            if (Counter != null) { Counter.Reset(Counter.StepLength, Counter.StepLength, "Sorting"); }
        }

        public override void InsertSort(ISequence<T> result)
        {
            throw new NotSupportedException();
        }


        private void Heapify(long topIndex, long heapLength)
        {
            if (Counter != null) { Counter.AddStep(); }

            // max element is top element
            var topElement = Sequence.GetElement(topIndex);
            long maxIndex = topIndex;
            var maxElement = topElement;

            // compare max element with left element if needed
            long leftIndex = topIndex * 2 + 1;
            if (leftIndex < heapLength)
            {
                var leftElement = Sequence.GetElement(leftIndex);
                if (Comparer(leftElement, maxElement) > 0)
                {
                    // set max element to left element
                    maxIndex = leftIndex;
                    maxElement = leftElement;
                }
            }

            // compare max element with right element if needed
            long rightIndex = topIndex * 2 + 2;
            if (rightIndex < heapLength)
            {
                var rightElement = Sequence.GetElement(rightIndex);
                if (Comparer(rightElement, maxElement) > 0)
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
                Heapify(maxIndex, heapLength);
            }
        }
    }
}
