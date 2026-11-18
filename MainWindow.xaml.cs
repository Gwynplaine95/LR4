using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinqPlinqTask
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtN.Text, out int n) || n <= 0)
            {
                MessageBox.Show("Введите корректное положительное число N.");
                return;
            }

            TxtOutput.Text = "Генерация данных и выполнение запросов...";
            BtnRun.IsEnabled = false;

            // Запускаем в отдельном потоке, чтобы не вешать UI
            string result = await Task.Run(() => ProcessData(n));

            TxtOutput.Text = result;
            BtnRun.IsEnabled = true;
        }

        private string ProcessData(int n)
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            // 1. Генерация массива строк
            string[] data = new string[n];
            for (int i = 0; i < n; i++)
            {
                // Генерируем числа длиной от 1 до 7 цифр (чтобы точно были числа > 3 цифр)
                int length = rnd.Next(1, 8);
                char[] digits = new char[length];
                for (int j = 0; j < length; j++)
                {
                    digits[j] = (char)('0' + rnd.Next(10));
                }

                // Убираем ведущие нули, если они появились, чтобы не ломать парсинг
                if (digits[0] == '0') digits[0] = '1';

                data[i] = new string(digits);
            }

            sb.AppendLine($"Сгенерирован массив из {n} строк.");
            sb.AppendLine(new string('-', 50));

            Stopwatch sw = new Stopwatch();

            // --- Запрос 1: Четные числа ---
            sw.Restart();
            var q1Linq = data.Select(int.Parse).Where(x => x % 2 == 0).OrderByDescending(x => x).ToList();
            sw.Stop();
            long q1LinqTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var q1Plinq = data.AsParallel().Select(int.Parse).Where(x => x % 2 == 0).OrderByDescending(x => x).ToList();
            sw.Stop();
            long q1PlinqTime = sw.ElapsedMilliseconds;

            AppendResult(sb, "1. Четные числа", q1LinqTime, q1PlinqTime, q1Linq);

            // --- Запрос 2: Нечетные числа ---
            sw.Restart();
            var q2Linq = data.Select(int.Parse).Where(x => x % 2 != 0).OrderByDescending(x => x).ToList();
            sw.Stop();
            long q2LinqTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var q2Plinq = data.AsParallel().Select(int.Parse).Where(x => x % 2 != 0).OrderByDescending(x => x).ToList();
            sw.Stop();
            long q2PlinqTime = sw.ElapsedMilliseconds;

            AppendResult(sb, "2. Нечетные числа", q2LinqTime, q2PlinqTime, q2Linq);

            // --- Запрос 3: Сумма второй и предпоследней цифры = 6 (только для чисел > 3 цифр) ---
            // Условие: s.Length > 3. Вторая цифра: s[1]. Предпоследняя: s[s.Length - 2]
            sw.Restart();
            var q3Linq = data.Where(s => s.Length > 3 && (s[1] - '0') + (s[s.Length - 2] - '0') == 6)
                             .Select(int.Parse)
                             .OrderByDescending(x => x)
                             .ToList();
            sw.Stop();
            long q3LinqTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var q3Plinq = data.AsParallel()
                              .Where(s => s.Length > 3 && (s[1] - '0') + (s[s.Length - 2] - '0') == 6)
                              .Select(int.Parse)
                              .OrderByDescending(x => x)
                              .ToList();
            sw.Stop();
            long q3PlinqTime = sw.ElapsedMilliseconds;

            AppendResult(sb, "3. Числа > 3 цифр, сумма 2-й и предпоследней цифры = 6", q3LinqTime, q3PlinqTime, q3Linq);

            // --- Запрос 4: Сумма всех цифр = 13 ---
            sw.Restart();
            var q4Linq = data.Where(s => s.Sum(c => c - '0') == 13)
                             .Select(int.Parse)
                             .OrderByDescending(x => x)
                             .ToList();
            sw.Stop();
            long q4LinqTime = sw.ElapsedMilliseconds;

            sw.Restart();
            var q4Plinq = data.AsParallel()
                              .Where(s => s.Sum(c => c - '0') == 13)
                              .Select(int.Parse)
                              .OrderByDescending(x => x)
                              .ToList();
            sw.Stop();
            long q4PlinqTime = sw.ElapsedMilliseconds;

            AppendResult(sb, "4. Сумма всех цифр = 13", q4LinqTime, q4PlinqTime, q4Linq);

            return sb.ToString();
        }

        // Вспомогательный метод для красивого вывода
        private void AppendResult(StringBuilder sb, string title, long linqTime, long plinqTime, List<int> results)
        {
            sb.AppendLine($"=== {title} ===");
            sb.AppendLine($"Время LINQ:  {linqTime} мс");
            sb.AppendLine($"Время PLINQ: {plinqTime} мс");
            sb.AppendLine($"Найдено элементов: {results.Count}");

            // Выводим первые 50 элементов, чтобы не перегружать текстовое поле
            sb.AppendLine("Результаты (первые 50, по убыванию):");
            sb.AppendLine(string.Join(", ", results.Take(50)) + (results.Count > 50 ? " ..." : ""));
            sb.AppendLine(new string('-', 50));
        }
    }
}