namespace AssistenciaTecnica.Domain.Models;

public sealed class Tecnico
{
    public string Matricula { get; }
    public string Nome { get; }
    public string Especialidade { get; private set; }
    public bool Ativo { get; private set; }

    public Tecnico(string matricula, string nome, string especialidade, bool ativo = true)
    {
        Matricula = Validacao.Texto(matricula, "a matrícula");
        Nome = Validacao.Texto(nome, "o nome do técnico");
        Especialidade = Validacao.Texto(especialidade, "a especialidade");
        Ativo = ativo;
    }

    public void AlterarEspecialidade(string especialidade) =>
        Especialidade = Validacao.Texto(especialidade, "a especialidade");

    public void DefinirDisponibilidade(bool ativo) => Ativo = ativo;
    public override string ToString() => $"{Nome} · {Especialidade}";
}
