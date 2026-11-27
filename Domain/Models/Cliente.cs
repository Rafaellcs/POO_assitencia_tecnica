using System.Net.Mail;
using AssistenciaTecnica.Domain.Exceptions;

namespace AssistenciaTecnica.Domain.Models;

public sealed class Cliente
{
    private readonly List<Computador> _equipamentos = [];
    public int Id { get; }
    public string Nome { get; }
    public string Cpf { get; }
    public string Telefone { get; private set; } = "";
    public string Email { get; private set; } = "";
    public IReadOnlyList<Computador> Equipamentos => _equipamentos.AsReadOnly();

    public Cliente(int id, string nome, string cpf, string telefone, string email)
    {
        Id = Validacao.Id(id);
        Nome = Validacao.Texto(nome, "o nome do cliente");
        Cpf = NormalizarCpf(cpf);
        AtualizarContato(telefone, email);
    }

    public static string NormalizarCpf(string cpf)
    {
        string digitos = Validacao.Texto(cpf, "o CPF").Replace(".", "").Replace("-", "");
        if (digitos.Length != 11 || !digitos.All(char.IsAsciiDigit) || digitos.All(c => c == digitos[0]))
            throw new DominioException("CPF inválido.");
        for (int tamanho = 9; tamanho <= 10; tamanho++)
        {
            int soma = 0;
            for (int i = 0; i < tamanho; i++) soma += (digitos[i] - '0') * (tamanho + 1 - i);
            int resto = soma % 11;
            if (digitos[tamanho] - '0' != (resto < 2 ? 0 : 11 - resto))
                throw new DominioException("CPF inválido.");
        }
        return digitos;
    }

    public void AtualizarContato(string telefone, string email)
    {
        string numero = Validacao.Texto(telefone, "o telefone");
        string endereco = Validacao.Texto(email, "o e-mail");
        if (!MailAddress.TryCreate(endereco, out var contato) || contato.Address != endereco)
            throw new DominioException("Informe um endereço de e-mail válido.");
        Telefone = numero;
        Email = endereco;
    }

    internal Computador AdicionarComputador(string marca, string modelo, string serie, string tipo)
    {
        var computador = new Computador(this, marca, modelo, serie, tipo);
        if (_equipamentos.Any(c => string.Equals(c.NumeroSerie, computador.NumeroSerie, StringComparison.OrdinalIgnoreCase)))
            throw new DominioException("Este número de série já pertence ao cliente.");
        _equipamentos.Add(computador);
        return computador;
    }

    public override string ToString() => $"{Nome} · {Cpf}";
}
