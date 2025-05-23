using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace Frontend_HomeBudget
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try {
            var app = new Application();
            View view = new View();
            app.Run();
        }
        catch (Exception ex)
        {
            File.WriteAllText(@"C:\Users\Public\budget_wpf_crash.txt", ex.ToString());
            throw;
        }
}
    }
}
