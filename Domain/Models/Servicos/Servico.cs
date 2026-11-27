namespace AssistenciaTecnica.Domain.Models.Servicos;

public abstract class Servico
{
    public int Id { get; }
    public string Descricao { get; }
    public decimal MaoDeObra { get; }
    public bool Concluido { get; private set; }

    protected Servico(int id, string descricao, decimal maoDeObra)
    {
        Id = Validacao.Id(id);
        Descricao = Validacao.Texto(descricao, "a descrição do serviço");
        MaoDeObra = Validacao.NaoNegativo(maoDeObra, "Mão de obra");
    }

    internal void Concluir() => Concluido = true;
    public abstract decimal CalcularValor();
    public override string ToString() =>
        $"{Id}. {Descricao} — {CalcularValor():C} ({(Concluido ? "concluído" : "pendente")})";
}
