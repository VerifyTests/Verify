using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VerifyTests
{
    public class SettingsTask
    {
        VerifySettings? settings;
        Func<VerifySettings, Task> buildTask;
        Task? task;

        public SettingsTask(VerifySettings? settings, Func<VerifySettings, Task> buildTask)
        {
            if (settings != null)
            {
                this.settings = new VerifySettings(settings);
            }
            this.buildTask = async verifySettings => { await buildTask(verifySettings); };
        }

        public SettingsTask AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            BuildSettings().AddExtraSettings(action);
            return this;
        }

        public SettingsTask AddScrubber(Action<StringBuilder> scrubber)
        {
            BuildSettings().AddScrubber(scrubber);
            return this;
        }
        public SettingsTask OnFirstVerify(FirstVerify firstVerify)
        {
            BuildSettings().OnFirstVerify(firstVerify);
            return this;
        }

        public SettingsTask OnVerifyMismatch(VerifyMismatch verifyMismatch)
        {
            BuildSettings().OnVerifyMismatch(verifyMismatch);
            return this;
        }

        public SettingsTask DisableClipboard()
        {
            BuildSettings().DisableClipboard();
            return this;
        }

        public SettingsTask EnableClipboard()
        {
            BuildSettings().EnableClipboard();
            return this;
        }

        public SettingsTask UseComparer(Compare compare)
        {
            BuildSettings().UseComparer(compare);
            return this;
        }

        public SettingsTask DisableDiff()
        {
            BuildSettings().DisableDiff();
            return this;
        }

        public SettingsTask UniqueForAssemblyConfiguration()
        {
            BuildSettings().UniqueForAssemblyConfiguration();
            return this;
        }

        public SettingsTask UniqueForRuntime()
        {
            BuildSettings().UniqueForRuntime();
            return this;
        }

        public SettingsTask UniqueForRuntimeAndVersion()
        {
            BuildSettings().UniqueForRuntimeAndVersion();
            return this;
        }

        public SettingsTask ScrubMachineName()
        {
            BuildSettings().ScrubMachineName();
            return this;
        }

        public SettingsTask DisableNewLineEscaping()
        {
            BuildSettings().DisableNewLineEscaping();
            return this;
        }

        public SettingsTask ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            BuildSettings().ScrubLinesContaining(comparison, stringToMatch);
            return this;
        }

        public SettingsTask ScrubLines(Func<string, bool> removeLine)
        {
            BuildSettings().ScrubLines(removeLine);
            return this;
        }

        public SettingsTask ScrubLinesWithReplace(Func<string, string> replaceLine)
        {
            BuildSettings().ScrubLinesWithReplace(replaceLine);
            return this;
        }

        public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
        {
            BuildSettings().ScrubLinesContaining(stringToMatch);
            return this;
        }

        public SettingsTask ModifySerialization(Action<SerializationSettings> action)
        {
            BuildSettings().ModifySerialization(action);
            return this;
        }

        public SettingsTask AutoVerify()
        {
            BuildSettings().AutoVerify();
            return this;
        }

        public SettingsTask UseExtension(string extension)
        {
            BuildSettings().UseExtension(extension);
            return this;
        }

        VerifySettings BuildSettings()
        {
            return settings ??= new VerifySettings();
        }

        Task ToTask()
        {
            return task ??= buildTask(BuildSettings());
        }

        public TaskAwaiter GetAwaiter()
        {
            return ToTask().GetAwaiter();
        }

        public static implicit operator Task(SettingsTask settingsTask)
        {
            return settingsTask.ToTask();
        }
    }
}