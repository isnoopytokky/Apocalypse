using System.Windows.Forms;
using Apocalypse.WindowsForms.Forms;

namespace Apocalypse.WindowsForms
{
    static class Program
    {
        public static void Main(string[] args)
        {
            using (var mainForm = new MainForm())
            {
                Application.Run(mainForm);
            }
        }
    }
}
