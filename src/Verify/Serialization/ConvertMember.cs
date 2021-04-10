namespace VerifyTests
{
    public delegate TMember ConvertMember<in TTarget, TMember>(TTarget target, TMember memberValue);

    public delegate object? ConvertMember(object? target, object? memberValue);
}