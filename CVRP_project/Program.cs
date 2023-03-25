using CVRP_project.Algorithms;
using CVRP_project.Structures;
using System.Diagnostics;

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
            double overLimitPenaltyFactor = 0.1;

            int elitistAntsCount = 10;
            int rankingSize = antsCount / 3;

            int repetitions = 5;
            int seed = 100;


            IAlgorithm[] algorithms = {
            new BasicAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor),
            new GreedyAntAlgorithm(cities, iterationsCount, antsCount, overLimitPenaltyFactor),
            new ElitistAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor, elitistAntsCount),
            new RankedAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor, rankingSize)
            };

            StreamWriter writer = new StreamWriter("output.txt");

            writer.WriteLine($"Algorytmy mrówkowe, rozmiar: {cities.Size}, liczba powtórzeń: {repetitions}, liczba iteracji: {iterationsCount}\n");
            Console.WriteLine($"Algorytmy mrówkowe, rozmiar: {cities.Size}, liczba powtórzeń: {repetitions}, liczba iteracji: {iterationsCount}\n");

            Parallel.ForEach(algorithms, algorithm =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = RunAlgorithm(algorithm, repetitions, seed);

                stopwatch.Stop();

                lock (writer)
                {
                    PrintResults(writer, result, algorithm.Name(), stopwatch.ElapsedMilliseconds / repetitions);
                    PrintResults(Console.Out, result, algorithm.Name(), stopwatch.ElapsedMilliseconds / repetitions);
                }
            });

            writer.Flush();

            Console.WriteLine("Enter any key to continue");
            Console.ReadKey();
        }

        private static void PrintResults(TextWriter writer, (double bestLength, double worstLength, double averageLength, double deviation) value, string name, long msTime)
        {
            writer.WriteLine($"{name}:");
            writer.WriteLine($"średni czas wykonania jednego powtórzenia algorytmu: {msTime / 1000.0}s");

            if (value.bestLength == double.MaxValue)
            {
                writer.WriteLine("nie znaleziono poprawnego rozwiązania");
                return;
            }


            writer.WriteLine($"najlepszy wynik: {value.bestLength}");
            writer.WriteLine($"najgorszy wynik: {value.worstLength}");
            writer.WriteLine($"średni wynik: {value.averageLength}");
            writer.WriteLine($"odchylenie standardowe: {value.deviation}");
            writer.WriteLine();
        }

        private static (double bestLength, double worstLength, double averageLength, double deviation) RunAlgorithm(IAlgorithm algorithm, int repetitions, int seed)
        {
            double bestLength = double.MaxValue;
            double worstLength = double.MinValue;
            double averageLength = 0;

            double[] results = new double[repetitions];

            for (int i = 0; i < repetitions; i++)
            {
                algorithm.Reset();
                (_, double length) = algorithm.Solve(i * seed);
                results[i] = length;

                if (length == double.MaxValue)
                    continue;

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

            double deviation = CalculateDeviation(results, averageLength);

            return (bestLength, worstLength, averageLength, deviation);
        }

        private static double CalculateDeviation(double[] results, double average)
        {
            double deviation = 0.0;

            foreach (double value in results)
            {
                deviation += Math.Pow(value - average, 2);
            }

            deviation /= results.Length;

            return Math.Sqrt(deviation);
        }
    }
}