/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        const int ArrayLength = 10;
        const int Min = 0;
        const int Max = 100;
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            Task<int[]> arrayCreator = Task.Factory.StartNew(() => { 
                var numbers = new int[ArrayLength];
                var random = new Random();

                for (int i = 0; i < ArrayLength; i++)
                {
                    numbers[i] = random.Next(Min, Max);
                }

                Console.Write("Generated numbers -> ");
                Print(numbers);
                return numbers;
            });

            Task<int[]> arrayMultiplier = arrayCreator.ContinueWith(task =>
            {
                var numbers = new int[ArrayLength];
                Array.Copy(task.Result, numbers, ArrayLength);
                var randomNumber = new Random().Next(Min, Max);

                for (int i = 0; i < ArrayLength; i++)
                {
                    numbers[i] *= randomNumber;
                }

                Console.Write(string.Format("Multiplied by {0} -> ", randomNumber));
                Print(numbers);
                return numbers;
            });

            Task<int[]> arraySorter = arrayMultiplier.ContinueWith(task =>
            {
                var numbers = new int[ArrayLength];
                Array.Copy(task.Result, numbers, ArrayLength);
                Array.Sort(numbers);

                Console.Write("Sorted numbers -> ");
                Print(numbers);
                return numbers;
            });

            Task<double> averageCalculator = arraySorter.ContinueWith(task =>
            {
                var average = task.Result.Average();

                Console.WriteLine(string.Format("Average: {0}", average));
                return average;
            });

            Console.ReadLine();
        }

        static void Print(int[] numbers)
        {
            foreach (int num in numbers)
            {
                Console.Write(string.Format("{0}  ", num));
            }
            Console.WriteLine();
        }

    }
}
