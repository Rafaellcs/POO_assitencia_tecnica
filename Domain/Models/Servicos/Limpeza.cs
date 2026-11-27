using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Exceptions;

namespace AssistenciaTecnica.Domain.Models.Servicos;

public sealed class Limpeza : Servico
{
    public TipoPastaTermica Pasta { get; }
    public decimal CustoPasta { get; }
    internal Limpeza(int id, decimal maoDeObra, TipoPastaTermica pasta) : base(id, "Limpeza", maoDeObra)
    {
        Pasta = pasta;
        CustoPasta = pasta switch
        {
            TipoPastaTermica.Comum => 10m,
            TipoPastaTermica.Prata => 25m,
            TipoPastaTermica.Ceramica => 35m,
            _ => throw new DominioException("Tipo de pasta desconhecido.")
        };
    }

    public override decimal CalcularValor() => Validacao.Moeda(MaoDeObra + CustoPasta);
}
