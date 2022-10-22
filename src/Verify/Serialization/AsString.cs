namespace VerifyTests;

public delegate AsStringResult AsString<in T>(T target, VerifySettings context)
    where T: notnull;