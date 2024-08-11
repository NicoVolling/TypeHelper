using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Design;
using TypeHelper.WinApi;
using TypeHelper.Words;

namespace TypeHelper.ClientApp;

public partial class Form1 : Form
{
    private KeyLogger KeyLogger;
    private WordManager WordManager;
    private TextManipulator TextManipulator;
    private WordListManager WordListManager;

    private string Word;

    private Word? FoundWord1;
    private Word? FoundWord2;
    private Word? FoundWord3;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        KeyLogger = new KeyLogger();
        WordManager = new WordManager();
        TextManipulator = new TextManipulator();
        WordListManager = new WordListManager();

        WordListManager.ProcessWords();

        KeyLogger.OnKeyPressed += WordManager.KeyPressed;
        KeyLogger.OnSpecialKeyPressed += WordManager.SpecialKeyPressed;
        WordManager.WordChanged += (s, e) => { try { this.Invoke(() => WordChanged(e)); } catch { } };
        WordManager.SlotSelected += (s, e) => { try { this.Invoke(() => SlotSelected(e)); } catch { } };
        WordManager.EvaluateAutocorrect = () =>
        {
            if (cb_autocorrect.Checked && FoundWord1 != null)
            {
                float distance = (float)Levenshtein.LevenshteinDistance(FoundWord1.Value.Value, Word, 999, true);
                float distancePercentage = distance / (float)FoundWord1.Value.Value.Length;
                if (distancePercentage < ((float)nud_autocorrect.Value / 100))
                {
                    return true;
                }
            }
            return false;
        };

        WordChanged("");

