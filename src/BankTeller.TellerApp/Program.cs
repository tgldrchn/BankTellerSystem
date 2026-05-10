namespace BankTeller.TellerApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.MainForm());
    }
}