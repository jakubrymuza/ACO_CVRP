using CVRP_project.Algorithms;
using CVRP_project.Structures;

namespace CVRP_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: load variables
            String filePath = "C:/Users/Jakub Rymuza/Downloads/test1.txt";
            ProblemInstance cities = new ProblemInstance(filePath);

            int iterationsCount = 1000;
            int antsCount = cities.Size;

            double alpha = 2;
            double beta = 5;
            double evaporation = 0.5;
            double pheromoneStrength = 1;

            int elitistAntsCount = 10;
            int rankingSize = antsCount / 3;

            int repetitions = 10;
            int seed = 100;


            IAlgorithm[] algorithms = {
            new BasicAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength),
            new GreedyAntAlgorithm(cities, iterationsCount, antsCount),
            new ElitistAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, elitistAntsCount),
            new RankedAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, rankingSize)
            };

            StreamWriter writer = new StreamWriter("output.txt");

            foreach (var algorithm in algorithms)
            {
                var result = RunAlgorithm(algorithm, repetitions, seed);
                // TODO: odchylenie standardowe?

                PrintResults(writer, result, algorithm.Name());
                PrintResults(Console.Out, result, algorithm.Name());
            }

            writer.Flush();
        }

        private static void PrintResults(TextWriter writer, (double bestLength, double worstLength, double averageLength) value, string name)
        {
            writer.WriteLine($"{name}:");
            writer.WriteLine($"best length: {value.bestLength}");
            writer.WriteLine($"worst length: {value.worstLength}");
            writer.WriteLine($"average length: {value.averageLength}");
            writer.WriteLine();
        }

        private static (double bestLength, double worstLength, double averageLength) RunAlgorithm(IAlgorithm algorithm, int repetitions, int seed)
        {
            double bestLength = double.MaxValue;
            double worstLength = double.MinValue;
            double averageLength = 0;

            for (int i = 0; i < repetitions; i++)
            {
                algorithm.Reset();
                (_, double length) = algorithm.Solve(seed);

                if (bestLength > length)
                {
                    bestLength = length;
                }

                if (worstLength < length)
                {
                    worstLength = length;
                }

                averageLength += length;
            }

            averageLength /= repetitions;

            return (bestLength, worstLength, averageLength);
        }
    }
}