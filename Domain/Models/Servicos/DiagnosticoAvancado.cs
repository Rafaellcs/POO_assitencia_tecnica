namespace AssistenciaTecnica.Domain.Models.Servicos;

public sealed class DiagnosticoAvancado : Servico
{
    public decimal Horas { get; }
    public decimal TaxaHora { get; }
    public bool OrcamentoAprovado { get; }

    internal DiagnosticoAvancado(int id, decimal horas, decimal taxaHora, bool aprovado)
        : base(id, "Diagnóstico avançado", 0)
    {
        Horas = Validacao.NaoNegativo(horas, "Horas de bancada");
        TaxaHora = Validacao.NaoNegativo(taxaHora, "Taxa por hora");
        OrcamentoAprovado = aprovado;
    }

    public override decimal CalcularValor() => Validacao.Moeda(Horas * TaxaHora * (OrcamentoAprovado ? 0.5m : 1m));
}
