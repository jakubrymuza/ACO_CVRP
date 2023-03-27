using CVRP_project.Algorithms;
using CVRP_project.Structures;
using System.Diagnostics;

namespace CVRP_project
{
    public class Program
    {
        // argumenty wywołania:
        // - ścieżka do pliku z instancją problemu
        // - liczba iteracji algorytmów

        public static void Main(string[] args)
        {
            String filePath = args[0];
            ProblemInstance cities = new ProblemInstance(filePath);

            int iterationsCount = int.Parse(args[1]);
            int antsCount = cities.Size;

            double alpha = 1;
            double beta = 5;
            double evaporation = 0.5;
            double pheromoneStrength = 10;
            double overLimitPenaltyFactor = 0.1;

            int elitistAntsCount = 5;
            int rankingSize = antsCount / 3;

            int repetitions = 10;
            int seed = 1000;

            IAlgorithm[] algorithms = {
            new GreedyAntAlgorithm(cities, iterationsCount, antsCount, beta, overLimitPenaltyFactor),
            new BasicAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor),
            new ElitistAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor, elitistAntsCount),
            new RankedAntAlgorithm(cities, iterationsCount, antsCount, alpha, beta, evaporation, pheromoneStrength, overLimitPenaltyFactor, elitistAntsCount, rankingSize)
            };

            StreamWriter writer = new StreamWriter("output.txt");

            writer.WriteLine($"Algorytmy mrówkowe, rozmiar: {cities.Size}, limit ciężarówek {cities.TrucksLimit}, liczba powtórzeń: {repetitions}, liczba iteracji: {iterationsCount}\n");
            Console.WriteLine($"Algorytmy mrówkowe, rozmiar: {cities.Size}, limit ciężarówek {cities.TrucksLimit}, liczba powtórzeń: {repetitions}, liczba iteracji: {iterationsCount}\n");

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

        private static void PrintResults(TextWriter writer, (List<int> route, double bestLength, double worstLength, double averageLength, double deviation) result, string name, long msTime)
        {
            writer.WriteLine($"{name}:");
            writer.WriteLine($"średni czas wykonania jednego powtórzenia algorytmu: {msTime / 1000.0}s");

            if (result.bestLength == double.MaxValue)
            {
                writer.WriteLine("nie znaleziono poprawnego rozwiązania\n");
                return;
            }

            writer.WriteLine($"najlepszy wynik: {result.bestLength}");
            writer.WriteLine($"najgorszy wynik: {result.worstLength}");
            writer.WriteLine($"średni wynik: {result.averageLength}");
            writer.WriteLine($"odchylenie standardowe: {result.deviation}");

            WriteRoute(writer, result.route);

            writer.WriteLine();
        }

        private static void WriteRoute(TextWriter writer, List<int> route)
        {
            writer.Write($"Najelpsze znalezione rozwiązanie: ");

            int baseStation = 0;
            for (int i = 0; i < route.Count; i++)
            {

                if (route[i] == baseStation)
                {
                    if (i > 0)
                    {
                        writer.Write("]");
                    }

                    if (i < route.Count - 1)
                    {
                        writer.Write("[");
                    }

                    continue;
                }

                writer.Write(route[i]);
                if (route[i + 1] != baseStation)
                {
                    writer.Write(", ");
                }
            }

            writer.WriteLine();
        }

        private static (List<int> route, double bestLength, double worstLength, double averageLength, double deviation) RunAlgorithm(IAlgorithm algorithm, int repetitions, int seed)
        {
            double bestLength = double.MaxValue;
            double worstLength = double.MinValue;
            double averageLength = 0;

            double[] results = new double[repetitions];

            List<int> route = new List<int>();

            for (int i = 0; i < repetitions; i++)
            {
                algorithm.Reset();
                (route, double length) = algorithm.Solve(i * seed);
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

            return (route, bestLength, worstLength, averageLength, deviation);
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