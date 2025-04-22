using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frontend_HomeBudget
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new Application();
            View view = new View();
            app.Run();
        }
    }
}
