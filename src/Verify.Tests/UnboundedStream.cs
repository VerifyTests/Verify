class UnboundedStream :
    MemoryStream
{
    public override long Length => throw new NotImplementedException();
}