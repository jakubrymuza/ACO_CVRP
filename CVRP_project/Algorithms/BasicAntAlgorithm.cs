﻿using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class BasicAntAlgorithm : IAlgorithm
    {
        protected ProblemInstance Cities;
        protected readonly int BaseStation = 0;

        protected Random RandomGenerator;
        protected int IterationsCount;

        protected Ant[] Ants;


        protected double Alpha;
        protected double Beta;
        protected double Evaporation;
        protected double PheromoneStrength;

        private List<int> GlobalBestRoute = new List<int>();
        private double GlobalBestRouteLength = double.MaxValue;

        public BasicAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength)
        {
            this.Cities = cities;
            IterationsCount = iterationsCount;

            Ants = new Ant[antsCount];
            Ant.Initialize(cities);

            for (int i = 0; i < antsCount; i++)
            {
                Ants[i] = new Ant();
            }
            

            Alpha = alpha;
            Beta = beta;
            Evaporation = evaporation;
            PheromoneStrength = pheromoneStrength;

            RandomGenerator = new Random();
        }

        public void Reset()
        {
            GlobalBestRoute = new List<int>();
            GlobalBestRouteLength = double.MaxValue;
        }

        public String Name() => "Basic Ant Algorithm";


        public (List<int> route, double routeLength) Solve(int seed)
        {
            RandomGenerator = new Random(seed);

            Cities.ResetPheromone();

            for (int i = 0; i < IterationsCount; i++)
            {
                ResetAnts();

                // najlepsza ścieżka w danej iteracji
                List<int> LocalBestRoute = new List<int>();
                double LocalBestRouteLength = double.MaxValue;

                for (int j = 0; j < Ants.Length; j++)
                {
                    SolveAnt(Ants[j]);

                    UpdateLocalBestRoute(ref LocalBestRoute, ref LocalBestRouteLength, Ants[j]);
                }

                UpdatePheromone();

                UpdateGlobalBestRoute(LocalBestRoute, LocalBestRouteLength);
            }

            return (GlobalBestRoute, GlobalBestRouteLength);
        }



        protected void SolveAnt(Ant ant)
        {
            int startCity = RandomGenerator.Next(Cities.Size - 1) + 1;
            ant.Visit(BaseStation);
            ant.Visit(startCity);

            while (!ant.RouteFinished()) // TODO: check if surpassed trucks limit?
            {
                int lastCity = 0;
                int nextCity = ChooseNextCity(ant, lastCity);

                if (nextCity == -1)
                    break;

                ant.Visit(nextCity);
            }
        }

        protected int ChooseNextCity(Ant ant, int lastCity)
        {
            double[] probabilities = CalculateProbabilities(ant, lastCity);

            return RunRoulette(probabilities);
        }



        protected double[] CalculateProbabilities(Ant ant, int lastCity)
        {
            // prawdopodebieństwo wybrania i-tego miasta
            double[] probabilities = new double[Cities.Size];
            double sum = 0.0;

            for (int city = 0; city < Cities.Size; city++)
            {
                if (!ant.CanVisit(city))
                {
                    probabilities[city] = 0.0;
                }
                else
                {
                    probabilities[city] = Math.Pow(Cities.GetPheromone(lastCity, city), Alpha) *
                                       Math.Pow(1.0 / Cities.GetDistance(lastCity, city), Beta);

                    sum += probabilities[city];
                }
            }

            // normalizacja prawdopobieństw do 1
            for (int i = 0; i < Cities.Size; i++)
            {
                probabilities[i] /= sum;
            }

            return probabilities;
        }

        protected int RunRoulette(double[] probabilities)
        {
            double r = RandomGenerator.NextDouble();
            double probSum = 0.0;

            for (int i = 0; i < Cities.Size; i++)
            {
                probSum += probabilities[i];
                if (r <= probSum)
                {
                    return i;
                }
            }

            return -1;
        }

        protected void UpdatePheromone()
        {
            EvaporatePheromone();

            AddNewPheromone();
        }

        protected void EvaporatePheromone()
        {
            for (int i = 0; i < Cities.Size; i++)
            {
                for (int j = 0; j < Cities.Size; j++)
                {
                    double newPheromone = Cities.GetPheromone(i, j) * (1.0 - Evaporation);
                    Cities.SetPheromone(i, j, newPheromone);
                }

            }
        }

        protected void AddNewPheromone()
        {
            foreach (Ant ant in Ants)
            {
                // TODO: daj feromon tylko jak dobra trasa?

                int lastCity = BaseStation;

                foreach (int city in ant.GetRoute())
                {
                    double newPheromone = Cities.GetPheromone(lastCity, city) + (PheromoneStrength / ant.GetRouteLength());
                    Cities.SetPheromone(lastCity, city, newPheromone);
                    Cities.SetPheromone(city, lastCity, newPheromone);
                }
            }

            PostAddNewPheromone();
        }

        protected void PostAddNewPheromone() { } // placeholder for modifications

        private void ResetAnts()
        {
            foreach (Ant ant in Ants)
            {
                ant.Reset();
            }
        }

        private void UpdateLocalBestRoute(ref List<int> LocalBestRoute, ref double LocalBestRouteLength, Ant ant)
        {
            if (ant.RouteFinished() && ant.WithinTrucksLimit())
            {
                if (ant.GetRouteLength() < LocalBestRouteLength)
                {
                    LocalBestRoute = ant.GetRoute();
                    LocalBestRouteLength = ant.GetRouteLength();
                }
            }
        }

        private void UpdateGlobalBestRoute(List<int> LocalBestRoute, double LocalBestRouteLength)
        {
            if (LocalBestRouteLength < GlobalBestRouteLength)
            {
                GlobalBestRoute = LocalBestRoute;
                GlobalBestRouteLength = LocalBestRouteLength;
            }
        }
    }
}