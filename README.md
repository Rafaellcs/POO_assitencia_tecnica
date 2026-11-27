# Sistema de Assistência Técnica de Computadores

Projeto da disciplina de **Programação Orientada por Objetos** do curso de Análise e Desenvolvimento de Sistemas da **PUC Minas** (2º/2026), sob orientação do Prof. Jardell Fillipe da Silva.

A aplicação Windows Forms registra os computadores que chegam à oficina, acompanha cada ordem de serviço (OS) e emite o recibo do reparo. **Os dados ficam em memória enquanto o programa está aberto. Fechar a janela principal encerra a sessão e apaga os cadastros e as ordens.**

## Baixar e executar no Windows

1. Instale o **SDK do .NET 8** para a arquitetura do seu Windows. O instalador do SDK já inclui o runtime do Windows Desktop. [Download oficial da Microsoft](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
2. Abra um novo PowerShell e execute `dotnet --list-sdks`. Procure uma linha que comece com `8.0.`.
3. Para baixar sem Git, abra o repositório, clique em **Code → Download ZIP**, extraia o arquivo e abra o PowerShell na pasta que contém `AssistenciaTecnica.csproj`. Se já usa Git, execute os dois comandos abaixo. [Repositório do projeto](https://github.com/Rafaellcs/POO_assitencia_tecnica)

```powershell
git clone https://github.com/Rafaellcs/POO_assitencia_tecnica.git
cd POO_assitencia_tecnica
```

Na pasta do projeto, restaure as dependências, compile e abra a aplicação.

```powershell
dotnet restore AssistenciaTecnica.csproj
dotnet build AssistenciaTecnica.csproj -c Release --no-restore
dotnet run --project AssistenciaTecnica.csproj -c Release --no-build
```

A primeira janela é **Ordens de serviço**. A lista começa vazia. O sistema já oferece dois técnicos para a demonstração, Ana (software) e Bruno (hardware).

## Usar o sistema

1. **Identificar o cliente.** Clique em **Nova OS**. Para o primeiro atendimento, escolha **Cadastrar cliente**, preencha nome, CPF, telefone e e-mail e clique em **Salvar**. Nas próximas visitas, escolha o cliente na lista ou digite o CPF e clique em **Buscar CPF**. A busca aceita o CPF com ou sem pontuação.
2. **Receber o equipamento.** Com o cliente selecionado, clique em **Cadastrar equipamento**. Informe marca, modelo, número de série e tipo, como notebook ou desktop. Clique em **Salvar**. Se a máquina já passou pela oficina, escolha o cadastro existente na lista de equipamentos do proprietário.
3. **Abrir a OS.** Escolha o técnico responsável, descreva o defeito relatado **nesta visita** e clique em **Abrir OS**. A ficha recebe um número e começa no estado **Aberta**.
4. **Executar os serviços.** Clique em **Iniciar atendimento** e depois em **Lançar serviço**. Selecione um dos quatro tipos, preencha os campos e clique em **Adicionar à OS**. Repita para os demais itens. Para corrigir um lançamento durante o atendimento, selecione o item, clique em **Remover item** e lance-o novamente.
5. **Finalizar e cobrar.** Selecione cada serviço e clique em **Concluir item**. Informe o desconto da ordem, de 0 a 100%. Confira o total e clique em **Finalizar OS**. O sistema exige pelo menos um serviço e a conclusão de todos os itens. Clique em **Consultar recibo** para ver os valores e as datas.
6. **Consultar o histórico.** Feche apenas a ficha para voltar à lista. Selecione uma OS e clique em **Abrir ficha**, ou dê um duplo clique na linha. Para registrar o retorno de uma máquina, abra **Nova OS** para o mesmo cliente e equipamento e descreva o novo defeito. A ficha anterior mantém o relato e o recibo originais.

### Campos e preços dos serviços

| Serviço | O que informar | Como o valor é calculado |
| --- | --- | --- |
| Formatação | Mão de obra, sistema operacional e, se houver backup, o volume em GB | Mão de obra + R$ 30,00 pelo backup de até 100 GB. Cada lote adicional de 50 GB ou fração custa R$ 10,00. Sem backup, cobra-se apenas a mão de obra. |
| Limpeza | Mão de obra e tipo de pasta térmica | Mão de obra + custo da pasta. Comum custa R$ 10,00, prata R$ 25,00 e cerâmica R$ 35,00. |
| Reparo de hardware | Mão de obra, componente, custo da peça, garantia em % e nível de solda | Mão de obra × fator da solda + peça + garantia sobre o custo da peça. Os fatores são 1 para baixa, 1,5 para média e 2 para alta. |
| Diagnóstico avançado | Horas de bancada, taxa por hora e aprovação do orçamento | Horas × taxa por hora. Com o orçamento aprovado, o diagnóstico custa metade desse valor. |

**Exemplo para a apresentação.** Lance uma limpeza com R$ 80,00 de mão de obra e pasta prata. O item custa R$ 105,00. Adicione um diagnóstico de 2 horas a R$ 80,00/h e marque **Orçamento aprovado**. Esse item custa R$ 80,00. O subtotal é R$ 185,00. Com desconto de 10%, o recibo deve mostrar **R$ 166,50**.

## Testes

No PowerShell, a partir da pasta que contém o projeto principal, execute os comandos abaixo. A primeira restauração dos testes requer internet para baixar o xUnit e o executor pelo NuGet.

```powershell
dotnet restore Tests/AssistenciaTecnica.Tests.csproj
dotnet test Tests/AssistenciaTecnica.Tests.csproj -c Release --no-restore
```

A suíte verifica os limites de backup, os valores dos insumos, os fatores de solda, a aprovação do orçamento, os cadastros, as transições de estado, os descontos e a integridade do recibo em duas visitas. O relatório descreve uma falha encontrada nas telas e como reproduzir a verificação. [Relatório de resolução](docs/Resolucao-de-bug.md)

## Estrutura e modelagem

| Pasta | Responsabilidade |
| --- | --- |
| `Domain/Models` | Cliente, computador, técnico, ordem de serviço e validações cadastrais. |
| `Domain/Models/Servicos` | Contrato abstrato de serviço e quatro especializações de preço. |
| `Domain/Enums` e `Domain/Exceptions` | Estados, tipos de pasta, níveis de solda e erros de negócio. |
| `Services` | A classe `Oficina` mantém os cadastros e o histórico de ordens da sessão. |
| `UI` | Janelas de listagem, abertura, ficha da OS e lançamento de serviços. |
| `Tests` | Testes automatizados do domínio e do fluxo de atendimento. |

Um cliente pode ter vários computadores. Cada OS referencia um equipamento e um técnico, e ambos podem participar de várias visitas. A relação entre OS e serviço é **composição**. A ordem cria seus itens, mantém a lista privada e controla a conclusão e a remoção de cada um. Um item pertence exclusivamente à sua ordem.

A classe abstrata `Servico` exige `CalcularValor()`. As quatro subclasses implementam suas próprias fórmulas. A OS soma o resultado de cada chamada e aplica o desconto ao subtotal. O total usa polimorfismo, e as regras de preço ficam reunidas no domínio.

## Equipe e divisão dos cards

| Responsável no quadro | Cards |
| --- | --- |
| Rafael Lucas Mota Oliveira | #1 Setup, #2 Serviço, #3 Serviços de bancada, #4 Ordem de serviço e #5 Integração e interface. |
| Jean Lucas Pereira de Assis | #6 Cliente e computador e #7 Slides, parte 1. |
| Kayky Miranda Gusmao Guimarães | #8 Técnico e formatação e #9 Slides, parte 2. |

A apresentação reúne o contexto da oficina, a modelagem, os quatro pilares de POO e o roteiro de demonstração. [Baixar os slides](docs/Apresentacao.pptx)
