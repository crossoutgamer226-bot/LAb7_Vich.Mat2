using System;

class Program
{
    static int calls = 0;

    static double F(double x)
    {
        calls++;
        return (2.5 * x * x - 0.1) / (Math.Log(x) + 1.0);
    }

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
                sumOdd += F(xk);
            else
                sumEven += F(xk);
        }

        return (h / 3.0) * (F(a) + F(b) + 4.0 * sumOdd + 2.0 * sumEven);
    }

    static double RungeAdaptive(
        Func<double, double, int, double> method,
        double a, double b, double eps, int p,
        out double h, out int n)
    {
        n = 4;
        int maxN = 1_000_000;

        while (true)
        {
            double I1 = method(a, b, n);
            double I2 = method(a, b, 2 * n);

            double delta = Math.Abs(I2 - I1) / (Math.Pow(2, p) - 1.0);

            if (delta <= eps)
            {
                h = (b - a) / (2.0 * n);
                return I2;
            }

            n *= 2;
            if (n > maxN)
                throw new Exception("Не удалось достичь заданной точности.");
        }
    }

    static void Main()
    {
        Console.WriteLine("Численное интегрирование");

        Console.Write("Введите a: ");
        double a = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите b: ");
        double b = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите eps1: ");
        double eps1 = double.Parse(Console.ReadLine().Replace('.', ','));

        Console.Write("Введите eps2: ");
        double eps2 = double.Parse(Console.ReadLine().Replace('.', ','));

        double[] epsValues = { eps1, eps2 };

        foreach (double eps in epsValues)
        {
            Console.WriteLine($"\n==============================");
            Console.WriteLine($"   Расчёты для eps = {eps}");
            Console.WriteLine("==============================");

            while (true)
            {
                Console.WriteLine("\nВыберите метод:");
                Console.WriteLine("1 — Правые прямоугольники");
                Console.WriteLine("2 — Трапеций");
                Console.WriteLine("3 — Симпсона");
                Console.Write("Ваш выбор: ");

                int choice = int.Parse(Console.ReadLine());

                calls = 0;

                double result, h;
                int n;

                try
                {
                    switch (choice)
                    {
                        case 1:
                            result = RungeAdaptive(RectRight, a, b, eps, 2, out h, out n);
                            break;

                        case 2:
                            result = RungeAdaptive(Trapezoid, a, b, eps, 2, out h, out n);
                            break;

                        case 3:
                            result = RungeAdaptive(Simpson, a, b, eps, 4, out h, out n);
                            break;

                        default:
                            Console.WriteLine("Неверный выбор.");
                            continue;
                    }

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

                Console.WriteLine("1 — выбрать другой метод для этой eps");
                Console.WriteLine("2 — перейти к следующей eps");
                Console.WriteLine("0 — выйти");
                Console.Write("Ваш выбор: ");

                int next = int.Parse(Console.ReadLine());

                if (next == 0)
                {
                    Console.WriteLine("Программа завершена:).");
                    return;
                }
                else if (next == 2)
                {
                    break;
                }
            }
        }

        Console.WriteLine("\nВсе расчёты завершены.");
    }
}
