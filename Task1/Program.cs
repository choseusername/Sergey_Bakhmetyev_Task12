using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "disposable_task_file.txt";

            try
            {
                IEnumerable<string> data = File.ReadAllLines(filename)
                    .Select(x =>
                    {
                        int i = int.Parse(x);
                        return Convert.ToString(i * i);
                    });

                File.WriteAllLines(filename, data);

                Console.WriteLine("Программа завершила свою работу успешно.");
            }
            catch(Exception e)
            {
                Console.WriteLine("В ходе работы программы произошла ошибка:");
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
