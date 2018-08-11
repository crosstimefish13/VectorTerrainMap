using System;

namespace TerrainMapLibrary.Mathematics.Sequencer
{
    public interface ISequencer<T>
    {
        void Sort(ISequence<T> sequence, Func<T, T, int> comparer, int autoFlushCount = 100);
    }
}
