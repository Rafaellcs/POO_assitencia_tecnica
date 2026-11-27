using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Exceptions;

namespace AssistenciaTecnica.Domain.Models.Servicos;

public sealed class ReparoHardware : Servico
{
    public string Componente { get; }
    public decimal CustoPeca { get; }
    public decimal GarantiaPercentual { get; }
    public decimal Fator { get; }

    internal ReparoHardware(int id, decimal maoDeObra, string componente, decimal custoPeca,
        decimal garantia, NivelComplexidadeSolda nivel) : base(id, "Reparo de hardware", maoDeObra)
    {
        Componente = Validacao.Texto(componente, "o componente");
        CustoPeca = Validacao.NaoNegativo(custoPeca, "Custo da peça");
        GarantiaPercentual = Validacao.Percentual(garantia);
        Fator = nivel switch
        {
            NivelComplexidadeSolda.Baixa => 1m,
            NivelComplexidadeSolda.Media => 1.5m,
            NivelComplexidadeSolda.Alta => 2m,
            _ => throw new DominioException("Nível de solda desconhecido.")
        };
    }

    public override decimal CalcularValor() =>
        Validacao.Moeda(MaoDeObra * Fator + CustoPeca + CustoPeca * GarantiaPercentual / 100m);
}
