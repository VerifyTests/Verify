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
            CurrentSettings.AddExtraSettings(action);
            return this;
        }

        public SettingsTask AddScrubber(Action<StringBuilder> scrubber)
        {
            CurrentSettings.AddScrubber(scrubber);
            return this;
        }
        public SettingsTask OnFirstVerify(FirstVerify firstVerify)
        {
            CurrentSettings.OnFirstVerify(firstVerify);
            return this;
        }

        public SettingsTask OnVerifyMismatch(VerifyMismatch verifyMismatch)
        {
            CurrentSettings.OnVerifyMismatch(verifyMismatch);
            return this;
        }

        public SettingsTask DisableClipboard()
        {
            CurrentSettings.DisableClipboard();
            return this;
        }

        public SettingsTask EnableClipboard()
        {
            CurrentSettings.EnableClipboard();
            return this;
        }

        public SettingsTask UseComparer(Compare compare)
        {
            CurrentSettings.UseComparer(compare);
            return this;
        }

        public SettingsTask DisableDiff()
        {
            CurrentSettings.DisableDiff();
            return this;
        }

        public SettingsTask UniqueForAssemblyConfiguration()
        {
            CurrentSettings.UniqueForAssemblyConfiguration();
            return this;
        }

        public SettingsTask UniqueForRuntime()
        {
            CurrentSettings.UniqueForRuntime();
            return this;
        }

        public SettingsTask UniqueForRuntimeAndVersion()
        {
            CurrentSettings.UniqueForRuntimeAndVersion();
            return this;
        }

        public SettingsTask ScrubMachineName()
        {
            CurrentSettings.ScrubMachineName();
            return this;
        }

        public SettingsTask DisableNewLineEscaping()
        {
            CurrentSettings.DisableNewLineEscaping();
            return this;
        }

        public SettingsTask ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
        {
            CurrentSettings.ScrubLinesContaining(comparison, stringToMatch);
            return this;
        }

        public SettingsTask ScrubLines(Func<string, bool> removeLine)
        {
            CurrentSettings.ScrubLines(removeLine);
            return this;
        }

        public SettingsTask ScrubLinesWithReplace(Func<string, string> replaceLine)
        {
            CurrentSettings.ScrubLinesWithReplace(replaceLine);
            return this;
        }

        public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
        {
            CurrentSettings.ScrubLinesContaining(stringToMatch);
            return this;
        }

        public SettingsTask ModifySerialization(Action<SerializationSettings> action)
        {
            CurrentSettings.ModifySerialization(action);
            return this;
        }

        public SettingsTask AutoVerify()
        {
            CurrentSettings.AutoVerify();
            return this;
        }

        public SettingsTask UseExtension(string extension)
        {
            CurrentSettings.UseExtension(extension);
            return this;
        }

        public VerifySettings CurrentSettings
        {
            get => settings ??= new VerifySettings();
        }

        Task ToTask()
        {
            return task ??= buildTask(CurrentSettings);
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