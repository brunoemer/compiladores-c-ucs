using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinguagensFormais
{
    class AnalisadorFimArquivoException : Exception
    {
        public AnalisadorFimArquivoException(string message) : base(message) { }
    }
}
