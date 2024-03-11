namespace VerifyTests;

public delegate bool AutoVerify(string verifiedFile);

public delegate bool GlobalAutoVerify(string typeName, string methodName, string verifiedFile);