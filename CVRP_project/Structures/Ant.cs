namespace CVRP_project.Structures
{
    internal class Ant
    {
        private static ProblemInstance Cities;
        private static int BaseStation = 0;

        private double CapacityLeft;
        private double DistanceLeft;

        private readonly List<int> Route;
        private double RouteLength;

        private readonly bool[] Visited;
        private int VisitedCount;
        private int TrucksUsed;



        public bool CanVisit(int city)
        {
            // nie można odwiedzić jeszcze raz miasta w którym już się jest
            if (city == Route.Last())
                return false;

            // zawsze można wrócić do stacji bazowej
            if (city == BaseStation)
                return true;

            // można odwiedzić miasto x, gdy x jest nieodwiedzone
            // demand x jest mniejszy od pozostałego capacity
            // oraz odległość do x i z x do stacji bazowej jest mnieszy od pozostałego sMax
            return !Visited[city] &&
                CapacityLeft >= Cities.GetDemand(city) &&
                DistanceLeft >= Cities.GetDistance(Route.Last(), city) + Cities.GetDistance(city, BaseStation);
        }


        public void Visit(int city)
        {
            if(Route.Count == 0)
            {
                Route.Add(city);
            }

            int lastCity = Route.Last();
            RouteLength += Cities.GetDistance(lastCity, city);

            if (city == BaseStation)
            {
                CapacityLeft = Cities.Capacity;
                DistanceLeft = Cities.MaxDistance;
                TrucksUsed++;
            }
            else
            {
                CapacityLeft -= Cities.GetDemand(city);
                DistanceLeft -= Cities.GetDistance(Route.Last(), city);
                VisitedCount++;
            }

            Route.Add(city);
            Visited[city] = true;
        }



        public List<int> GetRoute() => Route; 

        public double GetRouteLength() => RouteLength;

        public bool RouteFinished() => Route.Last() == BaseStation && VisitedCount == Cities.Size;

        public bool WithinTrucksLimit() => TrucksUsed <= Cities.TrucksLimit;

        public static void Initialize(ProblemInstance cities, int baseStation = 0)
        {
            Cities = cities;
            BaseStation = baseStation;
        }

        public void Reset()
        {
            CapacityLeft = Cities.Capacity;
            DistanceLeft = Cities.MaxDistance;
            TrucksUsed = 0;

            Route.Clear();
            RouteLength = 0;

            Array.Fill(Visited, false);
            VisitedCount = 1;
        }

        public Ant()
        {
            Route = new List<int>(2 * Cities.Size);
            Visited = new bool[Cities.Size];

            Reset();
        }
    }
}
