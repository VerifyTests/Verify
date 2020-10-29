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
            this.settings = settings;
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

        public SettingsTask UseExtension(string extension)
        {
            BuildSettings().UseExtension(extension);
            return this;
        }

        VerifySettings BuildSettings()
        {
            return settings ??= new VerifySettings();
        }

        public SettingsTask BasedOn(VerifySettings settings)
        {
            Guard.AgainstNull(settings, nameof(settings));
            if (this.settings != null)
            {
                throw new Exception("BasedOn must be the first extension method called");
            }
            this.settings = new VerifySettings(settings);
            return this;
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