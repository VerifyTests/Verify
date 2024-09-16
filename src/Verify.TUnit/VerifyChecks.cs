﻿#pragma warning disable InnerVerifyChecks
namespace VerifyTUnit;

public static class VerifyChecks
{
    public static Task Run()
    {
        var details = TestContext.Current!.TestDetails;
        var type = details.ClassType;
        VerifierSettings.AssignTargetAssembly(type.Assembly);
        return InnerVerifyChecks.Run(type.Assembly);
    }
}