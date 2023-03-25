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
            if (city == GetLastCity())
                return false;

            // zawsze można wrócić do stacji bazowej
            if (city == BaseStation)
                return true;

            // można odwiedzić miasto x, gdy x jest nieodwiedzone
            // demand x jest mniejszy od pozostałego capacity
            // oraz odległość do x i z x do stacji bazowej jest mnieszy od pozostałego sMax
            return !Visited[city] &&
                CapacityLeft >= Cities.GetDemand(city) &&
                DistanceLeft >= Cities.GetDistance(GetLastCity(), city) + Cities.GetDistance(city, BaseStation);
        }


        public void Visit(int city)
        {
            if (Route.Count == 0)
            {
                Route.Add(city);
                Visited[city] = true;
                VisitedCount++;
                return;
            }

            int lastCity = GetLastCity();
            RouteLength += Cities.GetDistance(lastCity, city);

            if (city == BaseStation)
            {
                CapacityLeft = Cities.Capacity;
                DistanceLeft = Cities.MaxDistance;
                Visited[city] = true;
                TrucksUsed++;
            }
            else
            {
                CapacityLeft -= Cities.GetDemand(city);
                DistanceLeft -= Cities.GetDistance(GetLastCity(), city);
                Visited[city] = true;
                VisitedCount++;
            }

            Route.Add(city);
            Visited[city] = true;
        }



        public List<int> GetRoute() => Route;

        public double GetRouteLength() => RouteLength;

        public bool RouteFinished() => GetLastCity() == BaseStation && VisitedCount == Cities.Size;

        public bool WithinTrucksLimit() => TrucksUsed <= Cities.TrucksLimit;

        public int GetLastCity() => Route.Count > 0 ? Route.Last() : -1;

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
            VisitedCount = 0;
        }

        public Ant()
        {
            Route = new List<int>(2 * Cities.Size);
            Visited = new bool[Cities.Size];

            Reset();
        }
    }
}
