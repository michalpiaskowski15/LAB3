using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    const int N = 1000;
    static int[,] A = new int[N, N]; //deklaracja dwuwymiarowych macierzy
    static int[,] B = new int[N, N];
    static int[,] C = new int[N, N];

    static void Main(string[] args)
    {
        Random rand = new Random(); //generowanie losowych wartości macierzy i wypełnianie nimi tablic
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                A[i, j] = rand.Next(10); //liczba od 0-9
                B[i, j] = rand.Next(10);
            }
        }

        Console.WriteLine("Trwa wykonywanie obliczeń na jednym wątku...");
        Stopwatch watch1 = Stopwatch.StartNew();
        MultiplyMatrix(0, N);
        watch1.Stop();
        Console.WriteLine("Czas: {0} ms", watch1.ElapsedMilliseconds);

        Console.WriteLine("\nTrwa wykonywanie obliczeń na wielu wątkach...");
        Stopwatch watch2 = Stopwatch.StartNew();

        Thread[] threads = new Thread[Environment.ProcessorCount]; //tworzymy tablice wątków
        int chunkSize = N / threads.Length; //obliczamy rozmiar fragmentu macierzy dla każdego wątku
        int startIndex = 0; //początkowy indeks dla obliczeń każdego wątku

        for (int i = 0; i < threads.Length; i++) 
        {
            // Tworzenie nowego wątku i przekazanie mu zakresu wierszy do obliczenia
            threads[i] = new Thread(() => MultiplyMatrix(startIndex, startIndex + chunkSize));
            threads[i].Start();

            // Przesunięcie indeksu wiersza startowego dla następnego wątku
            startIndex += chunkSize;
        }

        // Oczekiwanie na zakończenie wszystkich wątków
        foreach (Thread t in threads)
        {
            t.Join();
        }
        watch2.Stop();
        Console.WriteLine("Czas: {0} ms", watch2.ElapsedMilliseconds);
        Console.WriteLine("Obliczenia na 4 wątkach wykonały się około {0} razy szybciej.", (watch1.ElapsedMilliseconds / watch2.ElapsedMilliseconds));
        Console.ReadKey();
    }

    //funkcja wykonująca mnożenie macierzy dla wierszy 
    static void MultiplyMatrix(int startRow, int endRow)
    {
        for (int i = startRow; i < endRow; i++)
        {
            for (int j = 0; j < N; j++)
            {
                int sum = 0;
                for (int k = 0; k < N; k++)
                {
                    sum += A[i, k] * B[k, j];
                }
                C[i, j] = sum;
            }
        }
    }
}
