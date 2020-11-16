readonly struct EqualityResult
{
    public Equality Equality { get; }
    public string? Message { get; }

    public EqualityResult(in Equality equality, in string? message = null)
    {
        Equality = equality;
        Message = message;
    }

    public static implicit operator EqualityResult(Equality equality) => new(equality);
}