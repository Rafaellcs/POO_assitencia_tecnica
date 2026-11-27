using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Models;

namespace AssistenciaTecnica.UI;

internal sealed class Lancamento : Tela
{
    private readonly OrdemDeServico _os;
    private readonly ComboBox _tipo = Lista(), _pasta = Lista(), _solda = Lista();
    private readonly NumericUpDown _mao = Numero(80), _volume = Numero(), _peca = Numero(),
        _garantia = Numero(0, 100), _horas = Numero(1), _taxa = Numero(80);
    private readonly TextBox _sistema = new(), _componente = new();
    private readonly CheckBox _backup = new() { Text = "Realizar backup", AutoSize = true },
        _aprovado = new() { Text = "Orçamento aprovado", AutoSize = true };
    private readonly List<(Control Rotulo, Control Controle, Func<bool> Exibir)> _opcoes = [];

    public Lancamento(OrdemDeServico os) : base("Lançar serviço", 520, 570)
    {
        _os = os;
        Campo("Tipo de serviço", _tipo);
        Preencher(_tipo, new[] { "Formatação", "Limpeza", "Reparo de hardware", "Diagnóstico avançado" });
        Preencher(_pasta, Enum.GetValues<TipoPastaTermica>());
        Preencher(_solda, Enum.GetValues<NivelComplexidadeSolda>());
        Opcao("Mão de obra (R$)", _mao, () => _tipo.SelectedIndex != 3);
        Opcao("Sistema operacional", _sistema, () => _tipo.SelectedIndex == 0);
        Opcao("Arquivos", _backup, () => _tipo.SelectedIndex == 0);
        Opcao("Volume do backup (GB)", _volume, () => _tipo.SelectedIndex == 0 && _backup.Checked);
        Opcao("Pasta térmica", _pasta, () => _tipo.SelectedIndex == 1);
        Opcao("Componente", _componente, () => _tipo.SelectedIndex == 2);
        Opcao("Custo da peça (R$)", _peca, () => _tipo.SelectedIndex == 2);
        Opcao("Garantia sobre a peça (%)", _garantia, () => _tipo.SelectedIndex == 2);
        Opcao("Complexidade da solda", _solda, () => _tipo.SelectedIndex == 2);
        Opcao("Horas de bancada", _horas, () => _tipo.SelectedIndex == 3);
        Opcao("Taxa por hora (R$)", _taxa, () => _tipo.SelectedIndex == 3);
        Opcao("Orçamento", _aprovado, () => _tipo.SelectedIndex == 3);
        _tipo.SelectedIndexChanged += (_, _) => ExibirCampos();
        _backup.CheckedChanged += (_, _) => ExibirCampos();
        Acoes(Botao("Adicionar à OS", Salvar));
        ExibirCampos();
    }

    private void Opcao(string titulo, Control controle, Func<bool> exibir)
    {
        var rotulo = new Label { Text = titulo, AutoSize = true };
        controle.AccessibleName = titulo;
        Linha(rotulo);
        Linha(controle);
        _opcoes.Add((rotulo, controle, exibir));
    }

    private void ExibirCampos()
    {
        Campos.SuspendLayout();
        foreach (var opcao in _opcoes) opcao.Rotulo.Visible = opcao.Controle.Visible = opcao.Exibir();
        Campos.ResumeLayout(true);
    }

    private void Salvar()
    {
        switch (_tipo.SelectedIndex)
        {
            case 0: _os.AdicionarFormatacao(_mao.Value, _sistema.Text, _backup.Checked, _backup.Checked ? _volume.Value : 0); break;
            case 1: _os.AdicionarLimpeza(_mao.Value, Escolhido<TipoPastaTermica>(_pasta, "a pasta")); break;
            case 2: _os.AdicionarReparo(_mao.Value, _componente.Text, _peca.Value, _garantia.Value, Escolhido<NivelComplexidadeSolda>(_solda, "a solda")); break;
            case 3: _os.AdicionarDiagnostico(_horas.Value, _taxa.Value, _aprovado.Checked); break;
        }
        DialogResult = DialogResult.OK;
    }
}
