using System;

namespace TerrainMapLibrary.Utils.Sequence
{
    public abstract class Sequencer<T> where T : IElement
    {
        public ISequence<T> Sequence { get; private set; }

        public Func<T, T, int> Comparer { get; private set; }

        public StepCounter Counter { get; set; }

        public Sequencer(ISequence<T> sequence, Func<T, T, int> comparer, StepCounter counter = null)
        {
            if (sequence == null || comparer == null)
            {
                throw new ArgumentNullException();
            }

            Sequence = sequence;
            Comparer = comparer;
            Counter = counter;
        }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode() + Comparer.GetHashCode() + (Counter == null ? 0 : Counter.GetHashCode());
        }

        public override string ToString()
        {
            return $"Sequencer: {GetType().FullName}, Sequence: {Sequence.GetType().FullName}";
        }

        public abstract void Sort();

        public abstract void InsertSort(ISequence<T> result);
    }
}
