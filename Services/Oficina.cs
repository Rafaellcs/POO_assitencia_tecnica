using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models;

namespace AssistenciaTecnica.Services;

public sealed class Oficina
{
    private readonly List<Cliente> _clientes = [];
    private readonly List<OrdemDeServico> _ordens = [];
    public IReadOnlyList<Cliente> Clientes => _clientes.AsReadOnly();
    public IReadOnlyList<OrdemDeServico> Ordens => _ordens.AsReadOnly();
    public IReadOnlyList<Tecnico> Tecnicos { get; } = Array.AsReadOnly<Tecnico>([
        new("T01", "Ana", "Software"), new("T02", "Bruno", "Hardware")
    ]);

    public Cliente? BuscarCliente(string cpf)
    {
        string numero = Cliente.NormalizarCpf(cpf);
        return _clientes.Find(c => c.Cpf == numero);
    }

    public Cliente CadastrarCliente(string nome, string cpf, string telefone, string email)
    {
        if (BuscarCliente(cpf) != null) throw new DominioException("CPF já cadastrado. Selecione o cliente existente.");
        var cliente = new Cliente(_clientes.Count + 1, nome, cpf, telefone, email);
        _clientes.Add(cliente);
        return cliente;
    }

    public Computador CadastrarEquipamento(Cliente cliente, string marca, string modelo, string serie, string tipo)
    {
        if (!_clientes.Contains(cliente)) throw new DominioException("Selecione um cliente cadastrado.");
        serie = Validacao.Texto(serie, "o número de série");
        if (_clientes.SelectMany(c => c.Equipamentos).Any(c =>
            string.Equals(c.NumeroSerie, serie, StringComparison.OrdinalIgnoreCase)))
            throw new DominioException("Número de série já cadastrado.");
        return cliente.AdicionarComputador(marca, modelo, serie, tipo);
    }

    public OrdemDeServico AbrirOS(Computador computador, Tecnico tecnico, string defeito)
    {
        if (!_clientes.Any(c => c.Equipamentos.Contains(computador)) || !Tecnicos.Contains(tecnico))
            throw new DominioException("Selecione um equipamento e um técnico cadastrados na oficina.");
        var ordem = new OrdemDeServico(_ordens.Count + 1, computador, tecnico, defeito);
        _ordens.Add(ordem);
        return ordem;
    }
}
