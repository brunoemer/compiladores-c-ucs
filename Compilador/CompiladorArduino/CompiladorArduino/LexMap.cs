using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public class LexMap
    {
        public static Dictionary<String, Int32> Consts = new Dictionary<String, Int32>()
        {
            {"CONSTINTEIRO", 1}, //q1
            {"ID", 2},
            {"MAIS", 3}, //q10
            {"MENOS", 4}, //q11
            {"MULTIPLICACAO", 5},
            {"DIVISAO", 6},
            {"MENOR", 8},
            {"MAIOR", 9},
            {"IGUAL", 10},
            {"MAIORIGUAL", 11},
            {"MENORIGUAL", 12},
            {"DIFERENTE", 13},
            {"ATRIBUICAO", 14},
            {"ABREPAR", 16},
            {"FECHAPAR", 17},
            {"DOISPONTOS", 20},
            {"PONTO", 21},
            {"VIRGULA", 22},
            {"PONTOVIRGULA", 23},
            {"CONSTFLOAT", 25}, //q26
            {"CONSTFLOATPONTO", 26}, //q21
            {"CONSTFLOATPONTONUM", 27}, //q22
            {"CONSTFLOATNUME", 28}, //q23
            {"CONSTFLOATNUMPONTO", 29}, //q24
            {"CONSTFLOATE", 30}, //q25
            {"ESCREVA", 42},
            {"LEIA", 43},
            {"ESCREVAL", 44},
            {"INTEIRO", 45},
            {"FLOAT", 46},
            {"E", 47},
            {"OU", 48},
            {"LONG", 49},
            {"BYTE", 50},
            {"LOGICO", 51},
            {"TRUE", 52},
            {"FALSE", 53},
            {"HIGH", 54},
            {"LOW", 55},
            {"INPUT", 56},
            {"OUTPUT", 57},
            {"VOID", 58},
            {"NAO", 59},
            {"STRING", 61},
            {"MODC", 65},
            {"IF", 66},
            {"ELSE", 68},
            {"FOR", 70},
            {"ABRECHAVES", 73},
            {"FECHACHAVES", 74},
            {"WHILE", 75},
            {"DO", 77},
            {"PROCEDIMENTO", 78},
            {"FIMPROCEDIMENTO", 79},
            {"VAZIO", 80},
            {"FUNCAO", 81},
            {"FIMFUNCAO", 82},
            {"RETURN", 83},
            {"SWITCH", 84},
            {"CASE", 85},
            {"BREAK", 86},
            {"DEFAULT", 87}
        };

        public static Dictionary<String, Int32> PalavraReservada = new Dictionary<String, Int32>()
        {
            {"int", 45},
            {"float", 46},
            {"long", 49},
            {"byte", 50},
            {"bool", 51},
            {"void", 58},
            {"switch", 84},
            {"case", 85},
            {"break", 86},
            {"default", 87},
            {"if", 66},
            {"else", 68},
            {"for", 70},
            {"while", 75},
            {"do", 77},
            {"true", 52},
            {"false", 53},
            {"high", 54},
            {"low", 55},
            {"input", 56},
            {"output", 57},
            {"return", 83}
        };

        public static Dictionary<Int32, String> TokenNome = new Dictionary<Int32, String>()
        {
            {1, "Tk_Const_Int"},
            {2, "Tk_Ident"},
            {3, "Tk_Mais"},
            {4, "Tk_Menos"},
            {5, "Tk_Multiplicacao"},
            {6, "Tk_Divisao"},
            {7, "Tk_Resto"},
            {8, "Tk_Menor"},
            {9, "Tk_Maior"},
            {10, "Tk_Igual"},
            {11, "Tk_Maior_Igual"},
            {12, "Tk_Menor_Igual"},
            {13, "Tk_Diferente"},
            {14, "Tk_Atribuicao"},
            {15, "Tk_Div_Inteira"},
            {16, "Tk_Abre_Parenteses"},
            {17, "Tk_Fecha_Parenteses"},
            {20, "Tk_Dois_Pontos"},
            {21, "Tk_Ponto"},
            {22, "Tk_Virgula"},
            {23, "Tk_Ponto_Virgula"},
            {25, "Tk_Const_Float"},
            {42, "Tk_Escreva"},
            {43, "Tk_Leia"},
            {44, "Tk_Escreval"},
            {45, "Tk_Inteiro"},
            {46, "Tk_Float"},
            {47, "Tk_E"},
            {48, "Tk_Ou"},
            {49, "Tk_Long"},
            {50, "Tk_Byte"},
            {51, "Tk_Logico"},
            {52, "Tk_True"},
            {53, "Tk_False"},
            {54, "Tk_High"},
            {55, "Tk_Low"},
            {56, "Tk_Input"},
            {57, "Tk_Output"},
            {58, "Tk_Void"},
            {59, "Tk_Nao"},
            {61, "Tk_String"},
            {65, "Tk_Resto_Char"},
            {66, "Tk_If"},
            {68, "Tk_Else"},
            {70, "Tk_For"},
            {73, "Tk_Abre_Chaves"},
            {74, "Tk_Fecha_Chaves"},
            {75, "Tk_While"},
            {77, "Tk_Do"},
            {78, "Tk_Procedimento"},
            {79, "Tk_FimProcedimento"},
            {80, "Tk_Vazio"},
            {81, "Tk_Funcao"},
            {82, "Tk_FimFuncao"},
            {83, "Tk_Return"},
            {84, "Tk_Switch"},
            {85, "Tk_Case"},
            {86, "Tk_Break"},
            {87, "Tk_Default"}
        };

        public static List<char> Letras = new List<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        public static List<char> Numeros = new List<char>()
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static List<char> Caracteres = new List<char>()
        {
            '_'
        };

        public static int Tipo(Char c)
        {
            if (LexMap.Numeros.Contains(c))
            {
                return LexMap.Consts["CONSTINTEIRO"];
            }
            if (LexMap.Letras.Contains(c) || c == '_') // pode comecar com _
            {
                return LexMap.Consts["ID"];
            }
            else if (c == '.')
            {
                return LexMap.Consts["CONSTFLOATPONTO"];
            }
            else if (c == ':')
            {
                return LexMap.Consts["DOISPONTOS"];
            }
            else if (c == '"')
            {
                return LexMap.Consts["STRING"];
            }
            else if (c == ',')
            {
                return LexMap.Consts["VIRGULA"];
            }
            else if (c == ';')
            {
                return LexMap.Consts["PONTOVIRGULA"];
            }
            else if (c == '<')
            {
                return LexMap.Consts["MENOR"];
            }
            else if (c == '>')
            {
                return LexMap.Consts["MAIOR"];
            }
            else if (c == '=')
            {
                return LexMap.Consts["ATRIBUICAO"];
            }
            else if (c == '+')
            {
                return LexMap.Consts["MAIS"];
            }
            else if (c == '-')
            {
                return LexMap.Consts["MENOS"];
            }
            else if (c == '*')
            {
                return LexMap.Consts["MULTIPLICACAO"];
            }
            else if (c == '/')
            {
                return LexMap.Consts["DIVISAO"];
            }
            else if (c == '(')
            {
                return LexMap.Consts["ABREPAR"];
            }
            else if (c == ')')
            {
                return LexMap.Consts["FECHAPAR"];
            }
            else if (c == '{')
            {
                return LexMap.Consts["ABRECHAVES"];
            }
            else if (c == '}')
            {
                return LexMap.Consts["FECHACHAVES"];
            }
            else if (c == '%')
            {
                return LexMap.Consts["MODC"];
            }
            else if (c == '!')
            {
                return LexMap.Consts["NAO"];
            }
            else if (c == '&')
            {
                return LexMap.Consts["E"];
            }
            else if (c == '|')
            {
                return LexMap.Consts["OU"];
            }

            return 0;
        }

        public static string TokenGetNome(int _key)
        {
            String value = null;
            bool flag = LexMap.TokenNome.TryGetValue(_key, out value);

            if (!flag)
            {
                throw new Exception(String.Format("Token de chave {0} não encontrado", _key));    
            }

            return value;
        }
    }
}
