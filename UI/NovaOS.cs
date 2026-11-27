using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models;
using AssistenciaTecnica.Services;

namespace AssistenciaTecnica.UI;

internal sealed class NovaOS : Tela
{
    private readonly Oficina _oficina;
    private readonly ComboBox _clientes = Lista(), _maquinas = Lista(), _tecnicos = Lista();
    private readonly TextBox _cpf = new(), _defeito = new() { Multiline = true, Height = 64 };
    public OrdemDeServico? Ordem { get; private set; }

    public NovaOS(Oficina oficina) : base("Nova ordem de serviço", 640, 600)
    {
        _oficina = oficina;
        Campo("Localizar por CPF", _cpf);
        Acoes(Botao("Buscar CPF", () =>
        {
            _clientes.SelectedIndex = -1;
            _clientes.SelectedItem = oficina.BuscarCliente(_cpf.Text) ?? throw new DominioException("Cliente não encontrado. Use Cadastrar cliente.");
        }), Botao("Cadastrar cliente", NovoCliente));
        Campo("Cliente", _clientes);
        _clientes.SelectedIndexChanged += (_, _) => AtualizarEquipamentos();
        Preencher(_clientes, oficina.Clientes);
        Campo("Equipamento", _maquinas);
        Acoes(Botao("Cadastrar equipamento", NovoEquipamento));
        Campo("Técnico responsável", _tecnicos);
        Preencher(_tecnicos, oficina.Tecnicos.Where(t => t.Ativo));
        Campo("Defeito relatado nesta visita", _defeito);
        Acoes(Botao("Abrir OS", () =>
        {
            Ordem = oficina.AbrirOS(Escolhido<Computador>(_maquinas, "o equipamento"),
                Escolhido<Tecnico>(_tecnicos, "o técnico"), _defeito.Text);
            DialogResult = DialogResult.OK;
        }));
    }

    private void AtualizarEquipamentos() =>
        Preencher(_maquinas, (_clientes.SelectedItem as Cliente)?.Equipamentos ?? []);

    private void NovoCliente()
    {
        using var cadastro = new Cadastro("Cadastrar cliente");
        var nome = cadastro.Entrada("Nome");
        var cpf = cadastro.Entrada("CPF", _cpf.Text);
        var telefone = cadastro.Entrada("Telefone");
        var email = cadastro.Entrada("E-mail");
        Cliente? cliente = null;
        cadastro.AoSalvar(() => cliente = _oficina.CadastrarCliente(nome.Text, cpf.Text, telefone.Text, email.Text));
        if (cadastro.ShowDialog(this) != DialogResult.OK) return;
        Preencher(_clientes, _oficina.Clientes);
        _clientes.SelectedItem = cliente;
    }

    private void NovoEquipamento()
    {
        var cliente = Escolhido<Cliente>(_clientes, "o cliente");
        using var cadastro = new Cadastro("Cadastrar equipamento");
        var marca = cadastro.Entrada("Marca");
        var modelo = cadastro.Entrada("Modelo");
        var serie = cadastro.Entrada("Número de série");
        var tipo = cadastro.Entrada("Tipo (desktop, notebook ou estação de trabalho)");
        Computador? computador = null;
        cadastro.AoSalvar(() => computador = _oficina.CadastrarEquipamento(cliente, marca.Text, modelo.Text, serie.Text, tipo.Text));
        if (cadastro.ShowDialog(this) != DialogResult.OK) return;
        AtualizarEquipamentos();
        _maquinas.SelectedItem = computador;
    }

    private sealed class Cadastro(string titulo) : Tela(titulo, 480, 380)
    {
        public TextBox Entrada(string tituloCampo, string inicial = "")
        {
            var caixa = new TextBox { Text = inicial };
            Campo(tituloCampo, caixa);
            return caixa;
        }

        public void AoSalvar(Action salvar) => Acoes(Botao("Salvar", () =>
        {
            salvar();
            DialogResult = DialogResult.OK;
        }));
    }
}
