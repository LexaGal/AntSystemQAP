using System;
using System.IO;
using Grsu.Lab.Aoc.Contracts;
using TestAntSystem1.Classes;

namespace TestAntSystem1
{
    public class Program
    {        
        public static void Main()
        {
            AlgorithmCreator standartCreator = new AlgorithmCreator(new FileStream(@"C:\Graph.txt", FileMode.Open));
            
            IAlgorithm standartAlgorithm = standartCreator.CreateStandartAlgorithm();
            
            standartAlgorithm.Run();

            StreamWriter writer = new StreamWriter(@"C:\Result.txt");
            ((StandartAntAlgorithm) standartAlgorithm).Result.CopyTo(writer.BaseStream);
            
            writer.Write(true);
            
        }
    }
}
