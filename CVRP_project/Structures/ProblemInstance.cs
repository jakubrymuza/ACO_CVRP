namespace CVRP_project.Structures
{
    internal class ProblemInstance
    {
        private readonly double[,] ProximityMatrix;
        private readonly double[] Demands;

        public int Size { get; private set; }
        public double Capacity { get; private set; }
        public double MaxDistance { get; private set; }
        public int TrucksLimit { get; private set; }

        public double GetDistance(int a, int b) => ProximityMatrix[a, b];

        public double GetDemand(int a) => Demands[a];

        // schemat pliku: w pierwszej linijce size, capacity, maxDistance i trucksLimit
        // w następnych size linijkach: współrzędne x,y klientów (stacja bazowa pierwsza)
        // w następnych size linijkach: demand klientów
        public ProblemInstance(String filePath)
        {
            string fileText = File.ReadAllText(filePath);
            string[] lines = fileText.Split('\n');

            ReadParametres(lines);

            ProximityMatrix = new double[Size, Size];

            Demands = new double[Size];

            var positions = ReadCoordinates(lines);

            CalculateCoordinates(positions);

            ReadDemand(lines);
        }

        private void ReadParametres(string[] lines)
        {
            string[] nums = lines[0].Split(" ");

            Size = int.Parse(nums[0]);
            Capacity = double.Parse(nums[1]);
            MaxDistance = double.Parse(nums[2]);
            TrucksLimit = int.Parse(nums[3]);
        }

        private (double x, double y)[] ReadCoordinates(string[] lines)
        {
            (double x, double y)[] positions = new (double x, double y)[Size];

            for (int i = 0; i < Size; i++)
            {
                string[] nums = lines[i + 1].Split(" ");

                int id = int.Parse(nums[0]) - 1;
                positions[id] = (double.Parse(nums[1]), double.Parse(nums[2]));
            }

            return positions;
        }

        private void CalculateCoordinates((double x, double y)[] positions)
        {
            for (int i = 0; i < Size; i++)
            {
                var pt1 = positions[i];

                for (int j = 0; j < Size; j++)
                {
                    var pt2 = positions[j];

                    ProximityMatrix[i, j] = Math.Sqrt(Math.Pow(pt2.x - pt1.x, 2) + Math.Pow(pt2.y - pt1.y, 2));
                }
            }
        }

        private void ReadDemand(string[] lines)
        {
            for (int i = 0; i < Size; i++)
            {
                string[] nums = lines[i + Size + 1].Split(" ");

                int id = int.Parse(nums[0]) - 1;
                Demands[id] = double.Parse(nums[1]);
            }
        }
    }
}
