namespace GZipTest
{
    public class Block
    {
        public int Id { get; }
        public byte[] Data { get; }
        public byte[] Processed { get; set; }

        public static readonly Block Null = new Block(-1, new byte[0]);

        public Block(int id, byte[] data)
        {
            Id = id;
            Data = data;
        }
    }
}