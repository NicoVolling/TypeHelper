using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelper.WinApi;

public class TextManipulator
{
    private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const byte VK_BACK = 0x08;

    public void Manipulate(int deleteLastKeys, string addString)
    {
        // Delete the last `deleteLastKeys` characters
        for (int i = 0; i < deleteLastKeys; i++)
        {
            PressKey(VK_BACK);
            PressKey(VK_BACK, true);
        }

        // Add the new string
        foreach (char c in addString)
        {
            SendKey(c);
        }
    }

    private void PressKey(byte key, bool keyUp = false)
    {
        uint flag = keyUp ? KEYEVENTF_KEYUP : 0;
        WinApiHelper.keybd_event(key, 0, flag, UIntPtr.Zero);
    }

    private void SendKey(char c)
    {
        short vkey = WinApiHelper.VkKeyScan(c);

        if (vkey == -1)
        {
            throw new ArgumentException($"Zeichen '{c}' kann nicht über die Tastatur gesendet werden.");
        }

        byte key = (byte)(vkey & 0xff);
        byte shiftState = (byte)((vkey >> 8) & 0xff);

        if ((shiftState & 1) != 0)
        {
            PressKey(0x10); // Shift key down
        }

        if ((shiftState & 2) != 0)
        {
            PressKey(0x11); // Ctrl key down
        }

        if ((shiftState & 4) != 0)
        {
            PressKey(0x12); // Alt key down
        }

        PressKey(key);  // Key down
        PressKey(key, true);  // Key up

        if ((shiftState & 4) != 0)
        {
            PressKey(0x12, true); // Alt key up
        }

        if ((shiftState & 2) != 0)
        {
            PressKey(0x11, true); // Ctrl key up
        }

        if ((shiftState & 1) != 0)
        {
            PressKey(0x10, true); // Shift key up
        }
    }
}