using System;
using System.Diagnostics;

namespace DiffEngine
{
    public static class ProcessExtensions
    {
        public static void StartWithCatch(this Process process)
        {
            try
            {
                //TODO: handle exe not found
                process.Start();
            }
            catch (Exception exception)
            {
                var message = $@"Failed to launch diff tool.
{process.StartInfo.FileName} {process.StartInfo.Arguments}";
                throw new Exception(message, exception);
            }
        }
    }
}