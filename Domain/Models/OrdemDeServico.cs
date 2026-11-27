using System.Text;
using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models.Servicos;

namespace AssistenciaTecnica.Domain.Models;

public sealed class OrdemDeServico
{
    private readonly List<Servico> _itens = [];
    private int _proximoItem = 1;
    public int Numero { get; }
    public Computador Computador { get; }
    public Tecnico Tecnico { get; }
    public string DefeitoRelatado { get; }
    public DateTime Abertura { get; } = DateTime.Now;
    public DateTime? Conclusao { get; private set; }
    public StatusOS Status { get; private set; } = StatusOS.Aberta;
    public decimal DescontoPercentual { get; private set; }
    public IReadOnlyList<Servico> Itens => _itens.AsReadOnly();

    public OrdemDeServico(int numero, Computador computador, Tecnico tecnico, string defeito)
    {
        Numero = Validacao.Id(numero);
        Computador = computador ?? throw new DominioException("Selecione o equipamento.");
        if (tecnico is not { Ativo: true }) throw new DominioException("Selecione um técnico ativo.");
        Tecnico = tecnico;
        DefeitoRelatado = Validacao.Texto(defeito, "o defeito desta visita");
    }

    public void IniciarAtendimento()
    {
        if (Status != StatusOS.Aberta) throw new DominioException("Apenas uma OS aberta pode ser iniciada.");
        Status = StatusOS.EmAndamento;
    }

    private void ExigirAtendimento()
    {
        if (Status != StatusOS.EmAndamento) throw new DominioException("A OS precisa estar em andamento.");
    }

    private void Adicionar(Func<int, Servico> criar)
    {
        ExigirAtendimento();
        _itens.Add(criar(_proximoItem));
        _proximoItem++;
    }

    public void AdicionarFormatacao(decimal baseServico, string sistema, bool backup, decimal volumeGB) =>
        Adicionar(id => new Formatacao(id, baseServico, sistema, backup, volumeGB));

    public void AdicionarLimpeza(decimal baseServico, TipoPastaTermica pasta) =>
        Adicionar(id => new Limpeza(id, baseServico, pasta));

    public void AdicionarReparo(decimal baseServico, string componente, decimal custo, decimal garantia, NivelComplexidadeSolda nivel) =>
        Adicionar(id => new ReparoHardware(id, baseServico, componente, custo, garantia, nivel));

    public void AdicionarDiagnostico(decimal horas, decimal taxa, bool aprovado) =>
        Adicionar(id => new DiagnosticoAvancado(id, horas, taxa, aprovado));

    private Servico LocalizarItem(int id)
    {
        ExigirAtendimento();
        return _itens.Find(item => item.Id == id) ?? throw new DominioException("Selecione um serviço da ficha.");
    }

    public void RemoverServico(int id) => _itens.Remove(LocalizarItem(id));
    public void ConcluirServico(int id) => LocalizarItem(id).Concluir();

    public void DefinirDesconto(decimal percentual)
    {
        ExigirAtendimento();
        DescontoPercentual = Validacao.Percentual(percentual);
    }

    public decimal Subtotal => _itens.Sum(item => item.CalcularValor());
    public decimal ValorDesconto => Validacao.Moeda(Subtotal * DescontoPercentual / 100m);
    public decimal CalcularValorTotal() => Subtotal - ValorDesconto;

    public void FinalizarOS()
    {
        ExigirAtendimento();
        if (_itens.Count == 0 || _itens.Any(item => !item.Concluido))
            throw new DominioException("Inclua pelo menos um serviço e conclua todos os itens antes de finalizar.");
        Conclusao = DateTime.Now;
        Status = StatusOS.Concluida;
    }

    public string GerarRecibo()
    {
        if (Status != StatusOS.Concluida) throw new DominioException("Finalize a OS antes de emitir o recibo.");
        var texto = new StringBuilder();
        texto.AppendLine($"RECIBO · OS {Numero}");
        texto.AppendLine($"Abertura: {Abertura:g}    Conclusão: {Conclusao:g}");
        texto.AppendLine($"Cliente: {Computador.Proprietario}");
        texto.AppendLine($"Equipamento: {Computador} ({Computador.Tipo})");
        texto.AppendLine($"Técnico: {Tecnico.Nome} · {Tecnico.Matricula}");
        texto.AppendLine($"Defeito relatado: {DefeitoRelatado}");
        texto.AppendLine();
        foreach (var item in _itens) texto.AppendLine($"{item.Id}. {item.Descricao} — {item.CalcularValor():C}");
        texto.AppendLine($"Subtotal: {Subtotal:C}");
        texto.AppendLine($"Desconto ({DescontoPercentual:0.##}%): {ValorDesconto:C}");
        texto.Append($"Total: {CalcularValorTotal():C}");
        return texto.ToString();
    }
}
