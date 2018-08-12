namespace TerrainMapLibrary.Utils.Sequence
{
    public interface ISequence
    {
        long Count { get; }

        void Add(byte[] element);

        void Update(long index, byte[] element);

        byte[] GetElement(long index);
    }
}
