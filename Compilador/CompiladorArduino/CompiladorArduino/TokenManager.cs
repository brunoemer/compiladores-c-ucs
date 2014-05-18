using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompiladorArduino
{
    public class TokenManager
    {
        public Int32 TokenCode { get; set; }
        public String TokenSymbol { get; set; }
        public Int32 TokenCodeAnt { get; set; }
        public String TokenSymbolAnt { get; set; }

        private static TokenManager instance { get; set; }

        public static TokenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TokenManager();
                }

                return instance;
            }
        }

        public TokenManager()
        {
            this.TokenSymbol = String.Empty;
            this.TokenCode = 0;

            this.TokenSymbolAnt = String.Empty;
            this.TokenCodeAnt = 0; 
        }
    }
}
