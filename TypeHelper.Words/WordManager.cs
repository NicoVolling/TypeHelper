using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

namespace TypeHelper.Words;

public class WordManager
{
    private List<Char> currentWord = new();
    private int pos = 0;

    private bool changeWord = true;

    public event EventHandler<string>? WordChanged;

    public event EventHandler<byte>? SlotSelected;

    public Func<bool> EvaluateAutocorrect = () => false;

    public void KeyPressed(Int32 KeyCode)
    {
        if (!changeWord)
        {
            return;
        }

        Keys key = (Keys)KeyCode;

        if (IsBackKey(key))
        {
            HandleBackKey();
        }
        else if (IsManipulationKey(key) is byte slot && slot != 0)
        {
            StartTextManipulation(slot);
        }
        else
        {
            HandleCharacterInput(KeyCode);
        }

        WordChanged?.Invoke(this, string.Concat(currentWord));

        if (EvaluateAutocorrect.Invoke())
        {
            StartTextManipulation(1);
        }
    }

    private bool IsBackKey(Keys key)
    {
        return key == Keys.Back;
    }

    private void HandleBackKey()
    {
        if (currentWord.Any() && pos < currentWord.Count)
        {
            currentWord.RemoveAt(currentWord.Count - 1 - pos);
        }
    }

    private void StartTextManipulation(byte Slot)
    {
        new Thread(() =>
        {
            SelectSlot(Slot);
        }).Start();
    }

    public void SelectSlot(byte Slot)
    {
        changeWord = false;
        Thread.Sleep(100);
        SlotSelected?.Invoke(this, Slot);
        ResetCurrentWord();
        Thread.Sleep(100);
        changeWord = true;
    }

    private byte IsManipulationKey(Keys key)
    {
        return key switch
        {
            Keys.F6 => 1,
            Keys.F7 => 2,
            Keys.F8 => 3,
            _ => 0
        };
    }

    private void InsertCharacter(char chr)
    {
        if (currentWord.Count > 0 || chr != ' ')
        {
            currentWord.Add(chr);
        }
    }

    private void HandleCharacterInput(int keyCode)
    {
        char chr = ConvertKeyCodeToChar(keyCode);

        if (char.IsLetterOrDigit(chr) || chr == ' ' || chr == '-')
        {
            InsertCharacter(chr);
        }
        else
        {
            HandleSpecialKeys((Keys)keyCode);
        }
    }

    private char ConvertKeyCodeToChar(int keyCode)
    {
        return keyCode switch
        {
            222 => 'Ä',
            186 => 'Ü',
            192 => 'Ö',
            219 => 'ß',
            96 => '0',
            97 => '1',
            98 => '2',
            99 => '3',
            100 => '4',
            101 => '5',
            102 => '6',
            103 => '7',
            104 => '8',
            105 => '9',
            189 => '-',
            _ => (char)keyCode
        };
    }

    private void HandleSpecialKeys(Keys key)
    {
        if (key == Keys.Left)
        {
            MoveCursorLeft();
        }
        else if (key == Keys.Right)
        {
            MoveCursorRight();
        }
        else if (IsResetKey(key))
        {
            ResetCurrentWord();
        }
    }

    private void MoveCursorLeft()
    {
        if (pos < currentWord.Count)
        {
            pos++;
        }
    }

    private void MoveCursorRight()
    {
        if (pos > 0)
        {
            pos--;
        }
    }

    private bool IsResetKey(Keys key)
    {
        var resetKeys = new List<Keys>
    {
        Keys.Up, Keys.Down, /*Keys.Space,*/ Keys.Insert,
        Keys.Delete, Keys.Tab, Keys.Oemcomma,
        Keys.OemPeriod, Keys.Enter
    };
        return resetKeys.Contains(key);
    }

    private void ResetCurrentWord()
    {
        currentWord.Clear();
        pos = 0;
        WordChanged?.Invoke(this, string.Concat(currentWord));
    }
}