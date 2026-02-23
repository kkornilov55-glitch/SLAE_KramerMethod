using System;
using System.IO;

namespace lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int max_n = 6;
            const int max_kof = 5;
            try
            {
                //Открытие файла
                StreamReader stream = new StreamReader("input.txt");
                //Считывание первой строки файла (Порядок квадратной матрицы)

                if (!int.TryParse(stream.ReadLine(), out int n) || n > max_n || n < 1)
                    Error("Недопустимое число уравнений");

                //Создание матрицы коэффициентов
                int[,] massA = new int[n, n];
                //Создание матрицы свободных членов
                int[] massB = new int[n];


                string[] Buf = new string[n]; //Инициализируем массив для разделения чисел в строке
                string str = ""; //Инициализируем переменную для временного хранения считываемых строк
                

                //Заполнение матрицы коэффициентов
                for (int i = 0; i < n; i++) //Для каждой строки
                {
                    str = stream.ReadLine(); //Считываем текущую строку
                    Buf = str.Split(' '); //Разделяем числа в строке (Пробелом)

                    for (int j = 0; j < n; j++)
                    {
                        if (Math.Abs(int.Parse(Buf[j])) <= max_kof)
                            massA[i, j] = int.Parse(Buf[j]); //Добавляем в текущую строку числа
                        else
                            Error("Коэффициент слагаемого выходит за границы допустимого");
                    }
                }

                //Заполнение матрицы свободных членов
                for (int i = 0; i < n; i++)
                {
                    massB[i] = int.Parse(stream.ReadLine());
                }
                //Закрытие файла
                stream.Close();


                string SLAU = ""; //Инициализация текстовой переменной для вывода СЛАУ
                bool zero_flag = false; //Инициализация флага нуливой строки

                for (int i = 0;i < n; i++) //Для каждого уравнения системы
                {
                    for (int j = 0; j < n; j++) //Проверка на 0 в левой части
                    {
                        if (massA[i,j] != 0)
                            zero_flag = true;
                            
                    }
                     
                    if (zero_flag) //Если в левой части не ноль
                    {
                        SLAU += massA[i, 0].ToString() + "*x1"; //Первое слагаемое
                        for (int j = 1; j < n; j++) //Для каждого слагаемого
                        {
                            if (massA[i, j] == 0) //Если 0 -> пропускаем
                                continue;
                            else if (massA[i, j] == 1) //Если 1 -> добавляем без коэффициента с плюсом
                                SLAU += $" + x{j + 1}";
                            else if (massA[i, j] == -1) //Если -1 -> добавляем без коэффициента с минусом
                                SLAU += $" - x{j + 1}";
                            else //Во всех остальных случиях
                                if (massA[i, j] > 0) //Если положительный коэффициент -> добавляем слагаемое с плюсом
                                    SLAU += $" + {massA[i, j]}*x{j + 1}";
                            else //Если отрицательный коэффициент -> добавляем слагаемое по модулю с минусом
                                SLAU += $" - {Math.Abs(massA[i, j])}*x{j + 1}";
                        }
                    }
                    else //Если 0
                    {
                        SLAU += "0";
                    }
                    SLAU += $" = {massB[i]}\n";
                }
                Console.WriteLine(SLAU); //Вывод СЛАУ

                int det = Determinant(massA);
                int[] x = new int[n];
                for (int i = 0; i < n; i++)
                {
                    int[,] res = Stolbik_swap(massA, massB, i);
                    int det_new = Determinant(res);
                    x[i] = det_new/det;
                }

                for (int i = 0;i < n;i++)
                {
                    Console.WriteLine($"x{i+1} = {x[i]}");
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка типа данных");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("В текущей дериктории не существует файла с таким именем");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Директория не найдена");
                return;
            }
            catch
            {
                Console.WriteLine("Неизвестная ошибка");
                return;
            }

        }
        static void Error(string msg) //Функция ошибки
        {
            Console.WriteLine(msg);
            Console.Write("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
            Environment.Exit(0);
        }
        static int Determinant(int[,] mas)
        {
            if (mas.Length == 1) //Базовый случай (Если порядок матрицы у которой ищется определитель = 1, находим его без рекурсии)
            {
                //return mas[0, 0] * mas[1, 1] - mas[0, 1] * mas[1, 0];
                return mas[0,0];
            }
            else //Рекурсивный поиск определителя
            {
                int now_len = (int)Math.Sqrt(mas.Length); //Текущий порядок матрицы
                int[,] red_mas = new int[now_len-1, now_len-1]; //Подмассивчик для рекурсивного поиска определителя

                int opred = 0;
                int znak = 1;

                for (int stolbik = 0; stolbik < now_len; stolbik++) //Вычисление миноров
                {
                    //Создание матрицы для минора
                    for (int i = 1; i < now_len; i++) //Для каждой строки текущей матрицы со 2
                    {
                        int tek_stolb = 0; //Инициализация индикатора текущего элемента в строке
                        for (int j = 0; j < now_len; j++) //Для каждого элемента
                        {

                            if (j == stolbik) continue; 
                            red_mas[i - 1, tek_stolb] = mas[i, j];
                            tek_stolb++;
                        }
                    }

                    opred += znak * mas[0,stolbik] * Determinant(red_mas); //Рекурсивно считаем определитель
                    znak = -znak; //Чередуем знаки
                }
                return opred;
            }
        }
        static int[,] Stolbik_swap(int[,] mas, int[] svobodni, int stolbik)
        {
            int n = (int)Math.Sqrt(mas.Length);
            int[,] result = new int[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0;j < n; j++)
                {
                    if (j == stolbik)
                        result[i, j] = svobodni[i];
                    else
                        result[i, j] = mas[i, j];
                }
                
            }
            return result;
        }
    }
}
