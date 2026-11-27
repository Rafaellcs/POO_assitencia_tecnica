using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models;
using AssistenciaTecnica.Services;

namespace AssistenciaTecnica.UI;

internal sealed class Principal : Tela
{
    private readonly Oficina _oficina = new();
    private readonly ListView _ordens = new()
    {
        View = View.Details, FullRowSelect = true, MultiSelect = false, HideSelection = false, Height = 300,
        AccessibleName = "Lista de ordens de serviço"
    };

    public Principal() : base("Assistência técnica · Ordens de serviço", 850, 450)
    {
        StartPosition = FormStartPosition.CenterScreen;
        foreach (var (nome, largura) in new[] { ("OS", 55), ("Cliente", 180), ("Equipamento", 210), ("Técnico", 130), ("Status", 130) })
            _ordens.Columns.Add(nome, largura);
        Linha(_ordens);
        var abrir = Botao("Abrir ficha", () =>
        {
            if (_ordens.SelectedItems.Count == 0) throw new DominioException("Selecione uma OS na lista.");
            Exibir((OrdemDeServico)_ordens.SelectedItems[0].Tag!);
        });
        _ordens.DoubleClick += (_, _) => abrir.PerformClick();
        Acoes(Botao("Nova OS", () =>
        {
            using var janela = new NovaOS(_oficina);
            if (janela.ShowDialog(this) == DialogResult.OK) Exibir(janela.Ordem!);
        }), abrir);
    }

    private void Exibir(OrdemDeServico ordem)
    {
        using var ficha = new Ficha(ordem);
        ficha.ShowDialog(this);
        _ordens.Items.Clear();
        foreach (var os in _oficina.Ordens)
        {
            var item = new ListViewItem(new[] { os.Numero.ToString(), os.Computador.Proprietario.Nome,
                os.Computador.ToString(), os.Tecnico.Nome, Ficha.NomeStatus(os.Status) }) { Tag = os };
            _ordens.Items.Add(item);
            if (os == ordem) item.Selected = true;
        }
    }
}
