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

        public void Add(String id, int type)
        {
            this.Add(new Symbol(id, type));
        }

        public void Add(String id, int type, String context)
        {
            this.Add(new Symbol(id, type, context));
        }

        public void Add(String id, int type, StructureType stype)
        {
            this.Add(new Symbol(id, type, stype));
        }

        public void Add(Symbol s)
        {
            if (this.Exists(s.id))
            {
                throw new AnalisadorException("Identificador já declarado.");
            }
            this.TabelaSimbolos.Add(s);
        }

        public Symbol GetSymbol(String id)
        {
            foreach (Symbol s in this.TabelaSimbolos)
            {
                if (s.id.Equals(id) && s.context.Equals(CurrentContext))
                {
                    return s;
                }
            }
            return null;
        }

        public int GetType(String id)
        {
            Symbol s = this.GetSymbol(id);
            if (s == null)
            {
                return -1;
            }

            return s.type;
        }

        public bool Exists(String id)
        {
            if (this.GetSymbol(id) == null)
            {
                return false;
            }
            return true;
        }

        public bool ExistsVar(String id)
        {
            if (!this.Exists(id))
            {
                throw new AnalisadorException("Identificador não declarado.");
            }
            return true;
        }

        public bool ExistsFunction(String id)
        {
            bool hasFunc = false;
            foreach (Symbol s in this.TabelaSimbolos)
            {
                if (s.id.Equals(id) && s.context.Equals(CurrentContext) && s.sType == StructureType.Function)
                {
                    hasFunc = true;
                    break;
                }
            }

            if (!this.Exists(id) || !hasFunc)
            {
                throw new AnalisadorException("Função não declarada. Idenficador: "+id);
            }
            return true;
        }

        public int CalcType(int op, int t1, int t2)
        {
            // ver combinacoes com op, testar no c
            if (op == LexMap.Consts["OU"] ||
                op == LexMap.Consts["E"] ||
                op == LexMap.Consts["MAIOR"] ||
                op == LexMap.Consts["IGUAL"] ||
                op == LexMap.Consts["MENOR"] ||
                op == LexMap.Consts["MAIORIGUAL"] ||
                op == LexMap.Consts["DIFERENTE"] ||
                op == LexMap.Consts["MENORIGUAL"])
            {
                return LexMap.Consts["LOGICO"];
            }

            if (t1 == LexMap.Consts["CONSTFLOAT"] || t2 == LexMap.Consts["CONSTFLOAT"])
            {
                return LexMap.Consts["CONSTFLOAT"];
            }
            else if (t1 == LexMap.Consts["CONSTINTEIRO"] || t2 == LexMap.Consts["CONSTINTEIRO"])
            {
                return LexMap.Consts["CONSTINTEIRO"];
            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Symbol item in this.TabelaSimbolos)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }
    }
}
