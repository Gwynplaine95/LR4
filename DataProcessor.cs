using System;
using System.Collections.Generic;
using System.Linq;
using LinqPlinqTask;

namespace LinqPlinqTask
{
    public class DataProcessor
    {
        // Генерация данных
        public string[] GenerateData(int n)
        {
            if (n <= 0) return Array.Empty<string>();

            Random rnd = new Random();
            string[] data = new string[n];

            for (int i = 0; i < n; i++)
            {
                int length = rnd.Next(1, 8);
                char[] digits = new char[length];
                for (int j = 0; j < length; j++)
                {
                    digits[j] = (char)('0' + rnd.Next(10));
                }
                if (digits[0] == '0') digits[0] = '1';
                data[i] = new string(digits);
            }
            return data;
        }

        // Запрос 1: Четные числа
        public List<int> GetEvenNumbers(string[] data, bool usePlinq = false)
        {
            var query = usePlinq ? data.AsParallel() : data.AsEnumerable();
            return query.Select(int.Parse)
                        .Where(x => x % 2 == 0)
                        .OrderByDescending(x => x)
                        .ToList();
        }

        // Запрос 2: Нечетные числа
        public List<int> GetOddNumbers(string[] data, bool usePlinq = false)
        {
            var query = usePlinq ? data.AsParallel() : data.AsEnumerable();
            return query.Select(int.Parse)
                        .Where(x => x % 2 != 0)
                        .OrderByDescending(x => x)
                        .ToList();
        }

        // Запрос 3: Сумма второй и предпоследней цифры = 6 (только > 3 цифр)
        public List<int> GetSumSecondAndPreLastEqualsSix(string[] data, bool usePlinq = false)
        {
            var query = usePlinq ? data.AsParallel() : data.AsEnumerable();
            return query.Where(s => s.Length > 3 && (s[1] - '0') + (s[s.Length - 2] - '0') == 6)
                        .Select(int.Parse)
                        .OrderByDescending(x => x)
                        .ToList();
        }

        // Запрос 4: Сумма всех цифр = 13
        public List<int> GetSumAllDigitsEqualsThirteen(string[] data, bool usePlinq = false)
        {
            var query = usePlinq ? data.AsParallel() : data.AsEnumerable();
            return query.Where(s => s.Sum(c => c - '0') == 13)
                        .Select(int.Parse)
                        .OrderByDescending(x => x)
                        .ToList();
        }
    }
}