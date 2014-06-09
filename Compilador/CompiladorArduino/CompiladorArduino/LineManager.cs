using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompiladorArduino
{
    public class LineManager
    {
        public List<String> Lines { get; set; }
        public int LineIndex { get; set; }
        public String LineContent { get; set; }
        public List<String> LinesOut { get; set; }
        public int PosCurrentCaracter { get; set; }
        public int PosStartToken { get; set; } //Posicao que deve indicar aonde o Token começou a ser lido

        private static LineManager instance;

        public static LineManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LineManager();
                }

                return instance;
            }
        }

        public LineManager()
        {
            this.LineIndex = 0;

        }

        public void setArduinoCode(String arduinoCode)
        {
             List<string> list = new List<string>(
                                       arduinoCode.Split(new string[] { "\r\n" }, StringSplitOptions.None)
                                 );
             this.Lines = list;//Arquivo.Read(@"Arquivos/entrada.txt");

            this.LinesOut = new List<String>();
            this.LineIndex = 0;
            this.PosCurrentCaracter = 0;
            this.PosStartToken = 0;
        }

        public bool ReadLine()
        {
            this.PosCurrentCaracter = 0;
            this.PosStartToken = 0;

            try
            {
                this.LineContent = this.Lines[this.LineIndex].ToLower().Trim();
                this.LineIndex++;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ResetToLastPos()
        {
            this.PosCurrentCaracter = this.PosStartToken;
            try
            {
                LineManager.Instance.LinesOut.RemoveAt(LineManager.Instance.LinesOut.Count() - 1);
            }
            catch (Exception) { }
        }
    }
}
