namespace TerrainMapLibrary.Mathematics.Sequencer
{
    public interface ISequence<T>
    {
        T GetItem(long index);

        void UpdateItem(long index, T item);

        long GetCount();

        void Flush();

        bool GetAutoFlush();
    }
}
