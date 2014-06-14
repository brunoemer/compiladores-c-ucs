using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public enum StructureType
    {
        SimpleVar = 0,
        Function = 1
    }

    public class Symbol
    {
        public String id;
        public int type; // int, float... 
        public String context; // dentro de onde o id esta
        public StructureType sType; // variavel ou funcao
        public String temp; // variavel temporaria

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

        public Symbol(String _id, int _type, String _context, String _temp)
        {
            this.id = _id;
            this.type = _type;
            this.context = _context;
            this.temp = _temp;
        }

        public Symbol(String _id, int _type, StructureType _sType)
        {
            this.id = _id;
            this.type = _type;
            this.context = TableSymbol.GlobalContext;
            this.sType = _sType;
        }

        public String GetId(){
            if (this.temp == null)
            {
                return this.id;
            }
            else
            {
                return this.temp;
            }
        }

        public override String ToString()
        {
            return String.Format("id({0}) | type({1}) | contexto({2}) stype({3})", this.id, this.type, this.context, this.sType);
        }
    }
}
