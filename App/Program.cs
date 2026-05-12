using System;
using Spectre.Console;
using App.UI;

class Program
{
    static void Main()
    {
        var connectionString = "Host=localhost;Database=fuflobank;Username=postgres;Password=postgres";
        
        var app = new ConsoleApplication(connectionString);
        app.Run();
    }
}