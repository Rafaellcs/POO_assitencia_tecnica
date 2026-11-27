using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models;
using AssistenciaTecnica.Domain.Models.Servicos;

namespace AssistenciaTecnica.UI;

internal sealed class Ficha : Tela
{
    private readonly OrdemDeServico _os;
    private readonly ListBox _itens = new() { Height = 145 };
    private readonly Label _resumo = new() { AutoSize = true };
    private readonly Label _total = new() { AutoSize = true };
    private readonly NumericUpDown _desconto = Numero(0, 100);
    private readonly Button _iniciar, _adicionar, _concluir, _remover, _finalizar, _recibo;

    internal static string NomeStatus(StatusOS status) => status switch
    {
        StatusOS.EmAndamento => "Em andamento",
        StatusOS.Concluida => "Concluída",
        _ => "Aberta"
    };

    public Ficha(OrdemDeServico os) : base($"Ficha da OS {os.Numero}", 730, 590)
    {
        _os = os;
        Linha(_resumo);
        Campo("Serviços da visita", _itens);
        _iniciar = Botao("Iniciar atendimento", () => Alterar(_os.IniciarAtendimento));
        _adicionar = Botao("Lançar serviço", () =>
        {
            using var janela = new Lancamento(_os);
            janela.ShowDialog(this);
            Atualizar();
        });
        _concluir = Botao("Concluir item", () => Alterar(() => _os.ConcluirServico(ItemSelecionado())));
        _remover = Botao("Remover item", () => Alterar(() => _os.RemoverServico(ItemSelecionado())));
        _finalizar = Botao("Finalizar OS", () => Alterar(_os.FinalizarOS));
        _recibo = Botao("Consultar recibo", () => MessageBox.Show(this, _os.GerarRecibo(), "Recibo"));
        Acoes(_iniciar, _adicionar, _concluir, _remover);
        Campo("Desconto (%)", _desconto);
        _desconto.Value = os.DescontoPercentual;
        _desconto.ValueChanged += (_, _) => { _os.DefinirDesconto(_desconto.Value); AtualizarTotal(); };
        Linha(_total);
        Acoes(_finalizar, _recibo);
        Atualizar();
    }

    private int ItemSelecionado() => (_itens.SelectedItem as Servico)?.Id
        ?? throw new DominioException("Selecione um serviço da lista.");

    private void Alterar(Action acao)
    {
        acao();
        Atualizar();
    }

    private void AtualizarTotal() => _total.Text = $"Subtotal: {_os.Subtotal:C}     Total: {_os.CalcularValorTotal():C}";

    private void Atualizar()
    {
        _resumo.Text = $"OS {_os.Numero} · {NomeStatus(_os.Status)}\nCliente: {_os.Computador.Proprietario.Nome}" +
            $"\nEquipamento: {_os.Computador}\nTécnico: {_os.Tecnico.Nome}\nDefeito: {_os.DefeitoRelatado}";
        _itens.Items.Clear();
        foreach (var item in _os.Itens) _itens.Items.Add(item);
        bool emAndamento = _os.Status == StatusOS.EmAndamento;
        _iniciar.Visible = _os.Status == StatusOS.Aberta;
        _adicionar.Visible = _concluir.Visible = _remover.Visible = _finalizar.Visible = emAndamento;
        _desconto.Enabled = emAndamento;
        _recibo.Visible = _os.Status == StatusOS.Concluida;
        AtualizarTotal();
    }
}
