using System;
using System.IO;
using System.Collections.Generic;

namespace Hangman
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> countriesAndCapitals = ReadCountriesAndCapitals("countries_and_capitals.txt");
            bool applicationEnabled = true;

            if (countriesAndCapitals.Count == 0)
            {
                applicationEnabled = false;
            }

            Console.WriteLine("Welcome in Hangnam!");
            Console.WriteLine("Press any key to start...");
            Console.ReadLine();

            while (applicationEnabled)
            {
                string[] countryAndCapital = SelectRandomCountryAndCapital(countriesAndCapitals);

                for (int i = 0; i < countryAndCapital.Length; i++)
                {
                    countryAndCapital[i] = countryAndCapital[i].Trim();
                }

                int countryIndex = 0;
                int capitalIndex = 1;
                int lifePoints = 5;
                int guessTries = 0;
                DateTime beginTime = DateTime.Now;

                string[] capitalSplit = countryAndCapital[capitalIndex].Split(null);

                for (int i = 0; i < capitalSplit.Length; i++)
                {
                    capitalSplit[i] = capitalSplit[i].Trim();
                }

                List<string[] > capitalSplitByLetter = new List<string[]>();
                List<string> guessedLetters = new List<string>();
                Console.WriteLine(countryAndCapital[capitalIndex].Trim());
                InitializeGameView(capitalSplit, capitalSplitByLetter, lifePoints);
                bool capitalGuessed = false;

                for (int i = 0; i < lifePoints && !capitalGuessed;)
                {
                    string guessType = WaitUntilPlayerTypesGuessType();

                    if (guessType.Equals("l"))
                    {
                        Console.WriteLine("Guess a letter...");
                    }
                    else
                    {
                        Console.WriteLine("Guess a whole word...");
                    }

                    guessTries++;
                    string answer = Console.ReadLine();
                    int guessNumber = 0;

                    if (guessType.Equals("l"))
                    {


                        for (int j = 0; j < capitalSplitByLetter.Count; j++)
                        {
                            string[] wordLetters = capitalSplitByLetter[j];

                            for (int k = 0; k < wordLetters.Length; k++)
                            {

                                if (wordLetters[k].ToLower().Equals(answer.ToLower()))
                                {
                                    guessNumber++;
                                    continue;
                                }
                            }
                        }

                        if (guessNumber == 0)
                        {
                            lifePoints--;
                        }
                    }
                    else if (answer.ToLower().Equals(countryAndCapital[capitalIndex].ToLower()))
                    {
                        capitalGuessed = true;

                        for (int j = 0; j < answer.Length; j++)
                        {
                            string guessedLetter = Char.ToString(answer[j]).ToLower();

                            if (!guessedLetters.Contains(guessedLetter))
                            {
                                guessedLetters.Add(guessedLetter);
                            }
                        }
                    }
                    else
                    {
                        lifePoints = lifePoints - 2;
                    }

                    if (!guessedLetters.Contains(answer.ToLower()))
                    {
                        guessedLetters.Add(answer.ToLower());
                    }

                    if (guessNumber == 0 && lifePoints >= 1 && !capitalGuessed)
                    {
                        Console.WriteLine("Wrong guess! Try again...");
                    }

                    if (lifePoints == 0)
                    {
                        Console.WriteLine("\nOps... You've lost...");

                        applicationEnabled = AskIfWantPlayAgain();
                    }
                    else if (capitalGuessed || PlayerHasGuessedCapital(guessedLetters, capitalSplitByLetter))
                    {
                        Console.WriteLine($"\nAmazing! You've won! {countryAndCapital[capitalIndex]} is capital of {countryAndCapital[countryIndex]}");

                        capitalGuessed = true;

                        AskForNameAndSaveScore(beginTime, guessTries, countryAndCapital[capitalIndex]);

                        applicationEnabled = AskIfWantPlayAgain();
                    }
                    else
                    {
                        RewriteGameView(capitalSplitByLetter, guessedLetters, lifePoints, countryAndCapital[countryIndex]);
                    }
                }
            }
            Console.WriteLine("Thanks for playing Hangman. See you soon!");
        }




        private static List<string> ReadCountriesAndCapitals(string filename)
        {

            List<string> countriesAndCapitals = new List<string>();

            try
            {

                using (var sr = new StreamReader(filename))
                {

                    while (!sr.EndOfStream)
                    {
                        countriesAndCapitals.Add(sr.ReadLine());
                    }

                }

            }

            catch (IOException e)
            {
                Console.WriteLine("The file could not be read: filename=" + filename);
                Console.WriteLine(e.Message);
            }

            return countriesAndCapitals;
        }



        private static string[] SelectRandomCountryAndCapital(List<string> countriesAndCapitals)
        {

            int randomIndex = new Random().Next(countriesAndCapitals.Count);

            string[] randomPair = countriesAndCapitals[randomIndex].Split("|", StringSplitOptions.RemoveEmptyEntries/*TrimEntries*/);

            while (randomPair.Length != 2)
            {
                randomIndex = new Random().Next(countriesAndCapitals.Count);
                randomPair = countriesAndCapitals[randomIndex].Split("|", StringSplitOptions.RemoveEmptyEntries);
            }

            return randomPair;
        }



        private static void InitializeGameView(string[] capitalSplit, List<string[]> capitalSplitByLetter, int lifePoints)
        {

            Console.WriteLine($"Current life points ({lifePoints})");
            for (int i = 0; i < capitalSplit.Length; i++)
            {
                string capitalSingleWord = capitalSplit[i].Trim();

                if (i != 0)
                {
                    Console.Write(" ");
                }

                capitalSplitByLetter.Insert(i, new string[capitalSingleWord.Length]);
            

                for (int j = 0; j < capitalSingleWord.Length; j++)
                {
                    capitalSplitByLetter[i][j] = Char.ToString(capitalSingleWord[j]);

                    Console.Write("_");
                }
            }
            Console.WriteLine("\n");
        }
        private static string WaitUntilPlayerTypesGuessType()
        {
            Console.WriteLine("Would you like to guess a letter or whole word?");
            Console.WriteLine("type l (letter) or w (word)...");

            string guessType = Console.ReadLine();
            while (!guessType.Equals("l") && !guessType.Equals("w"))
            {
                Console.WriteLine("type l (letter) or w (word)...");
                guessType = Console.ReadLine();
            }

            return guessType;
        }

        private static void RewriteGameView(List<string[]> capitalSplitByLetter, List<string> guessedLetters, int lifePoints, string country)
        {
            Console.WriteLine($"\nCurrent life points ({lifePoints})");
            Console.WriteLine($"Already used - {string.Join(", ", guessedLetters)}");

            for (int i = 0; i < capitalSplitByLetter.Count; i++)
            {
                string[] capitalSingleWord = capitalSplitByLetter[i];

                if (i != 0)
                {
                    Console.Write(" ");
                }

                for (int j = 0; j < capitalSingleWord.Length; j++)
                {
                    string letter = capitalSingleWord[j];

                    if (guessedLetters.Contains(letter.ToLower()))
                    {
                        Console.Write(letter);
                    }
                    else
                    {
                        Console.Write("_");
                    }

                }
            }

            if (lifePoints == 1)
            {
                Console.WriteLine($"\nHINT: The capital of {country}");
            }
            else
            {
                Console.WriteLine("\n");
            }
        }
        private static bool PlayerHasGuessedCapital(List<string> guessedLetters, List<string[]> capitalSplitByLetter)
        {
            int lettersToGuess = 0;
            int guessedLettersCount = 0;

            foreach (string[] letters in capitalSplitByLetter)
            {
                lettersToGuess += letters.Length;
            }

            foreach (string[] letters in capitalSplitByLetter)
            {
                for (int i = 0; i < letters.Length; i++)
                {
                    if (guessedLetters.Contains(letters[i].ToLower()))
                    {
                        guessedLettersCount += 1;
                    }
                }
            }

            return lettersToGuess == guessedLettersCount;
        }
        private static bool AskIfWantPlayAgain()
        {
            Console.WriteLine("Would you like to try again?");
            Console.WriteLine("type y to start again...");

            string answer = Console.ReadLine().Trim();

            if (!answer.Equals("y"))
            {
                return false;
            }

            return true;
        }
        private static void AskForNameAndSaveScore(DateTime beginTime, int guessTries, string capital)
        {
            Console.WriteLine("Enter your name...");
            string name = Console.ReadLine();

            DateTime endTime = DateTime.Now;
            long ticks = endTime.Ticks - beginTime.Ticks;

            string scoreToSave = $"{name} | {DateTime.Now.ToString()} | {new TimeSpan(ticks).TotalSeconds} seconds | {guessTries} tries | {capital}";

            Console.WriteLine(scoreToSave);

            /*StreamReader sr = new StreamReader("HighScore.txt");*/

            try
            {
                using (StreamWriter sw = new StreamWriter("HighScore.txt"))
                {
                    sw.Write(scoreToSave.ToCharArray());
                }
            } catch (IOException e)
            {
                Console.WriteLine("Unable to save HighScore!");
                Console.WriteLine(e.Message);
            }
            

           
          
        }
       
    }
}



