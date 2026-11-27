using System.Globalization;
using AssistenciaTecnica.UI;

namespace AssistenciaTecnica;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        ApplicationConfiguration.Initialize();
        Application.Run(new Principal());
    }
}
