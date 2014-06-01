using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using InterfaceGrafica;
using CompiladorArduino;



namespace InterfaceGrafica
{
    public partial class Form1 : Form
    {
        //styles
        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        public Form1()
        {
            InitializeComponent();
            fctb.Language = FastColoredTextBoxNS.Language.Custom;
            fctb.CommentPrefix = "//";
            fctb.Selection.Start = Place.Empty;
            fctb.DoCaretVisible();
            fctb.IsChanged = false;
            fctb.ClearUndo();
            fctb.Text = @"
int i, j;

if(true){

}

int end, fim;
end = fim;

";


        }


        private void Compile()
        {
            try
            {
                AnalisadorSintatico analisadorSintatico = new AnalisadorSintatico();
                String codigo = analisadorSintatico.Execute(fctb.Text);

                StringBuilder sb = new StringBuilder();
                foreach (string item in LineManager.Instance.LinesOut)
                {
                    sb.Append(item);
                }

                textBoxC3E.Text = codigo;
                textBoxErrors.Text = "Compilado\r\n-----------------------\r\n\r\n";

                String ts = TableSymbol.Instance.ToString();
                textBoxC3E.Text += Environment.NewLine + "Tabela simbolos:" + Environment.NewLine + ts;

            }
            catch (AnalisadorException ae)
            {
                textBoxC3E.Text = "";
                textBoxErrors.Text = ae.Message + Environment.NewLine;
                textBoxErrors.Text += String.Format("Posição vetor da string: {0}{1}", ae.Pos, Environment.NewLine);
                textBoxErrors.Text += String.Format("Posição coluna: {0}{1}", ae.Col, Environment.NewLine);
                textBoxErrors.Text += String.Format("Linha: {0} ", ae.Linha);
                textBoxErrors.Text += String.Format("Token: {0}{1}-----------------------{1}{1}", ae.Token, Environment.NewLine);
               /* Console.WriteLine();
                Console.WriteLine("---ESTADO---");
                Console.WriteLine();
                Console.WriteLine(String.Format("Posição vetor da string: {0}", ae.Pos));
                Console.WriteLine(String.Format("Posição coluna: {0}", ae.Col));
                Console.WriteLine(String.Format("Linha: {0}", ae.Linha));
                Console.WriteLine(String.Format("Token: {0}", ae.Token));
                Console.WriteLine(String.Format("Código do Token: {0}", ae.TokenCod));
                Console.WriteLine();
                Console.WriteLine("------------");
                Console.WriteLine();*/
            }
            catch (Exception e)
            {
                textBoxC3E.Text = "";
                textBoxErrors.Text = e.Message + Environment.NewLine;
            }
        }



        //expressões regulares que definem a sintax do C (para visualização no editor)
        private void CSyntaxHighlight(TextChangedEventArgs e)
        {
            fctb.LeftBracket = '(';
            fctb.RightBracket = ')';
            fctb.LeftBracket2 = '\x0';
            fctb.RightBracket2 = '\x0';
            //clear style of changed range
            e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);

            //string highlighting
            e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            //comment highlighting
            e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(GreenStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            //number highlighting
            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            //attribute highlighting
            e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //class name highlighting
            e.ChangedRange.SetStyle(BoldStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
            //keyword highlighting
            e.ChangedRange.SetStyle(BlueStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();
            //set folding markers
            e.ChangedRange.SetFoldingMarkers("{", "}");//allow to collapse brackets block
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");//allow to collapse #region blocks
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");//allow to collapse comment block
            timer1.Enabled = false;
            timer1.Enabled = true;
                      
        }

        private void fctb_TextChanged(object sender, TextChangedEventArgs e)
        {
            CSyntaxHighlight(e);
        }

        private void fctb_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            fctb.VisibleRange.ClearStyle(SameWordsStyle);

            if (fctb.Selection.Start != fctb.Selection.End)
                return;//user selected diapason

            //get fragment around caret
            var fragment = fctb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
                return;
            //highlight same words
            var ranges = fctb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
            if (ranges.Length > 1)
                foreach (var r in ranges)
                    r.SetStyle(SameWordsStyle);
        }

        
        private void timer1_Tick_1(object sender, EventArgs e)
        {

            Compile();
            timer1.Enabled = false;
        }
    }
}
