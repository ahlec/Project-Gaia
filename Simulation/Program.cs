using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Simulation
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (GlobalSettings.DevelopmentMode)
            {
                Simulation simulation = new Simulation();
                simulation.Run();
                return;
            }

            try
            {
                Simulation simulation = new Simulation();
                simulation.Run();
            }
            catch (Exception e)
            {
                try
                {
                    if (!Directory.Exists("error_logs"))
                        Directory.CreateDirectory("error_logs");
                    string errorFilename = @"error_logs\error-" + DateTime.Now.ToFileTime() + ".txt";
                    StreamWriter writer = new StreamWriter(errorFilename, true, System.Text.Encoding.ASCII);
                    writer.WriteLine("Date: " + DateTime.Now.ToLongDateString() + "\n");
                    writer.WriteLine("Time: " + DateTime.Now.ToLongTimeString() + "\n\n");
                    writer.WriteLine(e.ToString());
                    writer.Close();
                } catch(Exception secondException)
                {
                    try
                    {
                        MessageBox(new IntPtr(0), "Encountered an error while running, and was unable to " +
                            "save to an error log.\n\nInitial Exception: " + e.ToString() + "\n\nError log exception: " +
                            secondException.ToString(), "Error", 0);
                    }
                    catch (Exception criticalError)
                    {
                        criticalError.ToString();
                    }
                    secondException.ToString();
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hwnd, String text, String caption, uint type);
    }
}

