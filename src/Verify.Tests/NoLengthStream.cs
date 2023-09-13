class NoLengthStream(byte[] bytes) :
    MemoryStream(bytes)
{
    public override long Length => throw new NotImplementedException();
}