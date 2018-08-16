namespace TerrainMapLibrary.Utils.Sequence
{
    public interface ISequence<T> where T : IElement
    {
        long Count { get; }

        void Add(T element);

        void Update(long index, T element);

        T GetElement(long index);

        void Flush();
    }
}
