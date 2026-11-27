namespace AssistenciaTecnica.Domain.Models.Servicos;

public sealed class Formatacao : Servico
{
    public const decimal TaxaBackup = 30m;
    public const decimal LimiteInclusoGB = 100m;
    public const decimal TamanhoLoteGB = 50m;
    public const decimal TarifaLote = 10m;
    public bool RealizarBackup { get; }
    public decimal VolumeGB { get; }
    public string SistemaOperacional { get; }

    internal Formatacao(int id, decimal maoDeObra, string sistema, bool backup, decimal volumeGB)
        : base(id, "Formatação", maoDeObra)
    {
        SistemaOperacional = Validacao.Texto(sistema, "o sistema operacional");
        VolumeGB = Validacao.NaoNegativo(volumeGB, "Volume do backup");
        RealizarBackup = backup;
    }

    public override decimal CalcularValor()
    {
        if (!RealizarBackup) return Validacao.Moeda(MaoDeObra);
        decimal excedente = Math.Max(0, VolumeGB - LimiteInclusoGB);
        return Validacao.Moeda(MaoDeObra + TaxaBackup + Math.Ceiling(excedente / TamanhoLoteGB) * TarifaLote);
    }
}
