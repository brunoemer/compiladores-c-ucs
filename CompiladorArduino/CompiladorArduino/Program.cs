using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompiladorArduino
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = true;

            Console.WriteLine("Iniciado processo...");

            try
            {
                AnalisadorSintatico analisadorSintatico = new AnalisadorSintatico();
                String codigo = analisadorSintatico.Execute();

                StringBuilder sb = new StringBuilder();
                foreach (string item in LineManager.Instance.LinesOut)
                {
                    sb.Append(item);
                }

                String ts = TableSymbol.getInstance().ToString();

                if (debug)
                {
                    //Console.WriteLine("Lex:\n" + sb.ToString());
                    Console.WriteLine("TableSymbol:\n" + ts);
                    //Console.WriteLine("C3E:\n" + codigo);
                }

                Arquivo.Write(@"Arquivos/saida_lex.txt", sb.ToString());
                Arquivo.Write(@"Arquivos/tablesymbol.txt", ts);
                Arquivo.Write(@"Arquivos/codigo.c3e", codigo);

            }
            catch (AnalisadorException ae)
            {
                Console.WriteLine(ae.Message);
                Console.WriteLine();
                Console.WriteLine("---ESTADO---");
                Console.WriteLine();
                Console.WriteLine(String.Format("Posição vetor da string: {0}", ae.Pos));
                Console.WriteLine(String.Format("Posição coluna: {0}", ae.Col));
                Console.WriteLine(String.Format("Linha: {0}", ae.Linha));
                Console.WriteLine(String.Format("Token: {0}", ae.Token));
                Console.WriteLine(String.Format("Código do Token: {0}", ae.TokenCod));
                Console.WriteLine();
                Console.WriteLine("------------");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("---MEMORIA---");
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("-------------");
                Console.WriteLine();
            }

            Console.WriteLine("Processo concluído, precione uma tecla...");
            Console.ReadKey();
        }
    }
}
