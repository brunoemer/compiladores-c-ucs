using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    class TableSymbol
    {
        private static TableSymbol instance = null;
        private List<Symbol> TabelaSimbolos;
        
        public static String GlobalContext = "";
        public static String CurrentContext;
        
        public TableSymbol()
        {
            this.TabelaSimbolos = new List<Symbol>();
        }
        
        public static TableSymbol getInstance()
        {
            if(TableSymbol.instance == null){
                TableSymbol.instance = new TableSymbol();
            }
            return TableSymbol.instance;
        }

        public void Add(Symbol s)
        {
            this.TabelaSimbolos.Add(s);
        }

        public bool Exists(String id)
        {
            foreach (Symbol s in this.TabelaSimbolos)
            {
                if (s.id.Equals(id) && s.context.Equals(CurrentContext))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
