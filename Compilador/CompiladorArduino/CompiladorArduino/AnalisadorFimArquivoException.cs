using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    class AnalisadorFimArquivoException : Exception
    {
        public AnalisadorFimArquivoException(string message) : base(message) { }
    }
}
