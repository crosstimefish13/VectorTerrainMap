using System;

namespace TerrainMapLibrary.Utils.Sequence
{
    public abstract class Sequencer
    {
        public ISequence Sequence { get; private set; }

        public Func<byte[], byte[], int> Comparer { get; private set; }


        public Sequencer(ISequence sequence, Func<byte[], byte[], int> comparer)
        {
            if (sequence == null || comparer == null) { throw new ArgumentNullException(); }

            Sequence = sequence;
            Comparer = comparer;
        }


        public abstract void Sort();

        public abstract void InsertSort(ISequence result);
    }
}
