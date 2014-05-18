using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorArduino
{
    public class Arquivo
    {
        public static List<String> Read(String FilePath)
        {
            List<String> Linhas = new List<String>();

            FileStream stream = (new FileInfo(FilePath)).OpenRead();
            StreamReader reader = new StreamReader(stream, Encoding.Default);
            String linha;
            while ((linha = reader.ReadLine()) != null)
            {
                Linhas.Add(linha);
            }

            reader.Close();
            stream.Close();

            return Linhas;
        }

        public static void Write(String FilePath, String Content)
        {
            FileStream stream = (new FileInfo(FilePath)).OpenWrite();
            stream.SetLength(0);
            StreamWriter writer = new StreamWriter(stream, Encoding.Default);
            writer.Write(Content);
            writer.Close();
            stream.Close();
        }
    }
}
