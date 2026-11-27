namespace AssistenciaTecnica.Domain.Models;

public sealed class Computador
{
    public Cliente Proprietario { get; }
    public string Marca { get; }
    public string Modelo { get; }
    public string NumeroSerie { get; }
    public string Tipo { get; }

    internal Computador(Cliente proprietario, string marca, string modelo, string numeroSerie, string tipo)
    {
        Proprietario = proprietario;
        Marca = Validacao.Texto(marca, "a marca");
        Modelo = Validacao.Texto(modelo, "o modelo");
        NumeroSerie = Validacao.Texto(numeroSerie, "o número de série");
        Tipo = Validacao.Texto(tipo, "o tipo de equipamento");
    }

    public override string ToString() => $"{Marca} {Modelo} · {NumeroSerie}";
}
