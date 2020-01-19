using System;

static class BuildServerDetector
{
    static BuildServerDetector()
    {
        // Appveyor
        // https://www.appveyor.com/docs/environment-variables/
        // Travis
        // https://docs.travis-ci.com/user/environment-variables/#default-environment-variables
        if (string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase))
        {
            Detected = true;
            return;
        }

        // Jenkins
        // https://wiki.jenkins.io/display/JENKINS/Building+a+software+project#Buildingasoftwareproject-belowJenkinsSetEnvironmentVariables
        if (Environment.GetEnvironmentVariable("JENKINS_URL") != null)
        {
            Detected = true;
            return;
        }

        // GitHub Action
        // https://help.github.com/en/actions/automating-your-workflow-with-github-actions/using-environment-variables#default-environment-variables
        if (Environment.GetEnvironmentVariable("GITHUB_ACTION") != null)
        {
            Detected = true;
            return;
        }

        // AzureDevops
        // https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#agent-variables
        // Variable name is 'Agent.Id' but must be referenced with an underscore
        // https://docs.microsoft.com/en-us/azure/devops/pipelines/release/variables?view=azure-devops&tabs=powershell#using-default-variables
        if (Environment.GetEnvironmentVariable("Agent_Id") != null)
        {
            Detected = true;
            return;
        }

        // TeamCity
        // https://www.jetbrains.com/help/teamcity/predefined-build-parameters.html#PredefinedBuildParameters-ServerBuildProperties
        if (Environment.GetEnvironmentVariable("teamcity") != null)
        {
            Detected = true;
            return;
        }
    }

    public static bool Detected { get; set; }
}