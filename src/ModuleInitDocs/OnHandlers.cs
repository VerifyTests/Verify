public class OnHandlers
{
    #region OnStaticHandlers

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifierSettings.OnVerify(
                before: () => Debug.WriteLine("before"),
                after: () => Debug.WriteLine("after"));
            VerifierSettings.OnFirstVerify(
                (receivedFile, receivedText) =>
                {
                    Debug.WriteLine(receivedFile);
                    Debug.WriteLine(receivedText);
                    return Task.CompletedTask;
                });
            VerifierSettings.OnVerifyMismatch(
                (filePair, message) =>
                {
                    Debug.WriteLine(filePair.ReceivedPath);
                    Debug.WriteLine(filePair.VerifiedPath);
                    Debug.WriteLine(message);
                    return Task.CompletedTask;
                });
        }
    }

    #endregion
}