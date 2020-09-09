namespace VerifyTests
{
    public readonly struct ToAppend
    {
        public string Name { get; }
        public object Data { get; }

        public ToAppend(string name, object data)
        {
            Guard.AgainstBadExtension(name, nameof(name));
            Guard.AgainstNull(data, nameof(data));
            Name = name;
            Data = data;
        }
    }
}