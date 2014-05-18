using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public class AnalisadorException : Exception
    {
        /* POSIÇÃO DO VETOR DA STRING */
        private int _Pos;
        public int Pos
        {
            get
            {
                return this._Pos;
            }
        }

        /* COLUNA QUE REPRESENTA O ARQUIVO TEXTO (SEMPRE +1 DO POS) */
        private int _Col;
        public int Col
        {
            get
            {
                return this._Col;
            }
        }

        /* LINHA QUE DEU ERRO */
        private int _Linha;
        public int Linha
        {
            get
            {
                return this._Linha;
            }
        }

        /* TOKEN cod */
        private int _TokenCod;
        public int TokenCod
        {
            get
            {
                return this._TokenCod;
            }
        }

        /* TOKEN LIDO */
        private string _Token;
        public string Token
        {
            get
            {
                return this._Token;
            }
        }

        /*
        public AnalisadorException(string message)
            : base(message)
        {
            this._Pos = 0;
            this._Col = 1;
            this._Linha = 0;
            this._Token = String.Empty;
        }
         */

        public AnalisadorException(string message) : base(message)
        {
            this._Pos = LineManager.Instance.PosStartToken;
            this._Col = LineManager.Instance.PosCurrentCaracter;
            this._Linha = LineManager.Instance.LineIndex;
            this._TokenCod = TokenManager.Instance.TokenCode;
            this._Token = TokenManager.Instance.TokenSymbol;
        }

        public AnalisadorException(string message, int pos, int linha, string token)
            : this(message)
        {
            this._Pos = pos;
            this._Col = pos + 1;
            this._Linha = linha;
            this._Token = token;
        }

        public AnalisadorException(string message, int pos, int linha, int tokenCod, string token)
            : this(message, pos, linha, token)
        {
            this._TokenCod = tokenCod;
        }
    }
}
