using AssistenciaTecnica.Domain.Exceptions;

namespace AssistenciaTecnica.Domain.Models;

internal static class Validacao
{
    public static string Texto(string? valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException($"Informe {campo}.");
        return valor.Trim();
    }

    public static int Id(int valor)
    {
        if (valor < 1) throw new DominioException("O identificador deve ser positivo.");
        return valor;
    }

    public static decimal NaoNegativo(decimal valor, string campo)
    {
        if (valor < 0) throw new DominioException($"{campo} deve ser maior ou igual a zero.");
        return valor;
    }

    public static decimal Percentual(decimal valor)
    {
        if (valor is < 0 or > 100) throw new DominioException("O percentual deve estar entre 0 e 100.");
        return valor;
    }

    public static decimal Moeda(decimal valor) => decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
}
