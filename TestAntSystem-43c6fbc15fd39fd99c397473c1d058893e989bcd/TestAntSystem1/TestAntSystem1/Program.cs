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
            AlgorithmCreator standartCreator = new AlgorithmCreator(new FileStream(@"C:\Users\Alex\Desktop\Graph.txt", FileMode.Open));
            
            IAlgorithm standartAlgorithm = standartCreator.CreateStandartAlgorithm();
            
            standartAlgorithm.Run();
            
            Console.ReadKey();
        }
    }
}
