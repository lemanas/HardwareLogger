using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace HardwareLogger
{
    static class RuntimeConfig
    {
        [DllImport("libc")]
        public static extern uint getuid();

        public static void RequireAdministrator()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                    {
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                        {
                            throw new InvalidOperationException("Application must be run as administrator");
                        }
                    }
                }
                else if (getuid() != 0)
                {
                    throw new InvalidOperationException("Application must be run as root");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to determine administrator or root status", ex);
            }
        }
    }
}
