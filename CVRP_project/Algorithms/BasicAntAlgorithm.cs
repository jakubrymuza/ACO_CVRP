using CVRP_project.Structures;

namespace CVRP_project.Algorithms
{
    internal class BasicAntAlgorithm : IAlgorithm
    {
        protected readonly ProblemInstance Cities;
        protected readonly int BaseStation = 0;

        protected Random RandomGenerator;
        protected readonly int IterationsCount;

        protected readonly Ant[] Ants;

        protected readonly double[,] PheromoneMatrix;

        protected readonly double Alpha;
        protected readonly double Beta;
        protected readonly double Evaporation;
        protected readonly double PheromoneStrength;
        protected readonly double OverLimitPenaltyFactor; // kara na feromon za zbyt dużą liczbę ciężarówek

        protected List<int> GlobalBestRoute = new List<int>();
        protected double GlobalBestRouteLength = double.MaxValue;

        protected List<int> LocalBestRoute = new List<int>();
        protected double LocalBestRouteLength = double.MaxValue;

        public virtual String Name() => "Basic Ant Algorithm";

        public BasicAntAlgorithm(ProblemInstance cities, int iterationsCount, int antsCount, double alpha, double beta, double evaporation, double pheromoneStrength, double overLimitPenaltyFactor)
        {
            Cities = cities;
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
            OverLimitPenaltyFactor = overLimitPenaltyFactor;

            PheromoneMatrix = new double[Cities.Size, Cities.Size];

            RandomGenerator = new Random();

            Reset();
        }

        public void Reset()
        {
            GlobalBestRoute.Clear();
            GlobalBestRouteLength = double.MaxValue;

            LocalBestRoute.Clear();
            LocalBestRouteLength = double.MaxValue;
        }

        public (List<int> route, double routeLength) Solve(int seed)
        {
            RandomGenerator = new Random(seed);

            ResetPheromone();

            for (int i = 0; i < IterationsCount; i++)
            {
                ResetAnts();
                ResetLocalBestRoute();

                for (int j = 0; j < Ants.Length; j++)
                {
                    SolveAnt(Ants[j], j);
                }

                UpdateLocalBestRoute();

                UpdateGlobalBestRoute();

                UpdatePheromone();
            }

            return (GlobalBestRoute, GlobalBestRouteLength);
        }

        protected void SolveAnt(Ant ant, int startCity)
        {
            if(startCity >= Cities.Size)
                startCity = RandomGenerator.Next(Cities.Size - 1) + 1;

            ant.Visit(BaseStation);
            ant.Visit(startCity);

            while (!ant.RouteFinished())
            {
                int lastCity = ant.GetLastCity();

                int nextCity = ChooseNextCity(ant, lastCity);

                if (nextCity == -1) // nie znaleziono żadnego miasta
                    break;

                ant.Visit(nextCity);
            }

            if (!ant.RouteFinished())
            {
                ant.InvalidateResult();
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
                    probabilities[city] = CalculateProbability(lastCity, city);

                    sum += probabilities[city];
                }
            }

            for (int i = 0; i < Cities.Size; i++)
            {
                probabilities[i] /= sum;
            }

            return probabilities;
        }

        protected virtual double CalculateProbability(int lastCity, int city) => 
            Math.Pow(PheromoneMatrix[lastCity, city], Alpha) *
                               Math.Pow(1.0 / Cities.GetDistance(lastCity, city), Beta);


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
                    PheromoneMatrix[i, j] *= (1.0 - Evaporation);
                }

            }
        }

        protected virtual void AddNewPheromone()
        {
            foreach (Ant ant in Ants)
            {
                if (!ant.RouteFinished())
                    continue;


                double pheromoneFactor = 1.0;
                if (!ant.WithinTrucksLimit())
                {
                    pheromoneFactor = OverLimitPenaltyFactor;
                }

                var route = ant.GetRoute();
                for (int i = 1; i < route.Count; ++i)
                {
                    int lastCity = route[i - 1];
                    int city = route[i];

                    double newPheromone = pheromoneFactor * (PheromoneStrength / ant.GetRouteLength());

                    if (lastCity == BaseStation || city == BaseStation)
                    {
                        newPheromone *= 0;
                    }

                    PheromoneMatrix[lastCity, city] += newPheromone;
                    PheromoneMatrix[city, lastCity] += newPheromone;
                }
            }

            PostAddNewPheromone();
        }

        protected virtual void PostAddNewPheromone() { } // placeholder for modifications

        private void UpdateLocalBestRoute()
        {
            foreach (Ant ant in Ants)
            {
                if (ant.WithinTrucksLimit() && ant.RouteFinished())
                {
                    if (ant.GetRouteLength() < LocalBestRouteLength)
                    {
                        LocalBestRoute = new List<int>(ant.GetRoute());
                        LocalBestRouteLength = ant.GetRouteLength();
                    }
                }
            }

        }

        private void UpdateGlobalBestRoute()
        {
            if (LocalBestRouteLength < GlobalBestRouteLength)
            {
                GlobalBestRoute = new List<int>(LocalBestRoute);
                GlobalBestRouteLength = LocalBestRouteLength;
            }
        }

        private void ResetAnts()
        {
            foreach (Ant ant in Ants)
            {
                ant.Reset();
            }
        }

        private void ResetLocalBestRoute()
        {
            LocalBestRoute.Clear();
            LocalBestRouteLength = double.MaxValue;
        }

        public void ResetPheromone()
        {
            for (int i = 0; i < Cities.Size; i++)
            {
                for (int j = 0; j < Cities.Size; j++)
                {
                    PheromoneMatrix[i, j] = 1.0;
                }
            }
        }
    }
}
