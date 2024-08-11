using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypeHelper.WinApi;

public class KeyLogger
{
    public delegate void KeyPressedEventHandler(char keyChar);

    public event KeyPressedEventHandler OnKeyPressed;

    public delegate void SpecialKeyPressedEventHandler(Keys key);

    public event SpecialKeyPressedEventHandler OnSpecialKeyPressed;

    private IntPtr hookId = IntPtr.Zero;
    private LowLevelKeyboardProc proc;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private bool running;

    public KeyLogger()
    {
        proc = HookCallback;
    }

    public bool IsRunning()
    {
        return running;
    }

    public void Start()
    {
        hookId = SetHook(proc);
        running = true;
    }

    public void Stop()
    {
        UnhookWindowsHookEx(hookId);
        running = false;
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            // Handle regular keys
            if (key >= Keys.A && key <= Keys.Z)
            {
                char keyChar = (char)(key + (IsShiftPressed() ? 0 : 32));
                OnKeyPressed?.Invoke(keyChar);
            }
            else if (key >= Keys.D0 && key <= Keys.D9)
            {
                char keyChar = IsShiftPressed() ? GetShiftedNumber(key) : (char)key;
                OnKeyPressed?.Invoke(keyChar);
            }
            else if (key == Keys.Oem1 || key == Keys.Oem3 || key == Keys.Oem7)
            {
                char keyChar = key == Keys.Oem1 ? 'ü' : key == Keys.Oem3 ? 'ö' : key == Keys.Oem7 ? 'ä' : ' ';
                if (IsShiftPressed())
                {
                    keyChar = char.ToUpper(keyChar);
                }
                OnKeyPressed?.Invoke(keyChar);
            }
            else
            {
                OnSpecialKeyPressed?.Invoke(key);
            }
        }
        return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    private bool IsShiftPressed()
    {
        return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
    }

    private char GetShiftedNumber(Keys key)
    {
        switch (key)
        {
            case Keys.D1: return '!';
            case Keys.D2: return '\"';
            case Keys.D3: return '§';
            case Keys.D4: return '$';
            case Keys.D5: return '%';
            case Keys.D6: return '&';
            case Keys.D7: return '/';
            case Keys.D8: return '(';
            case Keys.D9: return ')';
            case Keys.D0: return '=';
            default: return (char)key;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
}