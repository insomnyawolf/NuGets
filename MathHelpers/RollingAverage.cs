namespace MathHelpers
{
    public class RollingAverage
    {
        private readonly double[] Values;
        private readonly int MaxPossibleLenght;

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

            Values[CurrentPosition] = nextValue;

            double total = 0;

            for (int i = 0; i < Values.Length; i++)
            {
                total += Values[i];
            }

            return total / Values.Length;
        }
    }
}
