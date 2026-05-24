using System;

class Program
{
    // Счётчик вызовов подынтегральной функции
    static int calls = 0;

    // Подынтегральная функция f(x), заданная по варианту
    static double F(double x)
    {
        calls++;
        return (2.5 * x * x - 0.1) / (Math.Log(x) + 1.0);
    }

    // Метод правых прямоугольников
    static double RectRight(double a, double b, int n)
    {
        double h = (b - a) / n;
        double sum = 0.0;

        for (int k = 1; k <= n; k++)
        {
            double xk = a + k * h;
            sum += F(xk);
        }

        return h * sum;
    }

    // Метод трапеций
    static double Trapezoid(double a, double b, int n)
    {
        double h = (b - a) / n;
        double sum = 0.0;

        for (int k = 1; k < n; k++)
        {
            double xk = a + k * h;
            sum += F(xk);
        }

        return h * ((F(a) + F(b)) / 2.0 + sum);
    }

    // Метод Симпсона (n должно быть чётным)
    static double Simpson(double a, double b, int n)
    {
        if (n % 2 != 0)
            throw new Exception("Для метода Симпсона n должно быть чётным.");

        double h = (b - a) / n;
        double sumOdd = 0.0;
        double sumEven = 0.0;

        for (int k = 1; k < n; k++)
        {
            double xk = a + k * h;

            if (k % 2 == 1)
                sumOdd += F(xk);   // нечётные индексы — коэффициент 4
            else
                sumEven += F(xk);  // чётные — коэффициент 2
        }

        return (h / 3.0) * (F(a) + F(b) + 4.0 * sumOdd + 2.0 * sumEven);
    }

    // Адаптивный выбор шага по правилу Рунге
    static double RungeAdaptive(
        Func<double, double, int, double> method,
        double a, double b, double eps, int p,
        out double h, out int n)
    {
        n = 4; // стартовое разбиение
        int maxN = 1_000_000;

        while (true)
        {
            double I1 = method(a, b, n);       // интеграл на сетке n
            double I2 = method(a, b, 2 * n);   // интеграл на сетке 2n

            // Оценка погрешности по правилу Рунге
            double delta = Math.Abs(I2 - I1) / (Math.Pow(2, p) - 1.0);

            // Если точность достигнута — возвращаем результат
            if (delta <= eps)
            {
                h = (b - a) / (2.0 * n);
                return I2;
            }

            // Иначе увеличиваем число разбиений
            n *= 2;
            if (n > maxN)
                throw new Exception("Не удалось достичь заданной точности.");
        }
    }

    static void Main()
    {
        Console.WriteLine("Численное интегрирование");

        // Ввод пределов интегрирования и двух значений eps
        Console.Write("Введите a: ");
        double a = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите b: ");
        double b = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите eps1: ");
        double eps1 = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите eps2: ");
        double eps2 = double.Parse(Console.ReadLine().Replace('.', ','));

        double[] epsValues = { eps1, eps2 };

        while (true)
        {
            // Выбор метода интегрирования
            Console.WriteLine("\nВыберите метод:");
            Console.WriteLine("1 — Правые прямоугольники");
            Console.WriteLine("2 — Трапеций");
            Console.WriteLine("3 — Симпсона");
            Console.Write("Ваш выбор: ");

            int choice = int.Parse(Console.ReadLine());

            Func<double, double, int, double> method;
            int p; // порядок точности метода

            switch (choice)
            {
                case 1:
                    method = RectRight;
                    p = 2;
                    break;

                case 2:
                    method = Trapezoid;
                    p = 2;
                    break;

                case 3:
                    method = Simpson;
                    p = 4;
                    break;

                default:
                    Console.WriteLine("Неверный выбор.");
                    continue;
            }

            // Выполняем расчёты сразу для двух eps
            foreach (double eps in epsValues)
            {
                Console.WriteLine($"\n==============================");
                Console.WriteLine($"   Расчёты для eps = {eps}");
                Console.WriteLine("==============================");

                calls = 0; // сброс счётчика вызовов функции

                try
                {
                    double result = RungeAdaptive(method, a, b, eps, p, out double h, out int n);

                    // Вывод результатов
                    Console.WriteLine("\n--- Результаты ---");
                    Console.WriteLine($"Интеграл = {result:G17}");
                    Console.WriteLine($"Шаг h = {h:G17}");
                    Console.WriteLine($"Разбиений n = {n}");
                    Console.WriteLine($"Вызовов f(x) = {calls}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            // Возможность выбрать другой метод
            Console.WriteLine("\n1 — выбрать другой метод");
            Console.WriteLine("0 — выйти");
            Console.Write("Ваш выбор: ");

            int next = int.Parse(Console.ReadLine());

            if (next == 0)
            {
                Console.WriteLine("Программа завершена.");
                return;
            }
        }
    }
}
