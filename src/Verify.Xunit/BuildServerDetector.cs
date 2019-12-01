using System;

static class BuildServerDetector
{
    static BuildServerDetector()
    {
        // Appveyor: https://www.appveyor.com/docs/environment-variables/
        // Travis: https://docs.travis-ci.com/user/environment-variables/#default-environment-variables
        if (Environment.GetEnvironmentVariable("CI") == "true")
        {
            Detected = true;
            return;
        }
        // Jenkins: https://wiki.jenkins.io/display/JENKINS/Building+a+software+project#Buildingasoftwareproject-belowJenkinsSetEnvironmentVariables
        if (Environment.GetEnvironmentVariable("JENKINS_URL") != null)
        {
            Detected = true;
            return;
        }
        // AzureDevops: https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#agent-variables
        if (Environment.GetEnvironmentVariable("Agent.Id") != null)
        {
            Detected = true;
            return;
        }
    }

    public static bool Detected { get; }
}