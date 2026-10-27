namespace BMECat.net_Test
{
    /// <summary>
    /// A stream that can be read and written but not repositioned, just like e.g. a network stream
    /// </summary>
    internal class NonSeekableStream : MemoryStream
    {
        public NonSeekableStream()
            : base()
        {
        }


        public NonSeekableStream(byte[] buffer)
            : base(buffer, false)
        {
        }


        public override bool CanSeek => false;


        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }


        public override long Length => throw new NotSupportedException();


        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException();
        }
    }
}
