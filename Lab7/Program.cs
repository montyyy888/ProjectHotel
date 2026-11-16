using System;
using System.Globalization;
using System.Text;

namespace Hotel
{
    class Program
    {
        const double PricePerNight = 1200.0;  // ціна за ніч, грн
        const double DiscountPercent = 10.0;  // знижка, %
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            bool running = true;
            while (running)
            {
                ShowMenu();

                // Trim прибирає випадкові пробіли, null буває при Ctrl+Z
                string choice = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": ShowCostTable(); Pause(); break;
                    case "3": ShowOccupancyTable(); Pause(); break;
                    case "0": running = false; break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
            Console.WriteLine("До побачення!");
        }
        static void ShowMenu()
        {
            Console.WriteLine("=== Готель ===");
            Console.WriteLine("1 — довідка про програму");
            Console.WriteLine("2 — вартість проживання");
            Console.WriteLine("3 — завантаженість готелю по днях");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
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
        // Рахує вартість проживання, нічого не виводить
        static double CalculateCost(double nights, double pricePerNight)
        {
            return nights * pricePerNight;
        }

        // Рахує суму після знижки у відсотках
        static double ApplyDiscount(double total, double percent)
        {
            return total * (100.0 - percent) / 100.0;
        }
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim();

                double value;
                // приймаємо і кому, і крапку
                bool ok = double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                       || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

                if (!ok)
                {
                    Console.WriteLine("Помилка! Це не число. Спробуйте ще раз.");
                }
                else if (!(value >= min && value <= max)) // так відсіюється і NaN
                {
                    Console.WriteLine("Помилка! Число має бути від " + min + " до " + max + ".");
                }
                else
                {
                    return value;
                }
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string text = (Console.ReadLine() ?? "").Trim();

                int value;
                if (!int.TryParse(text, out value))
                {
                    Console.WriteLine("Помилка! Потрібне ціле число. Спробуйте ще раз.");
                }
                else if (value < min || value > max)
                {
                    Console.WriteLine("Помилка! Число має бути від " + min + " до " + max + ".");
                }
                else
                {
                    return value;
                }
            }
        }
        static void ShowCostTable()
        {
            Console.WriteLine("=== Таблиця вартості проживання ===");
            double start = ReadDouble("Початок (ночей, 0,5–10): ", 0.5, 10.0);
            double end = ReadDouble("Кінець (ночей, не менше за початок, до 10): ", start, 10.0);
            double step = ReadDouble("Крок (ночей, 0,5–5): ", 0.5, 5.0);
            Console.WriteLine();

            PrintCostTable(start, end, step);
        }

        // Табулювання: вартість залежно від кількості ночей
        static void PrintCostTable(double start, double end, double step)
        {
            string discountHead = "Зі знижкою " + DiscountPercent + "%";
            Console.WriteLine($"{"Ночей",8}{"Вартість",12}{discountHead,18}");

            // 1e-9 додаємо, щоб остання стрічка не зникла через похибку double
            for (double days = start; days <= end + 1e-9; days += step)
            {
                double total = CalculateCost(days, PricePerNight);
                double withDiscount = ApplyDiscount(total, DiscountPercent);
                Console.WriteLine($"{days,8:F1}{total,12:F2}{withDiscount,18:F2}");
            }
        }
        static void ShowOccupancyTable()
        {
            Console.WriteLine("=== Зведена таблиця завантаженості ===");

            Console.Write("Введіть кількість категорій (рядків, 1–20): ");
            int rows;
            if (!int.TryParse(Console.ReadLine(), out rows) || rows <= 0 || rows > 20)
            {
                Console.WriteLine("Помилка! Потрібне ціле число від 1 до 20.");
                return;
            }

            Console.Write("Введіть кількість днів (стовпців, 1–10): ");
            int cols;
            if (!int.TryParse(Console.ReadLine(), out cols) || cols <= 0 || cols > 10)
            {
                Console.WriteLine("Помилка! Потрібне ціле число від 1 до 10.");
                return;
            }

            Console.WriteLine();

            // Шапка таблиці (друкується до зовнішнього циклу)
            Console.Write($"{"Категорія",-12}");
            for (int j = 1; j <= cols; j++)
            {
                string head = "День " + j;
                Console.Write($"{head,8}");
            }
            Console.WriteLine($"{"Разом",8}");

            int[] colSums = new int[cols]; // підсумки по стовпцях
            int grandTotal = 0;

            // Зовнішній цикл — рядки (категорії)
            for (int i = 1; i <= rows; i++)
            {
                string name = "Кат. " + i;
                Console.Write($"{name,-12}");

                int rowSum = 0; // обнуляється для кожного рядка

                // Внутрішній цикл - стовпці (дні)
                for (int j = 1; j <= cols; j++)
                {
                    int val = i * j; // умовна кількість зайнятих номерів
                    rowSum += val;
                    colSums[j - 1] += val;
                    Console.Write($"{val,8}");
                }

                Console.WriteLine($"{rowSum,8}");
                grandTotal += rowSum;
            }

            // Рядок підсумків по стовпцях
            Console.Write($"{"Разом",-12}");
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{colSums[j],8}");
            }
            Console.WriteLine($"{grandTotal,8}");
        }
    }
}