using System;

namespace SimpleBruteForce
{
    public class Bruteforce
    {
        private const string PossibleValues = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        private readonly Func<string, string> Transformation;

        public Bruteforce(Func<string, string> Transformation = null)
        {
            this.Transformation = Transformation ?? new Func<string, string>(item => item);
        }

        public string Force(string Target)
        {
            char[] Current = Array.Empty<char>();

            while (true)
            {
                Array.Resize(ref Current, Current.Length + 1);

                bool addOneToPrevVal = false;

                while (!addOneToPrevVal)
                {
                    for (int currentIndex = 0; currentIndex < Current.Length; currentIndex++)
                    {
                        if (currentIndex == 0)
                        {
                            for (int letterIndex = 0; letterIndex < PossibleValues.Length; letterIndex++)
                            {
                                Current[currentIndex] = PossibleValues[letterIndex];

                                var value = new string(Current);

                                string valueTransformed = Transformation(value);

                                if (valueTransformed == Target)
                                {
                                    return value;
                                }

                                addOneToPrevVal = true;
                            }
                        }

                        if (addOneToPrevVal)
                        {
                            var letterindex = PossibleValues.IndexOf(Current[currentIndex]);

                            letterindex++;
                            if (letterindex > PossibleValues.Length - 1)
                            {
                                letterindex = 0;
                            }
                            else
                            {
                                addOneToPrevVal = false;
                            }
                            Current[currentIndex] = PossibleValues[letterindex];
                        }
                    }
                }
            }
        }
    }
}