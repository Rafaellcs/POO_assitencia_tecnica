using AssistenciaTecnica.Domain.Exceptions;

namespace AssistenciaTecnica.UI;

internal class Tela : Form
{
    protected readonly TableLayoutPanel Campos = new()
    {
        Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, GrowStyle = TableLayoutPanelGrowStyle.AddRows
    };

    protected Tela(string titulo, int largura = 620, int altura = 560)
    {
        Text = titulo;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(largura, altura);
        MinimumSize = new Size(400, 340);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        Padding = new Padding(14);
        Campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(Campos);
    }

    protected void Linha(Control controle)
    {
        int linha = Campos.RowCount++;
        Campos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        controle.Dock = DockStyle.Fill;
        Campos.Controls.Add(controle, 0, linha);
    }

    protected void Campo(string titulo, Control controle)
    {
        Linha(new Label { Text = titulo, AutoSize = true, Margin = new Padding(3, 8, 3, 0) });
        controle.AccessibleName = titulo;
        Linha(controle);
    }

    protected Button Botao(string titulo, Action acao)
    {
        var botao = new Button { Text = titulo, AutoSize = true };
        botao.Click += (_, _) =>
        {
            try { acao(); }
            catch (DominioException erro) { MessageBox.Show(this, erro.Message, "Verifique os dados", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        };
        return botao;
    }

    protected void Acoes(params Button[] botoes)
    {
        var barra = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        barra.Controls.AddRange(botoes);
        Linha(barra);
    }

    protected static ComboBox Lista() => new() { DropDownStyle = ComboBoxStyle.DropDownList };

    protected static T Escolhido<T>(ComboBox lista, string campo) =>
        lista.SelectedItem is T valor ? valor : throw new DominioException($"Selecione {campo}.");

    protected static NumericUpDown Numero(decimal inicial = 0, decimal maximo = 1000000) =>
        new() { DecimalPlaces = 2, Maximum = maximo, Value = inicial, ThousandsSeparator = true };

    protected static void Preencher<T>(ComboBox lista, IEnumerable<T> valores)
    {
        lista.Items.Clear();
        foreach (var valor in valores) lista.Items.Add(valor!);
        if (lista.Items.Count > 0) lista.SelectedIndex = 0;
    }
}
