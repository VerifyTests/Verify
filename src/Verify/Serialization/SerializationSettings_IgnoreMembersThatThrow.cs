partial class SerializationSettings
{
    internal List<Func<Exception, bool>> ignoreMembersThatThrow = new();

    public void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ignoreMembersThatThrow.Add(_ => _ is T);

    public void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        IgnoreMembersThatThrow<Exception>(item);

    public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
        where T : Exception =>
        ignoreMembersThatThrow.Add(
            _ =>
            {
                if (_ is T exception)
                {
                    return item(exception);
                }

                return false;
            });
}