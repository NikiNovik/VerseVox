using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VerseVox.Scripts
{
    public static class ProcessListener
    {
        public static string GetActiveProcessName()
        {
            // Получаем хэндл активного окна
            GetWindowThreadProcessId(GetForegroundWindow(), out uint processId);

            // Получаем процесс по его ID
            Process proc = Process.GetProcessById((int)processId);

            // Возвращаем название процесса
            return "Сейчас в: " + proc.ProcessName;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    }
}
