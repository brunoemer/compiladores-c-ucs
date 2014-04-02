using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinguagensFormais
{
    public class AnalisadorLexico
    {
        public static void Analisar()
        {
            String nome;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Token: {0}", TokenManager.Instance.TokenSymbol));
            sb.AppendLine(String.Format("Token Anterior: {0}", TokenManager.Instance.TokenSymbolAnt));
            sb.AppendLine(String.Format("Código do Token: {0}", TokenManager.Instance.TokenCode));
            sb.AppendLine(String.Format("Código do Token Anterior: {0}", TokenManager.Instance.TokenCodeAnt));
            sb.AppendLine(String.Format("Posição do Caracter de parada: {0}", LineManager.Instance.PosCurrentCaracter));
            sb.AppendLine(String.Format("Posição de início de leitura do Token: {0}", LineManager.Instance.PosStartToken));

            if (AnalisadorLexico.Reconhecedor())
            {
                if (TokenManager.Instance.TokenCode == LexMap.Consts["ID"])
                {
                    try
                    {
                        TokenManager.Instance.TokenCode = LexMap.PalavraReservada[TokenManager.Instance.TokenSymbol];
                    }
                    catch (Exception) { }
                }

                try
                {
                    nome = LexMap.TokenNome[TokenManager.Instance.TokenCode];

                    LineManager.Instance.LinesOut.Add(
                        String.Format("Cod: {0}\tLinha: {1}\tColuna: {2}\tTipo: {3}\tLexema: {4}\n",
                        TokenManager.Instance.TokenCode,
                        LineManager.Instance.LineIndex,
                        LineManager.Instance.PosStartToken, 
                        nome,
                        TokenManager.Instance.TokenSymbol));
                }
                catch (Exception)
                {
                    throw new AnalisadorException(String.Format("O Token -{0}- não foi encontrado", TokenManager.Instance.TokenSymbol));
                }
            }
            else
            {
                TokenManager.Instance.TokenCode = 0;
                //TokenManager.Instance.TokenCodeAnt = 0;
                TokenManager.Instance.TokenSymbol = "Provável fim de arquivo";
                //TokenManager.Instance.TokenSymbolAnt = "Provável fim de arquivo";
                //throw new AnalisadorFimArquivoException("O reconhecedor encontrou uma sintaxe não finalizada" + Environment.NewLine + sb.ToString());
            }
        }

        public static bool Reconhecedor()
        {
            Int32 estado = 0;
            TokenManager.Instance.TokenSymbol = String.Empty;
            TokenManager.Instance.TokenCode = 0;

            bool endofline = false;
            bool parsing_a_float = false;
            char c = '\0';

            //Busco próximo caracter (mesmo que tenha que pular linhas)
            if (LineManager.Instance.PosCurrentCaracter < LineManager.Instance.LineContent.Length)
            {
                c = LineManager.Instance.LineContent[LineManager.Instance.PosCurrentCaracter];
            }
            else
            {
                if (!LineManager.Instance.ReadLine())
                {
                    //throw new AnalisadorFimArquivoException("O arquivo chegou ao fim.");
                    return false;
                }

                while (String.IsNullOrEmpty(LineManager.Instance.LineContent.Trim()))
                {
                    if (!LineManager.Instance.ReadLine())
                    {
                        //throw new AnalisadorFimArquivoException("O arquivo chegou ao fim.");
                        return false;
                    }
                    
                }
                c = LineManager.Instance.LineContent[LineManager.Instance.PosCurrentCaracter];
            }

            while (c == ' ' || c == '\n' || Environment.NewLine.Equals(c) || c == '\t')
            {
                LineManager.Instance.PosCurrentCaracter++;

                if (LineManager.Instance.PosCurrentCaracter >= LineManager.Instance.LineContent.Length)
                {
                    while (String.IsNullOrEmpty(LineManager.Instance.LineContent.Trim()))
                    {
                        if (!LineManager.Instance.ReadLine())
                        {
                            return false;
                        }
                    }
                }

                c = LineManager.Instance.LineContent[LineManager.Instance.PosCurrentCaracter];
            }
            //FIM -> Termino de buscar o próximo caracter

            while (endofline == false)
            {
                if (estado == 0)
                {
                    if (c == ' ' || c == '\n' || Environment.NewLine.Equals(c) || c == '\t')
                    {
                        //nada
                    }
                    else
                    {
                        estado = LexMap.Tipo(c);
                        if (estado <= 0)
                        {
                            return false;
                        }
                    }

                    if (c != ' ' && c != '\t')
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosStartToken = LineManager.Instance.PosCurrentCaracter;
                    }

                    LineManager.Instance.PosCurrentCaracter++;
                }


                endofline = false;
                if (LineManager.Instance.PosCurrentCaracter >= LineManager.Instance.LineContent.Length)
                {
                    endofline = true;
                }
                else
                {
                    c = LineManager.Instance.LineContent[LineManager.Instance.PosCurrentCaracter];
                }


                // Segunda "volta" concatena o token inteiro

                if (estado == LexMap.Consts["CONSTINTEIRO"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else if(c.Equals('.') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATPONTO"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else if (c.Equals('e') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATNUME"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["CONSTINTEIRO"];
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["CONSTFLOAT"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["CONSTFLOAT"];
                        parsing_a_float = true;
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["CONSTFLOATPONTO"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATPONTONUM"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["PONTO"];
                        parsing_a_float = true;
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["CONSTFLOATPONTONUM"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else if (c.Equals('e') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATNUME"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["CONSTFLOAT"];
                        parsing_a_float = true;
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["CONSTFLOATNUME"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOAT"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else if ((c.Equals('+') || c.Equals('-')) && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATE"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    // erro?
                }
                else if (estado == LexMap.Consts["CONSTFLOATNUMPONTO"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOAT"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else if (c.Equals('e') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATE"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["CONSTFLOAT"];
                        parsing_a_float = true;
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["CONSTFLOATE"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOAT"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    // se nao terminar em numero? erro?
                }
                else if (estado == LexMap.Consts["ID"])
                {
                    if ((LexMap.Letras.Contains(c) ||
                         LexMap.Numeros.Contains(c) ||
                         LexMap.Caracteres.Contains(c)) &&
                         (endofline == false))
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["ID"];
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["STRING"])
                {
                    if (endofline == true)
                    {
                        return false;
                    }

                    if (c != '"')
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;

                        TokenManager.Instance.TokenCode = LexMap.Consts["STRING"];

                        return true;
                    }
                }
                else if (estado == LexMap.Consts["DOISPONTOS"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["DOISPONTOS"];

                    return true;
                }
                else if (estado == LexMap.Consts["VIRGULA"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["VIRGULA"];

                    return true;
                }
                else if (estado == LexMap.Consts["PONTOVIRGULA"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["PONTOVIRGULA"];

                    return true;
                }
                else if (estado == LexMap.Consts["ABREPAR"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["ABREPAR"];

                    return true;
                }
                else if (estado == LexMap.Consts["FECHAPAR"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["FECHAPAR"];

                    return true;
                }
                else if (estado == LexMap.Consts["ABRECHAVES"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["ABRECHAVES"];

                    return true;
                }
                else if (estado == LexMap.Consts["FECHACHAVES"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["FECHACHAVES"];

                    return true;
                }
                else if (estado == LexMap.Consts["MAIS"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false && parsing_a_float == true)
                    {
                        estado = LexMap.Consts["CONSTINTEIRO"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else if (c.Equals('.') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATPONTO"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["MAIS"];
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["MENOS"])
                {
                    if (LexMap.Numeros.Contains(c) && endofline == false && parsing_a_float == true)
                    {
                        estado = LexMap.Consts["CONSTINTEIRO"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                    }
                    else if (c.Equals('.') && endofline == false)
                    {
                        estado = LexMap.Consts["CONSTFLOATPONTO"];
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        parsing_a_float = true;
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["MENOS"];
                        return true;
                    }
                }
                else if (estado == LexMap.Consts["MULTIPLICACAO"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["MULTIPLICACAO"];

                    return true;
                }
                else if (estado == LexMap.Consts["DIVISAO"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["DIVISAO"];

                    return true;
                }
                else if (estado == LexMap.Consts["NAO"])
                {
                    if (c.Equals('=') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["DIFERENTE"];
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["NAO"];
                    }

                    return true;
                }
                else if (estado == LexMap.Consts["MENOR"])
                {
                    if (c.Equals('=') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["MENORIGUAL"];
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["MENOR"];
                    }

                    return true;
                }
                else if (estado == LexMap.Consts["ATRIBUICAO"])
                {
                    if (c.Equals('=') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["IGUAL"];
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["ATRIBUICAO"];
                    }

                    return true;
                }
                else if (estado == LexMap.Consts["MAIOR"])
                {
                    if (c.Equals('=') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["MAIORIGUAL"];
                    }
                    else
                    {
                        TokenManager.Instance.TokenCode = LexMap.Consts["MAIOR"];
                    }

                    return true;
                }
                else if (estado == LexMap.Consts["MODC"])
                {
                    TokenManager.Instance.TokenCode = LexMap.Consts["MODC"];

                    return true;
                }
                else if (estado == LexMap.Consts["DIFERENTE"])
                {
                    if (c.Equals('=') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["DIFERENTE"];

                        return true;
                    }
                }
                else if (estado == LexMap.Consts["E"])
                {
                    if (c.Equals('&') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["E"];

                        return true;
                    }
                }
                else if (estado == LexMap.Consts["OU"])
                {
                    if (c.Equals('|') && endofline == false)
                    {
                        TokenManager.Instance.TokenSymbol += c;
                        LineManager.Instance.PosCurrentCaracter++;
                        TokenManager.Instance.TokenCode = LexMap.Consts["OU"];

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
