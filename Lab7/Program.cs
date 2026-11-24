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

            const int Capacity = 100;                 // місткість масиву
            double[] values = new double[Capacity];   // ціни за ніч, грн
            int count = 0;                            // скільки місць зайнято
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
                    case "4":
                        {
                            double newValue = ReadDouble("Введіть ціну за ніч (0–100000): ", 0, 100000);
                            AddValue(values, ref count, newValue, Capacity);
                            Console.WriteLine("Записів тепер (лічильник у Main): " + count);
                            Pause();
                            break;
                        }
                    case "5": ShowValues(values, count); Pause(); break;
                    case "6": ShowStatistics(values, count); Pause(); break;
                    case "7": DemoValueVsReference(); Pause(); break;
                    case "8":
                        {
                            int number = ReadInt("Номер для видалення (як у таблиці): ", 1, Capacity);
                            RemoveAt(values, ref count, number - 1);
                            Pause();
                            break;
                        }
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
            PrintTitle("Готель");
            Console.WriteLine("1 — довідка про програму");
            Console.WriteLine("2 — вартість проживання");
            Console.WriteLine("3 — завантаженість готелю по днях");
            Console.WriteLine("4 — додати значення (ціну за ніч)");
            Console.WriteLine("5 — показати всі значення");
            Console.WriteLine("6 — показати статистику");
            Console.WriteLine("7 — дослід: значення і посилання");
            Console.WriteLine("8 — видалити за номером");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        static void PrintTitle(string title)
        {
            Console.WriteLine("=== " + title + " ===");
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
            PrintTitle("Таблиця вартості проживання");
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
            PrintTitle("Зведена таблиця завантаженості");

            int rows = ReadInt("Введіть кількість категорій (рядків, 1–20): ", 1, 20);
            int cols = ReadInt("Введіть кількість днів (стовпців, 1–10): ", 1, 10);
            Console.WriteLine();

            PrintOccupancyTable(rows, cols);
        }

        // Умовна кількість зайнятих номерів категорії в певний день
        static int CalculateOccupied(int category, int day)
        {
            return category * day;
        }

        // Сума по одному дню для всіх категорій
        static int CalculateColumnSum(int rows, int day)
        {
            int sum = 0;
            for (int i = 1; i <= rows; i++)
            {
                sum += CalculateOccupied(i, day);
            }
            return sum;
        }

        static void PrintOccupancyHeader(int cols)
        {
            Console.Write($"{"Категорія",-12}");
            for (int j = 1; j <= cols; j++)
            {
                string head = "День " + j;
                Console.Write($"{head,8}");
            }
            Console.WriteLine($"{"Разом",8}");
        }

        static void PrintOccupancyTable(int rows, int cols)
        {
            PrintOccupancyHeader(cols);

            int grandTotal = 0;

            // Зовнішній цикл — рядки (категорії)
            for (int i = 1; i <= rows; i++)
            {
                string name = "Кат. " + i;
                Console.Write($"{name,-12}");

                int rowSum = 0; // обнуляється для кожного рядка

                // Внутрішній цикл — стовпці (дні)
                for (int j = 1; j <= cols; j++)
                {
                    int val = CalculateOccupied(i, j);
                    rowSum += val;
                    Console.Write($"{val,8}");
                }

                Console.WriteLine($"{rowSum,8}");
                grandTotal += rowSum;
            }

            PrintOccupancyTotals(rows, cols, grandTotal);
        }

        // Рядок підсумків по стовпцях
        static void PrintOccupancyTotals(int rows, int cols, int grandTotal)
        {
            Console.Write($"{"Разом",-12}");
            for (int j = 1; j <= cols; j++)
            {
                int colSum = CalculateColumnSum(rows, j);
                Console.Write($"{colSum,8}");
            }
            Console.WriteLine($"{grandTotal,8}");
        }

        // Додає значення в кінець масиву; лічильник змінюється через ref
        static void AddValue(double[] values, ref int count, double newValue, int capacity)
        {
            if (count == capacity)
            {
                Console.WriteLine("Місця більше немає.");
                return;
            }
            values[count] = newValue;
            count++;
        }

        // Виводить значення до count таблицею, з клавіатури нічого не читає
        static void ShowValues(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Набір порожній.");
                return;
            }

            Console.WriteLine($"{"№",4}{"Ціна, грн",14}");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{i + 1,4}{values[i],14:F2}");
            }
            Console.WriteLine("Записів: " + count);
        }

        // Повертає найбільше значення та його номер через два out-параметри
        static void FindMax(double[] values, int count, out double max, out int maxNumber)
        {
            if (count == 0)
            {
                max = 0;
                maxNumber = 0; // 0 означає «немає»
                return;
            }

            max = values[0];
            maxNumber = 1;
            for (int i = 1; i < count; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                    maxNumber = i + 1;
                }
            }
        }

        // Повертає суму та середнє через два out-параметри
        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }
            average = count > 0 ? sum / count : 0;
        }

        static void ShowStatistics(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Набір порожній, статистики немає.");
                return;
            }

            Statistics(values, count, out double sum, out double average);
            FindMax(values, count, out double max, out int maxNumber);

            Console.WriteLine("Записів: " + count);
            Console.WriteLine($"Сума: {sum:F2} грн");
            Console.WriteLine($"Середня ціна: {average:F2} грн");
            Console.WriteLine($"Найбільша ціна: {max:F2} грн (№ {maxNumber})");
        }

        // Значиме передається копією: зміна видна тільки всередині
        static void IncreasePrice(int price)
        {
            price = price + 100;
        }

        // Масив передається посиланням: змінюється той самий масив
        static void IncreaseFirstPrice(double[] prices)
        {
            prices[0] = prices[0] + 100;
        }

        static void DemoValueVsReference()
        {
            PrintTitle("Дослід: значення і посилання");

            int price = 50;
            double[] prices = { 50, 70, 90 };

            Console.WriteLine("До виклику:    price = " + price + ", prices[0] = " + prices[0]);
            IncreasePrice(price);
            IncreaseFirstPrice(prices);
            Console.WriteLine("Після виклику: price = " + price + ", prices[0] = " + prices[0]);

            Console.WriteLine();
            Console.WriteLine("price не змінився: метод отримав копію значення.");
            Console.WriteLine("prices[0] змінився: метод отримав адресу того самого масиву.");
        }

        // Видаляє елемент і зсуває решту вліво; лічильник змінюється через ref
        static void RemoveAt(double[] values, ref int count, int index)
        {
            if (index < 0 || index >= count)
            {
                Console.WriteLine("Помилка! Немає елемента з таким номером.");
                return;
            }

            for (int i = index; i < count - 1; i++)
            {
                values[i] = values[i + 1];
            }
            count--;
            values[count] = 0; // звільнене місце обнуляємо
            Console.WriteLine("Елемент видалено.");
        }
    }
}
