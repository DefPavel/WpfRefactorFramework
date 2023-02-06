using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WpfRefactorFramework.Helpers
{
    public sealed class KeyHelper
    {
         private const int WmKeydown = 0x100;
        private const int WmKeyup = 0x101;
        private const int WmSyskeydown = 0x104;
        private const int WmSyskeyup = 0x105;
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct Kbdllhookstruct
        {
            public readonly Keys key;
            private readonly int vkCode;
            private readonly int scanCode;
            private readonly int flags;
            private readonly int time;
            private readonly IntPtr extra;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, int wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, int wParam, IntPtr lParam);
        private LowLevelKeyboardProc _keyboardProcess;

        private static IntPtr _ptrHook;

        public event KeyEventHandler KeyUp;
        public event KeyEventHandler KeyDown;

        public KeyHelper()
        {
            Hook();
        }
        ~KeyHelper()
        {
            Unhook();
        }

        private void Hook()
        {
            var objCurrentModule = Process.GetCurrentProcess().MainModule;
            _keyboardProcess = CaptureKey;
            _ptrHook = SetWindowsHookEx(13, _keyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
        }

        private void Unhook()
        {
            _ = UnhookWindowsHookEx(_ptrHook);
        }

        private IntPtr CaptureKey(int nCode, int wp, IntPtr lp)
        {
            if (nCode < 0) return CallNextHookEx(_ptrHook, nCode, wp, lp);
            var keyInfo = (Kbdllhookstruct)Marshal.PtrToStructure(lp, typeof(Kbdllhookstruct));
            var eventArgs = new KeyEventArgs(keyInfo.key);
            if ((wp == WmKeydown || wp == WmSyskeydown) && KeyDown != null)
            {
                KeyDown(this, eventArgs);
            }
            else if ((wp == WmKeyup || wp == WmSyskeyup) && (KeyUp != null))
            {
                KeyUp(this, eventArgs);
            }
            if (eventArgs.Handled)
            {
                return (IntPtr)1;
            }
            return CallNextHookEx(_ptrHook, nCode, wp, lp);
        }
    }
}