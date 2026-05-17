using BankTeller.NumberDisplay.Forms;

namespace BankTeller.NumberDisplay;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new DisplayForm());
    }
}