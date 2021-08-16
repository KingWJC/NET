using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
#if DNX46
using System.Security.Permissions;
#endif

/* 
 * 平台调用非托管的方法 
*/
namespace code
{
    public class PInvoke
    {
        public static void Test()
        {
            try
            {
                FileUtility.CreateHardLink("./Program.cs", "./newProgram.cs");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /* 公共类: 提供本机方法的功能 */
    public static class FileUtility
    {
#if DNX46
        [FileIOPermission(SecurityAction.LinkDemand,Unrestricted=true)]
#endif
        public static void CreateHardLink(string oldFileName, string newFileName)
        {
            NativeMethods.CreateHardLink(oldFileName, newFileName);
        }
    }

    /* 内部类: 包装平台调用的方法 */
    [SecurityCritical]
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true,
        EntryPoint = "CreateHardLink", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateHardLink(
            [In, MarshalAs(UnmanagedType.LPStr)] string newFileName,
            [In, MarshalAs(UnmanagedType.LPStr)] string excitingFileName,
            IntPtr securityAttributes
        );

        internal static void CreateHardLink(string oldFileName, string newFileName)
        {
            if (!CreateHardLink(newFileName, oldFileName, IntPtr.Zero))
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new IndexOutOfRangeException($"Error code {errorCode}");
            }
        }
    }
}