partial class SerializationSettings
{
    List<Func<Exception, bool>> ignoreMembersThatThrow = [];

    public void IgnoreMembersThatThrow<T>()
        where T : Exception =>
        ignoreMembersThatThrow.Add(_ => _ is T);

    public void IgnoreMembersThatThrow(Func<Exception, bool> item) =>
        IgnoreMembersThatThrow<Exception>(item);

    public bool ShouldIgnoreException(Exception exception) =>
        ignoreMembersThatThrow.Any(func => func(exception));

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