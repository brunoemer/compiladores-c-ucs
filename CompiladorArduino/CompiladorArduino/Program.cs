using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinguagensFormais
{
    class Program
    {
        static void Main(string[] args)
        {
            int x, k, y;
            for(x = 0, k = 7, y = x * 8; x < 10 && k >8; x++){}

            bool debug = false;

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

                if (debug)
                {
                    Console.WriteLine("Sintatico: "+codigo);
                    Console.Write(sb.ToString());
                }

                Arquivo.Write(@"Arquivos/saida_lex.txt", sb.ToString());
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
