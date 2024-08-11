using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypeHelper.WinApi;

public class KeyLogger
{
    private static readonly List<Key> Keys = new List<Key>
    {
        new Key { Digit = 1, Value = "[Left Click]" },
        new Key { Digit = 8, Value = "[Back]" },
        new Key { Digit = 9, Value = "[TAB]" },
        new Key { Digit = 13, Value = "[Enter]" },
        new Key { Digit = 16, Value = "[Shift]" },
        new Key { Digit = 19, Value = "[Pause]" },
        new Key { Digit = 20, Value = "[Caps Lock]" },
        new Key { Digit = 27, Value = "[Esc]" },
        new Key { Digit = 32, Value = "[Space]" },
        new Key { Digit = 33, Value = "[Page Up]" },
        new Key { Digit = 34, Value = "[Page Down]" },
        new Key { Digit = 35, Value = "[End]" },
        new Key { Digit = 36, Value = "[Home]" },
        new Key { Digit = 37, Value = "[Left]" },
        new Key { Digit = 38, Value = "[Up]" },
        new Key { Digit = 39, Value = "[Right]" },
        new Key { Digit = 40, Value = "[Down]" },
        new Key { Digit = 44, Value = "[Print Screen]" },
        new Key { Digit = 45, Value = "[Insert]" },
        new Key { Digit = 46, Value = "[Delete]" },
        new Key { Digit = 48, Value = "0" },
        new Key { Digit = 49, Value = "1" },
        new Key { Digit = 50, Value = "2" },
        new Key { Digit = 51, Value = "3" },
        new Key { Digit = 52, Value = "4" },
        new Key { Digit = 53, Value = "5" },
        new Key { Digit = 54, Value = "6" },
        new Key { Digit = 55, Value = "7" },
        new Key { Digit = 56, Value = "8" },
        new Key { Digit = 57, Value = "9" },
        new Key { Digit = 65, Value = "a" },
        new Key { Digit = 66, Value = "b" },
        new Key { Digit = 67, Value = "c" },
        new Key { Digit = 68, Value = "d" },
        new Key { Digit = 69, Value = "e" },
        new Key { Digit = 70, Value = "f" },
        new Key { Digit = 71, Value = "g" },
        new Key { Digit = 72, Value = "h" },
        new Key { Digit = 73, Value = "i" },
        new Key { Digit = 74, Value = "j" },
        new Key { Digit = 75, Value = "k" },
        new Key { Digit = 76, Value = "l" },
        new Key { Digit = 77, Value = "m" },
        new Key { Digit = 78, Value = "n" },
        new Key { Digit = 79, Value = "o" },
        new Key { Digit = 80, Value = "p" },
        new Key { Digit = 81, Value = "q" },
        new Key { Digit = 82, Value = "r" },
        new Key { Digit = 83, Value = "s" },
        new Key { Digit = 84, Value = "t" },
        new Key { Digit = 85, Value = "u" },
        new Key { Digit = 86, Value = "v" },
        new Key { Digit = 87, Value = "w" },
        new Key { Digit = 88, Value = "x" },
        new Key { Digit = 89, Value = "y" },
        new Key { Digit = 90, Value = "z" },
        new Key { Digit = 91, Value = "[Windows]" },
        new Key { Digit = 92, Value = "[Windows]" },
        new Key { Digit = 93, Value = "[List]" },
        new Key { Digit = 96, Value = "0" },
        new Key { Digit = 97, Value = "1" },
        new Key { Digit = 98, Value = "2" },
        new Key { Digit = 99, Value = "3" },
        new Key { Digit = 100, Value = "4" },
        new Key { Digit = 101, Value = "5" },
        new Key { Digit = 102, Value = "6" },
        new Key { Digit = 103, Value = "7" },
        new Key { Digit = 104, Value = "8" },
        new Key { Digit = 105, Value = "9" },
        new Key { Digit = 106, Value = "*" },
        new Key { Digit = 107, Value = "+" },
        new Key { Digit = 109, Value = "-" },
        new Key { Digit = 110, Value = "," },
        new Key { Digit = 111, Value = "/" },
        new Key { Digit = 112, Value = "[F1]" },
        new Key { Digit = 113, Value = "[F2]" },
        new Key { Digit = 114, Value = "[F3]" },
        new Key { Digit = 115, Value = "[F4]" },
        new Key { Digit = 116, Value = "[F5]" },
        new Key { Digit = 117, Value = "[F6]" },
        new Key { Digit = 118, Value = "[F7]" },
        new Key { Digit = 119, Value = "[F8]" },
        new Key { Digit = 120, Value = "[F9]" },
        new Key { Digit = 121, Value = "[F10]" },
        new Key { Digit = 122, Value = "[F11]" },
        new Key { Digit = 123, Value = "[F12]" },
        new Key { Digit = 144, Value = "[Num Lock]" },
        new Key { Digit = 145, Value = "[Scroll Lock]" },
        new Key { Digit = 162, Value = "[Ctrl]" },
        new Key { Digit = 163, Value = "[Ctrl]" },
        new Key { Digit = 164, Value = "[Alt]" },
        new Key { Digit = 165, Value = "[Alt]" },
        new Key { Digit = 188, Value = "," },
        new Key { Digit = 190, Value = "." },
        new Key { Digit = 222, Value = "ä" },
        new Key { Digit = 226, Value = "\\" },
        new Key { Digit = 192, Value = "ö" },
        new Key { Digit = 186, Value = "ü" },
        new Key { Digit = 219, Value = "ß" }
    };

    public event EventHandler<int>? KeyPressed;

    private bool running;

    public void Start()
    {
        running = true;
        new Thread(() =>
        {
            Key? letter = null;
            while (running)
            {
                for (int i = 0; i < 255; i++)
                {
                    int key = WinApiHelper.GetAsyncKeyState(i);
                    if (key == 1 || key == -32767)
                    {
                        letter = Keys.FirstOrDefault(x => x.Digit == i);

                        string l = string.Empty;
                        int ij = 0;

                        if (letter != null)
                        {
                            Debug.WriteLine(letter.Digit);

                            l = letter.Value;
                            ij = letter.Digit;
                        }
                        else
                        {
                            Debug.WriteLine(i.ToString());

                            l = i.ToString();
                            ij = i;
                        }

                        KeyPressed?.Invoke(this, ij);

                        break;
                    }
                }
            }
        }).Start();
    }

    public void Stop()
    {
        running = false;
    }
}