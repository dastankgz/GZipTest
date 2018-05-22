using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NLog;

namespace GZipTest
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            
            try
            {
                var isValid = ValidateArgs(args);
                if (!isValid)
                    return;

                var factory = new CompressorFactory();
                var compress = args[0].ToLower() == "compress";
                var command = compress
                    ? factory.CreateCompressor(args[1], args[2])
                    : factory.CreateDecompressor(args[1], args[2]);
                var watch = new Stopwatch();

                watch.Start();
                command.Execute();

                while (!command.IsCompleted)
                {
                    Thread.Sleep(10);

                    var time = new DateTime(watch.Elapsed.Ticks).ToString("HH:mm:ss ff");
                    Console.Write('\r' + time);
                }

                watch.Stop();
                Console.WriteLine("\r\nФайл " + (compress ? "сжат" : "расжат") + " успешно");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            LogManager.Flush();
        }

        private static bool ValidateArgs(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                ShowInfo();
                return false;
            }

            if (args[0].ToLower() != "compress" && args[0].ToLower() != "decompress")
            {
                ShowInfo();
                return false;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"Файл {args[1]} отсутствует, пожалуйста, проверьте параметры");
                return false;
            }

            if (File.Exists(args[2]))
            {
                Console.WriteLine($"Файл {args[2]} имеется, пожалуйста, укажите другое название");
                return false;
            }

            return true;
        }

        private static void ShowInfo()
        {
            Console.WriteLine("Пожалуйста, задайте параметры след. образом:");
            Console.WriteLine("GZipTest.exe compress/decompress [имя исходного файла] [имя результирующего файла]");
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Fatal("Unhandled exception: {0}", e.ExceptionObject);
            LogManager.Flush();
        }
    }
}