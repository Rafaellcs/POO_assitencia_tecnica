using AssistenciaTecnica.Domain.Enums;
using AssistenciaTecnica.Domain.Exceptions;
using AssistenciaTecnica.Domain.Models;
using AssistenciaTecnica.Services;
using Xunit;

namespace AssistenciaTecnica.Tests;

public class OficinaTests
{
    private readonly Oficina _oficina = new();
    private readonly Cliente _cliente;
    private readonly Computador _pc;
    private readonly OrdemDeServico _os;

    public OficinaTests()
    {
        _cliente = _oficina.CadastrarCliente("Cliente de teste", "529.982.247-25", "31999999999", "teste@example.org");
        _pc = _oficina.CadastrarEquipamento(_cliente, "Marca", "Modelo", "PC-01", "Notebook");
        _os = _oficina.AbrirOS(_pc, _oficina.Tecnicos[0], "Tela escura");
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("52998224726")]
    [InlineData("123")]
    [InlineData("a52998224725")]
    public void RejeitaCpfInvalido(string cpf) => Assert.Throws<DominioException>(() => Cliente.NormalizarCpf(cpf));

    [Fact]
    public void CadastroNormalizaCpfEImpedeDuplicatas()
    {
        Assert.Same(_cliente, _oficina.BuscarCliente("52998224725"));
        Assert.Throws<DominioException>(() => _oficina.CadastrarCliente("Outro", _cliente.Cpf, "123", "outro@example.org"));
        var outro = _oficina.CadastrarCliente("Outro", "11144477735", "123", "outro@example.org");
        Assert.Throws<DominioException>(() => _oficina.CadastrarEquipamento(outro, "A", "B", " pc-01 ", "Desktop"));
        Assert.Empty(outro.Equipamentos);
    }

    [Fact]
    public void ContatoInvalidoPreservaValoresAnteriores()
    {
        Assert.Throws<DominioException>(() => _cliente.AtualizarContato("novo número", "inválido"));
        Assert.Equal("31999999999", _cliente.Telefone);
        Assert.Equal("teste@example.org", _cliente.Email);
    }

    [Theory]
    [InlineData(false, 0, 80)]
    [InlineData(true, 100, 110)]
    [InlineData(true, 101, 120)]
    [InlineData(true, 150, 120)]
    [InlineData(true, 151, 130)]
    public void FormatacaoCobraLotesSomenteNoExcedente(bool backup, int gb, int esperado)
    {
        _os.IniciarAtendimento();
        _os.AdicionarFormatacao(80, "Linux", backup, gb);
        Assert.Equal((decimal)esperado, _os.Itens.Single().CalcularValor());
    }

    [Theory]
    [InlineData(TipoPastaTermica.Comum, 90)]
    [InlineData(TipoPastaTermica.Prata, 105)]
    [InlineData(TipoPastaTermica.Ceramica, 115)]
    public void LimpezaSomaInsumo(TipoPastaTermica pasta, int esperado)
    {
        _os.IniciarAtendimento();
        _os.AdicionarLimpeza(80, pasta);
        Assert.Equal((decimal)esperado, _os.Itens.Single().CalcularValor());
    }

    [Theory]
    [InlineData(NivelComplexidadeSolda.Baixa, 190)]
    [InlineData(NivelComplexidadeSolda.Media, 230)]
    [InlineData(NivelComplexidadeSolda.Alta, 270)]
    public void ReparoAplicaFatorNaMaoDeObraEGarantiaNaPeca(NivelComplexidadeSolda nivel, int esperado)
    {
        _os.IniciarAtendimento();
        _os.AdicionarReparo(80, "Conector", 100, 10, nivel);
        Assert.Equal((decimal)esperado, _os.Itens.Single().CalcularValor());
    }

    [Theory]
    [InlineData(false, 160)]
    [InlineData(true, 80)]
    public void DiagnosticoAplicaReducaoQuandoAprovado(bool aprovado, int esperado)
    {
        _os.IniciarAtendimento();
        _os.AdicionarDiagnostico(2, 80, aprovado);
        Assert.Equal((decimal)esperado, _os.Itens.Single().CalcularValor());
    }

