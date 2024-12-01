using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Ошибка: укажите два пути к файлам в аргументах командной строки.");
                return;
            }
            string file1Path = args[0];
            string file2Path = args[1];

            if (!File.Exists(file1Path) || !File.Exists(file2Path))
            {
                Console.WriteLine("Ошибка: Один или оба файла не найдены.");
                return;
            }

            IReadOnlyStream inputStream1 = GetInputStream(file1Path);
            IReadOnlyStream inputStream2 = GetInputStream(file2Path);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            Console.WriteLine("Согласные символы в 1 файле:");
            //Console.WriteLine("\n");
            PrintStatistic(singleLetterStats);
            Console.WriteLine("\nГласные символы во 2 файле:");
            PrintStatistic(doubleLetterStats);

            // TODO[X] : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("\nНажмите любую клавишу для завершения программы...");
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var stats = new List<LetterStats>(); // Коллекция для статистики

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (char.IsLetter(c)) // Если символ - буква
                {
                    IncStatistic(stats, c); // Увеличиваем статистику
                }
                // TODO[X] : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            }

            //return ???;
            return stats;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var stats = new List<LetterStats>(); // Коллекция для статистики

            stream.ResetPositionToStart(); // Начало чтения

            char? previousChar = null; // Храним предыдущий символ

            while (!stream.IsEof)
            {
                char c = char.ToLower(stream.ReadNextChar()); // Приводим символ к нижнему регистру

                if (char.IsLetter(c)) // Если символ - буква
                {
                    if (previousChar.HasValue && previousChar.Value == c)
                    {
                        
                        IncStatistic(stats, c); // Если пара одинаковая, увеличиваем статистику
                    }

                    previousChar = c; // Обновляем предыдущий символ
                }
                else
                {
                    previousChar = null; // Сбрасываем, если не буква
                }
            }

            /*Console.WriteLine("Пары для второго файла:");
            foreach (var stat in stats)
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
            }*/

            return stats;
        }

         private static void IncStatistic(IList<LetterStats> stats, char letter)
 {
     // Проверяем, существует ли буква в списке
     int index = stats.ToList().FindIndex(s => s.Letter == letter.ToString());

     if (index >= 0)
     {
         // Увеличиваем счётчик для найденной буквы
         var stat = stats[index];
         stat.Count++;
         stats[index] = stat;
     }
     else
     {
         // Добавляем новую запись, если буква отсутствует
         stats.Add(new LetterStats { Letter = letter.ToString(), Count = 1 });
     }
 }


        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>

        public enum CharType
        {
            // Гласные
            Vowel,
            // Согласные
            Consonants
        }

        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            char[] vowels = { 'A', 'E', 'I', 'O', 'U', 'Y', 'a', 'e', 'i', 'o', 'u', 'y' };

            for (int i = letters.Count - 1; i >= 0; i--) // Идём с конца, чтобы безопасно удалять
            {
                string letter = letters[i].Letter.ToString(); // Получаем строку
                if (string.IsNullOrEmpty(letter)) // Если пусто, пропускаем
                    continue;

                char firstChar = letter[0]; // Берём первый символ

                if (charType == CharType.Vowel && vowels.Contains(firstChar))
                {
                    letters.RemoveAt(i); //Избавляемся от гласных
                }
                else if (charType == CharType.Consonants && !vowels.Contains(firstChar))
                {
                    letters.RemoveAt(i); //А тут от согласных
                }
            }
        }


        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>

        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sortedLetters = letters.OrderBy(l => l.Letter);

            int total = 0; // Общее количество букв

            foreach (var letterStat in sortedLetters)
            {
                Console.WriteLine($"{letterStat.Letter} : {letterStat.Count}"); // Вывод статистики
                total += letterStat.Count; // Увеличиваем общее количество
            }

            // Вывод итоговой строки
            Console.WriteLine($"ИТОГО: {total}");
            // TODO[X] : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
        }
    }
}