        KeyLogger.Start();
    }

    private void SlotSelected(byte Slot)
    {
        string addText = cb_addspacecomma.Checked ? ", " : "";

        switch (Slot)
        {
            case 1:
                if (FoundWord1 != null)
                {
                    TextManipulator.Manipulate(Word.Length, FoundWord1.Value.Value + addText);
                }
                break;

            case 2:
                if (FoundWord2 != null)
                {
                    TextManipulator.Manipulate(Word.Length, FoundWord2.Value.Value + addText);
                }
                break;

            case 3:
                if (FoundWord3 != null)
                {
                    TextManipulator.Manipulate(Word.Length, FoundWord3.Value.Value + addText);
                }
                break;

            default:
                //Do Nothing
                break;
        }
    }

    private void WordChanged(string Word)
    {
        this.Word = Word;
        this.lbl_typedword.Text = Word;

        FoundWord1 = null;
        FoundWord2 = null;
        FoundWord3 = null;

        if (Word.Length > 2 && Word.Length < 25)
        {
            DateTime dt = DateTime.Now;
            IOrderedEnumerable<KeyValuePair<Word, int>> dictDistances = CalculateLevenshteinDistances(WordListManager.GetWords(), Word, 20).OrderBy(o => o.Value);

            if (dictDistances.Count() > 0)
            {
                FoundWord1 = dictDistances.First().Key;
            }

            if (FoundWord1 != null && dictDistances.Count() > 1 && dictDistances.Skip(1).First().Value < 10)
            {
                FoundWord2 = dictDistances.Skip(1).First().Key;
            }

            if (FoundWord2 != null && dictDistances.Count() > 2)
            {
                FoundWord3 = dictDistances.Skip(2).First().Key;
            }
            DateTime dt2 = DateTime.Now;
            Debug.WriteLine(dt2 - dt);
        }

        ChangePanel(pnl_1, lbl_1_main, lbl_1_category, lbl_1_explanation, FoundWord1);
        ChangePanel(pnl_2, lbl_2_main, lbl_2_category, lbl_2_explanation, FoundWord2);
        ChangePanel(pnl_3, lbl_3_main, lbl_3_category, lbl_3_explanation, FoundWord3);
    }

    private Dictionary<Word, int> CalculateLevenshteinDistances(IEnumerable<Word> words, string target, int maxDistance)
    {
        var result = new ConcurrentDictionary<Word, int>();

        Parallel.ForEach(words, word =>
        {
            int distance = Levenshtein.LevenshteinDistance(word.Value, target, maxDistance);

            if (distance <= maxDistance)
            {
                result[word] = distance;
            }
        });

        return result.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private void ChangePanel(Panel panel, Label lbl_main, Label lbl_category, Label lbl_explanation, Word? Word)
    {
        foreach (Control control in panel.Controls)
        {
            control.Visible = Word != null;
        }

        if (Word is Word word)
        {
            panel.BackColor = GetColorByCategory(word.Category);
            lbl_category.ForeColor = panel.BackColor;
            lbl_main.Text = word.Value;
            lbl_category.Text = word.Category;
            lbl_explanation.Text = word.Explanation;
        }
        else
        {
            panel.BackColor = SystemColors.Control;
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        KeyLogger.Stop();
    }

    private Color GetColorByCategory(string Category)
    {
        return Category switch
        {
            "Medikament" => Color.Green,
            "Fachbegriff" => Color.Red,
            "Baustein" => Color.Purple,
            "Telefonnummer" => Color.Orange,
            _ => Color.Blue
        };
    }

    private void cb_disclaimer_CheckedChanged(object sender, EventArgs e)
    {
        if (cb_disclaimer.Checked)
        {
            cb_disclaimer.ForeColor = Color.Green;
            this.MinimumSize = new Size(this.MinimumSize.Width, this.MinimumSize.Height - lbl_disclaimer_body.Height - lbl_disclaimer_header.Height);
            this.Size = new Size(this.Size.Width, this.Size.Height - lbl_disclaimer_body.Height - lbl_disclaimer_header.Height);
        }
        else
        {
            cb_disclaimer.ForeColor = Color.Red;
            this.Size = new Size(this.Size.Width, this.Size.Height + lbl_disclaimer_body.Height + lbl_disclaimer_header.Height);
            this.MinimumSize = new Size(this.MinimumSize.Width, this.MinimumSize.Height + lbl_disclaimer_body.Height + lbl_disclaimer_header.Height);
        }

        lbl_disclaimer_body.Visible = !cb_disclaimer.Checked;
        lbl_disclaimer_header.Visible = !cb_disclaimer.Checked;
    }

    private void cb_autocorrect_CheckedChanged(object sender, EventArgs e)
    {
        pnl_autocorrect.Visible = cb_autocorrect.Checked;
    }

    private void btn_start_stop_Click(object sender, EventArgs e)
    {
        if (KeyLogger.IsRunning())
        {
            KeyLogger.Stop();
            lbl_startstop.Text = "Tastatur-Aufnahme: AUS";
            lbl_startstop.BackColor = Color.FromArgb(255, 128, 128);
        }
        else
        {
            KeyLogger.Start();
            lbl_startstop.Text = "Tastatur-Aufnahme: AN";
            lbl_startstop.BackColor = Color.FromArgb(128, 255, 128);
        }
    }

    private void cb_onewordmode_CheckedChanged(object sender, EventArgs e)
    {
        WordManager.OneWordMode = cb_onewordmode.Checked;
    }

    private void btn_phone_Click(object sender, EventArgs e)
    {
        ShowAllOfCategory("Telefonnummer");
    }

    private Dictionary<string, Form> CategoryForms = new();

    private void ShowAllOfCategory(string Category, bool ShowExplanation = true)
    {
        if (CategoryForms.ContainsKey(Category) && CategoryForms[Category] is Form _form && !_form.IsDisposed)
        {
            _form.Show();
        }
        else
        {
            Form form = new Form();
            form.AutoScroll = true;
            form.TopMost = true;
            form.Width = 800;
            form.Height = 600;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            form.MaximizeBox = false;
            form.Text = Category;
            form.Padding = new Padding(6);

            IEnumerable<Word> words;
            if (Category == "PLZ")
            {
                words = WordListManager.GetZipCodes().OrderByDescending(o => o.Category).ThenByDescending(o => o.Explanation).ThenByDescending(o => o.Value);
            }
            else
            {
                words = WordListManager.GetWords().Where(o => o.Category == Category).OrderByDescending(o => o.Value);
            }

            foreach (Word word in words)
            {
                Panel pnl = new();
                pnl.Dock = DockStyle.Top;
                pnl.Height = 35;
                pnl.Padding = new Padding(1);
                pnl.BackColor = GetColorByCategory(Category);

                Label lbl = new();
                lbl.AutoSize = false;
                lbl.Dock = ShowExplanation ? DockStyle.Left : DockStyle.Fill;
                lbl.Text = word.Value + (ShowExplanation ? ": " : "");
                lbl.TextAlign = ShowExplanation ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                lbl.Width = 250;
                lbl.BackColor = Color.White;
                lbl.Font = new Font(lbl.Font.FontFamily, 12, FontStyle.Bold);

                Label lbl2 = new();
                lbl2.AutoSize = false;
                lbl2.Dock = DockStyle.Fill;
                lbl2.Text = word.Explanation;
                lbl2.TextAlign = ContentAlignment.MiddleLeft;
                lbl2.BackColor = Color.White;

                if (ShowExplanation) { pnl.Controls.Add(lbl2); }
                pnl.Controls.Add(lbl);

                form.Controls.Add(new Panel() { Dock = DockStyle.Top, Height = 6, });
                form.Controls.Add(pnl);
            }

            CategoryForms[Category] = form;

            form.Show();
        }
    }

    private void btn_medikamente_Click(object sender, EventArgs e)
    {
        ShowAllOfCategory("Medikament");
    }

    private void btn_fachbegriffe_Click(object sender, EventArgs e)
    {
        ShowAllOfCategory("Fachbegriff");
    }

    private void btn_plz_Click(object sender, EventArgs e)
    {
        ShowAllOfCategory("PLZ");
    }

    private void btn_bausteine_Click(object sender, EventArgs e)
    {
        ShowAllOfCategory("Baustein", false);
    }
}