namespace Apocalypse.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var app = new Application())
            {
                app.Run();
            }
        }
    }
}
