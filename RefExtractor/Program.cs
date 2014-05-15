using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

using RefExtractor.Data;
using RefExtractor.Processing;


namespace RefExtractor
{
    class Program
    {
        static bool _processCompleted;

        static void Main(string[] args)
        {
            //Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener(DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + "_log.txt"));
            Trace.AutoFlush = true;

            Console.WindowWidth = 120;
            Console.WindowHeight = 40;


            Console.WriteLine("****************************** Экстрактор ссылок ******************************");
            Console.WriteLine();
            Console.WriteLine("Cтрока для подключения к БД и другие настройки берутся из конфигурационного файла.");
            Console.WriteLine("Полный лог процесса будет сохранен в файле.");
            Console.WriteLine();            
            Console.WriteLine("Для остановки процесса нажмите Escape в любой момент.");
            Console.WriteLine("Для запуска процесса нажмите любую клавишу.");
            Console.ReadKey(true);

            try
            {
                var process = StartProcess();

                while (!_processCompleted)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            process.Stop();
                            break;
                        }
                    }

                    System.Threading.Thread.Sleep(1000);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось запустить процесс: " + ex.ToString());
            }

            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey(true);
            Console.WriteLine("Чао");

        }

        static MainProcess StartProcess()
        {
            _processCompleted = false;

            string connString = ConfigurationManager.ConnectionStrings["SqlRepository"].ConnectionString;
            int parallelRequests = Convert.ToInt32(ConfigurationManager.AppSettings["ParallelRequests"]);

            var repository = new Data.Repositories.SqlRepository(connString); /* new Data.Repositories.FakeRepository; */

            MainProcess process = new MainProcess(repository, parallelRequests);

            //process.PageProcessStarted += (page) => Console.WriteLine("Началась обработка страницы " + page.Url);
            process.PageProcessed += (page) => Console.WriteLine("Завершена обработка страницы " + page.Url);
            process.ProcessCompleted += () => { Console.WriteLine("Процесс завершен."); _processCompleted = true; };
            process.ProcessStarted += () => Console.WriteLine("Процесс успешно запущен."); 

            process.Start();

            return process;
        }
    }
}
