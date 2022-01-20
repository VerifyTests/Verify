namespace VerifyTests;

//TODO add convert member with no target
public delegate TMember ConvertMember<in TTarget, TMember>(TTarget target, TMember memberValue);

//TODO target should not be nullable
public delegate object? ConvertMember(object? target, object? memberValue);