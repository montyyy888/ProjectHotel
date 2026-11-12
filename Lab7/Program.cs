using System;
using System.Text;

namespace Hotel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            bool running = true;
            while (running)
            {
                Console.WriteLine("=== Готель ===");
                Console.WriteLine("1 — довідка про програму");
                Console.WriteLine("2 — вартість проживання");
                Console.WriteLine("3 — завантаженість готелю по днях");
                Console.WriteLine("0 — вихід");
                Console.Write("Ваш вибір: ");

                // Trim прибирає випадкові пробіли, null буває при Ctrl+Z
                string choice = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        ShowCostTable();
                        Pause();
                        break;
                    case "3":
                        ShowOccupancyTable();
                        Pause();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("До побачення!");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey(true);
            Console.Clear();
        }
        static void ShowAbout()
        {
            Console.WriteLine("Тема: Готель");
            Console.WriteLine("Головна сутність: Бронювання");
            Console.WriteLine("Довідник: Категорія номера");
            Console.WriteLine();
            Console.WriteLine("Поля сутності «Бронювання»:");
            Console.WriteLine("- string   : номер броні");
            Console.WriteLine("- string   : прізвище гостя");
            Console.WriteLine("- int      : номер кімнати");
            Console.WriteLine("- int      : код категорії (з довідника)");
            Console.WriteLine("- DateTime : дата заїзду");
            Console.WriteLine("- int      : кількість ночей");
            Console.WriteLine("- double   : ціна за ніч");
            Console.WriteLine("- bool     : чи сплачено");
        }