namespace MathHelpers
{
    public class RollingAverage
    {
        private readonly int MaxPossibleLenght;
        private readonly double[] Values;
        public double Total = 0;

        public double Average => Total / Values.Length;

        private int CurrentPosition;
        public RollingAverage(int totalValues)
        {
            Values = new double[totalValues];
            MaxPossibleLenght = totalValues - 1;
        }

        public double Next(double nextValue)
        {
            CurrentPosition++;

            if (CurrentPosition > MaxPossibleLenght)
            {
                CurrentPosition = 0;
            }

            Total -= Values[CurrentPosition];
            Total += nextValue;

            Values[CurrentPosition] = nextValue;

            return Average;
        }
    }
}