    [Fact]
    public void AtendimentoBloqueiaFechamentoVazioOuPendente()
    {
        Assert.Throws<DominioException>(_os.FinalizarOS);
        Assert.Throws<DominioException>(() => _os.AdicionarDiagnostico(1, 80, false));
        _os.IniciarAtendimento();
        Assert.Throws<DominioException>(_os.IniciarAtendimento);
        Assert.Throws<DominioException>(_os.FinalizarOS);
        _os.AdicionarDiagnostico(1, 80, false);
        Assert.Throws<DominioException>(_os.FinalizarOS);
        Assert.Throws<DominioException>(() => _os.GerarRecibo());
        Assert.Null(_os.Conclusao);
    }

    [Fact]
    public void CicloCompletoPreservaReciboEHistoricoDeDuasVisitas()
    {
        _os.IniciarAtendimento();
        _os.AdicionarLimpeza(80, TipoPastaTermica.Prata);
        _os.AdicionarDiagnostico(2, 80, true);
        foreach (var item in _os.Itens) _os.ConcluirServico(item.Id);
        _os.DefinirDesconto(10);
        _os.FinalizarOS();
        Assert.Equal(StatusOS.Concluida, _os.Status);
        Assert.NotNull(_os.Conclusao);
        Assert.Equal(166.50m, _os.CalcularValorTotal());
        string recibo = _os.GerarRecibo();
        Assert.Contains("Tela escura", recibo);
        Assert.Contains(_os.CalcularValorTotal().ToString("C"), recibo);
        Assert.Throws<DominioException>(() => _os.DefinirDesconto(100));
        Assert.Throws<DominioException>(() => _os.RemoverServico(1));
        Assert.Throws<DominioException>(() => _os.AdicionarLimpeza(10, TipoPastaTermica.Comum));
        var retorno = _oficina.AbrirOS(_pc, _oficina.Tecnicos[1], "Falha na bateria");
        Assert.NotEqual(_os.Numero, retorno.Numero);
        Assert.Same(_pc, retorno.Computador);
        Assert.Equal("Falha na bateria", retorno.DefeitoRelatado);
        Assert.Empty(retorno.Itens);
        Assert.Equal(recibo, _os.GerarRecibo());
    }

    [Fact]
    public void ReciboPreservaIdentificacaoDoTecnicoQuandoEspecialidadeMuda()
    {
        _os.IniciarAtendimento();
        _os.AdicionarDiagnostico(1, 100, false);
        _os.ConcluirServico(1);
        _os.FinalizarOS();
        var recibo = _os.GerarRecibo();
        _os.Tecnico.AlterarEspecialidade("Outra área");
        Assert.Equal(recibo, _os.GerarRecibo());
    }

    [Fact]
    public void ItemRemovidoNaoPodeSerRecuperadoPeloProximoLancamento()
    {
        _os.IniciarAtendimento();
        _os.AdicionarDiagnostico(1, 100, false);
        int id = _os.Itens.Single().Id;
        _os.RemoverServico(id);
        _os.AdicionarDiagnostico(1, 50, false);
        Assert.Throws<DominioException>(() => _os.ConcluirServico(id));
        Assert.False(_os.Itens.Single().Concluido);
    }

    [Fact]
    public void ValidacoesNaoDeixamItensParciaisOuDescontoInvalido()
    {
        _os.IniciarAtendimento();
        Assert.Throws<DominioException>(() => _os.AdicionarReparo(80, "Peça", -1, 0, NivelComplexidadeSolda.Baixa));
        Assert.Throws<DominioException>(() => _os.AdicionarLimpeza(-1, TipoPastaTermica.Comum));
        Assert.Throws<DominioException>(() => _os.AdicionarDiagnostico(-1, 80, false));
        Assert.Throws<DominioException>(() => _os.AdicionarFormatacao(10, " ", true, 5));
        Assert.Throws<DominioException>(() => _os.DefinirDesconto(101));
        Assert.Empty(_os.Itens);
        Assert.Equal(0m, _os.DescontoPercentual);
    }

    [Fact]
    public void TecnicoInativoEDefeitoVazioNaoAbremOrdem()
    {
        _oficina.Tecnicos[1].DefinirDisponibilidade(false);
        Assert.Throws<DominioException>(() => _oficina.AbrirOS(_pc, _oficina.Tecnicos[1], "Ruído"));
        Assert.Throws<DominioException>(() => _oficina.AbrirOS(_pc, _oficina.Tecnicos[0], " "));
        Assert.Single(_oficina.Ordens);
    }
}
