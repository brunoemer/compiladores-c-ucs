using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    class Symbol
    {
        public String id;
        public int type; // int, float...
        public String context;

        public Symbol(String _id, int _type)
        {
            this.id = _id;
            this.type = _type;
            this.context = TableSymbol.GlobalContext;
        }

        public Symbol(String _id, int _type, String _context)
        {
            this.id = _id;
            this.type = _type;
            this.context = _context;
        }
    }
}
