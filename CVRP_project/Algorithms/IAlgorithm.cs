namespace CVRP_project.Algorithms
{
    internal interface IAlgorithm
    {
        public (List<int> route, double routeLength) Solve(int seed);

        public void Reset();

        public String Name();
    }
}
