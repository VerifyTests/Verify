[SupportedOSPlatform("windows")]
static class RestartManager
{
    const int rmRebootReasonNone = 0;
    const int cchRmMaxAppName = 255;
    const int cchRmMaxSvcName = 63;
    const int errorMoreData = 234;

    [StructLayout(LayoutKind.Sequential)]
    struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cchRmMaxAppName + 1)]
        public string strAppName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cchRmMaxSvcName + 1)]
        public string strServiceShortName;

        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;

        [MarshalAs(UnmanagedType.Bool)]
        public bool bRestartable;
    }

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmRegisterResources(
        uint pSessionHandle,
        uint nFiles,
        string[] rgsFilenames,
        uint nApplications,
        [In] RM_UNIQUE_PROCESS[]? rgApplications,
        uint nServices,
        string[]? rgsServiceNames);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
    static extern int RmStartSession(
        out uint pSessionHandle,
        int dwSessionFlags,
        string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll")]
    static extern int RmGetList(
        uint dwSessionHandle,
        out uint pnProcInfoNeeded,
        ref uint pnProcInfo,
        [In, Out] RM_PROCESS_INFO[]? rgAffectedApps,
        ref uint lpdwRebootReasons);

    public static List<Process> GetProcessesLockingFile(string path)
    {
        var processes = new List<Process>();
        var key = Guid.NewGuid().ToString();

        var startResult = RmStartSession(out var handle, 0, key);
        if (startResult != 0)
        {
            return processes;
        }

        try
        {
            string[] resources = [path];
            var registerResult = RmRegisterResources(handle, (uint) resources.Length, resources, 0, null, 0, null);
            if (registerResult != 0)
            {
                return processes;
            }

            uint pnProcInfo = 0;
            var rebootReasons = (uint) rmRebootReasonNone;

            var listResult = RmGetList(handle, out var pnProcInfoNeeded, ref pnProcInfo, null, ref rebootReasons);
            if (listResult == 0)
            {
                return processes;
            }

            if (listResult != errorMoreData)
            {
                return processes;
            }

            var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
            pnProcInfo = pnProcInfoNeeded;
            listResult = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref rebootReasons);
            if (listResult != 0)
            {
                return processes;
            }

            for (var i = 0; i < pnProcInfo; i++)
            {
                try
                {
                    var process = Process.GetProcessById(processInfo[i].Process.dwProcessId);
                    processes.Add(process);
                }
                catch (ArgumentException)
                {
                }
            }
        }
        finally
        {
            RmEndSession(handle);
        }

        return processes;
    }
}
