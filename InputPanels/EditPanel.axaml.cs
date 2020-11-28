using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace chemdb_contribute_tool.InputPanels
{
    public class EditPanel : Window
    {
        public EditPanel()
        {
            this.InitializeComponent();
    #if DEBUG
            this.AttachDevTools();
#endif
            Width = 300;
            Height = 240;
        }

        public void Change(string f = "", string n = "", string c = "", string s = "")
        {
            this.FindControl<TextBox>("formula").Text = f;
            this.FindControl<TextBox>("formula").IsEnabled = false;
            this.FindControl<TextBox>("name").Text = n;
            this.FindControl<TextBox>("cas").Text = c;
            this.FindControl<TextBox>("smiles").Text = s;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public bool exitNormally = false;
        public string F, N, C, S;

        private void OKClick(object s, RoutedEventArgs e)
        {
            F = this.FindControl<TextBox>("formula").Text.Replace("·", ".");
            if (new MolCalculator(F, AtomDB.mass).Calculate() == -1)
            {
                this.FindControl<Button>("OKbutton").Content = "非法化学式!";
                return;
            }
            if (this.FindControl<TextBox>("formula").IsEnabled && Restore.db.position.ContainsKey(F))
            {
                this.FindControl<Button>("OKbutton").Content = "重复化学式!";
                return;
            }
            exitNormally = true;
            N = this.FindControl<TextBox>("name").Text.Replace("\0", "");
            string nC = this.FindControl<TextBox>("cas").Text;
            C = "";
            if(nC != null) for (int i = 0; i < nC.Length; ++i) if (nC[i] >= 32 && nC[i] <= 126) C += nC[i];
            string nS = this.FindControl<TextBox>("smiles").Text;
            S = "";
            if (nS != null) for (int i = 0; i < nS.Length; ++i) if (nS[i] >= 32 && nS[i] <= 126) S += nS[i];
            Close();
        }

        private void TextChanged(object s, KeyEventArgs e)
        {
            TextBox text = this.FindControl<TextBox>("formula");
            string name = text.Text;
            if (name == null) return;
            string finame = "";
            for (int i = 0; i < name.Length; ++i)
            {
                if (name[i] == '(' || name[i] == '[' || name[i] == '{' || name[i] == '<') finame += '(';
                else if (name[i] == ')' || name[i] == ']' || name[i] == '}' || name[i] == '>') finame += ')';
                else if ((name[i] < 'A' || name[i] > 'Z') && (name[i] < '0' || name[i] > '9') && (name[i] < 'a' || name[i] > 'z'))
                    finame += '·';
                else finame += name[i];
            }
            this.FindControl<TextBox>("formula").Text = finame;
            string nme = this.FindControl<TextBox>("name").Text;
            if (finame != null && finame != "" && nme != null && nme != "")
                this.FindControl<Button>("OKbutton").IsEnabled = true;
            else this.FindControl<Button>("OKbutton").IsEnabled = false;
        }
    }
}
