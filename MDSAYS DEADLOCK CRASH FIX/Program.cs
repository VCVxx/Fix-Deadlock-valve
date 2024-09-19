using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    private static bool isProcessRunningLastState = false;
    private static string currentPowerScheme = string.Empty;

    static void Main()
    {
        // Вывод информации об авторе и задержка
        Console.WriteLine("-----------------------");
        Console.WriteLine("Автор недорфикса MDSAYS");
        Console.WriteLine("ТГ https://t.me/MDsays");
        Console.WriteLine("-----------------------");
        Thread.Sleep(2000);

        // Запускаем project9.exe, если он не запущен
        StartProcessIfNotRunning("project9.exe");

        while (true)
        {
            bool isProcessRunning = IsProcessRunning("project9.exe");
            string newPowerScheme = GetCurrentPowerScheme();

            if (isProcessRunning != isProcessRunningLastState)
            {
                if (isProcessRunning)
                {
                    Console.WriteLine("Процесс запущен");

                    if (newPowerScheme.Equals("Balanced", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Переключаем на экономичный режим");
                        SetPowerScheme("SCHEME_MAX");
                    }
                }
                else
                {
                    Console.WriteLine("Процесс не запущен");

                    if (!newPowerScheme.Equals("Balanced", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Переключаем на сбалансированный режим");
                        SetPowerScheme("SCHEME_BALANCED");
                    }
                    else
                    {
                        Console.WriteLine("Режим уже сбалансированный, действие не требуется");
                    }
                }

                // Обновляем состояние последнего процесса
                isProcessRunningLastState = isProcessRunning;
            }
            else if (!newPowerScheme.Equals(currentPowerScheme, StringComparison.OrdinalIgnoreCase))
            {
                // Если текущий режим питания изменился, сообщаем об этом
                Console.WriteLine($"Режим питания изменился на {newPowerScheme}");
                currentPowerScheme = newPowerScheme;

                // Проверяем, если режим питания стал сбалансированным и процесс не запущен
                if (newPowerScheme.Equals("Balanced", StringComparison.OrdinalIgnoreCase) && !isProcessRunning)
                {
                    Thread.Sleep(100);
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("Наигрался руина?)))");
                    Console.WriteLine("-----------------------");
                    Thread.Sleep(500);
                    Console.WriteLine("Отключаюсь");
                    Thread.Sleep(2000);
                    break; // Завершаем выполнение программы
                }
            }

            // Спим 5 секунд
            Thread.Sleep(5000);
        }
    }

    static void StartProcessIfNotRunning(string processName)
    {
        if (!IsProcessRunning(processName))
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, processName);
            if (File.Exists(exePath))
            {
                Console.WriteLine($"Запускаем {processName}");
                Process.Start(exePath);
            }
            else
            {
                Console.WriteLine($"Файл {processName} не найден в директории {AppDomain.CurrentDomain.BaseDirectory}");
            }
        }
    }

    static bool IsProcessRunning(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName.Replace(".exe", ""));
        return processes.Length > 0;
    }

    static string GetCurrentPowerScheme()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "powercfg",
            Arguments = "/query SCHEME_CURRENT",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(psi))
        {
            using (var reader = process.StandardOutput)
            {
                string output = reader.ReadToEnd();
                if (output.Contains("Balanced", StringComparison.OrdinalIgnoreCase))
                    return "Balanced";
                else if (output.Contains("Power saver", StringComparison.OrdinalIgnoreCase))
                    return "Power saver";
                else if (output.Contains("High performance", StringComparison.OrdinalIgnoreCase))
                    return "High performance";
                else
                    return "Unknown";
            }
        }
    }

    static void SetPowerScheme(string schemeGuid)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "powercfg",
            Arguments = $"/setactive {schemeGuid}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(psi))
        {
            process.WaitForExit();
        }
    }
}
