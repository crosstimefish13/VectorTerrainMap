namespace TerrainMapLibrary.Utils.Sequence
{
    public interface IElement
    {
        int ArrayLength { get; }

        void Initialize(byte[] array);

        byte[] ToArray();
    }
}
