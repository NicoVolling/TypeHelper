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

    public bool OneWordMode { get; set; }

    public void KeyPressed(char Char)
    {
        if (!changeWord)
        {
            return;
        }

        HandleCharacterInput(Char);

        WordChanged?.Invoke(this, string.Concat(currentWord));

        if (EvaluateAutocorrect.Invoke())
        {
            StartTextManipulation(1);
        }
    }

    public void SpecialKeyPressed(Keys Key)
    {
        if (HandleNavigationKey(Key)) { }
        else if (HandleSpecialCharKey(Key)) { }
        else if (IsBackKey(Key))
        {
            HandleBackKey();
        }
        else if (IsManipulationKey(Key) is byte slot && slot != 0)
        {
            StartTextManipulation(slot);
        }

        WordChanged?.Invoke(this, string.Concat(currentWord));

        if (EvaluateAutocorrect.Invoke())
        {
            StartTextManipulation(1);
        }
    }

    private bool HandleSpecialCharKey(Keys Key)
    {
        if (Key == Keys.Space)
        {
            HandleCharacterInput(' ');
            return true;
        }
        else if (Key == Keys.OemMinus)
        {
            HandleCharacterInput('-');
            return true;
        }
        return false;
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
            Keys.F3 => 1,
            Keys.F4 => 2,
            Keys.F5 => 3,
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

    private void HandleCharacterInput(char Char)
    {
        InsertCharacter(Char);
    }

    private bool HandleNavigationKey(Keys key)
    {
        if (key == Keys.Left)
        {
            MoveCursorLeft();
            return true;
        }
        else if (key == Keys.Right)
        {
            MoveCursorRight();
            return true;
        }
        else if (IsResetKey(key))
        {
            ResetCurrentWord();
            return true;
        }
        return false;
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
            Keys.OemPeriod, Keys.Enter, Keys.Escape, Keys.LButton, Keys.F2
        };
        if (OneWordMode) { resetKeys.Add(Keys.Space); }
        return resetKeys.Contains(key);
    }

    private void ResetCurrentWord()
    {
        currentWord.Clear();
        pos = 0;
        WordChanged?.Invoke(this, string.Concat(currentWord));
    }
}