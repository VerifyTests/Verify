namespace VerifyTests;

public delegate TMember ConvertTargetMember<in TTarget, TMember>(TTarget target, TMember memberValue);

public delegate object? ConvertTargetMember(object target, object? memberValue);

public delegate TMember ConvertMember<TMember>(TMember memberValue);

public delegate object? ConvertMember(object? memberValue);