# 💻 Sistema de Assistência Técnica de Computadores

> Projeto desenvolvido para a disciplina de **Programação Orientada por Objetos (POO)** do curso de Análise e Desenvolvimento de Sistemas da **PUC Minas** (2º/2026), sob orientação do Prof. Jardell Fillipe da Silva.

---

## 📌 Sobre o Projeto

Este sistema gerencia o fluxo operacional principal de uma loja de manutenção de computadores. Ele permite registrar a entrada de equipamentos dos clientes, alocar técnicos responsáveis, abrir Ordens de Serviço (OS), vincular os diferentes serviços executados a cada atendimento e, ao final, calcular automaticamente o valor total a ser pago pelo cliente de acordo com as particularidades de cada serviço.

---

## 🏗️ Arquitetura e Modelagem (POO)

O projeto foi desenhado para aplicar de forma prática os quatro pilares da Programação Orientada por Objetos, expandindo a proposta inicial para contemplar um domínio mais rico:

### Principais Entidades
* **`Cliente`**: Armazena os dados de identificação e contato do proprietário do equipamento.
* **`Computador`**: Registra os dados da máquina (Marca, Modelo, Número de Série e Defeito Relatado) vinculada a um cliente.
* **`Tecnico`**: Representa o profissional responsável pela execução dos serviços (Nome, Matrícula, Especialidade).
* **`OrdemDeServico`**: Classe central que gerencia o atendimento, vinculando o `Computador`, o `Tecnico`, o status da manutenção e a lista de `Servico`s realizados.
* **`Servico` (Classe Abstrata)**: Define a estrutura base para qualquer manutenção, exigindo a implementação do método abstrato `CalcularValor()`.

### Aplicação de Herança e Polimorfismo
As subclasses herdam de `Servico` e sobrescrevem (`override`) o método `CalcularValor()` com regras de negócio específicas:
1. **`Formatacao`**: Cobra um valor fixo base pelo serviço e adiciona uma taxa extra caso a opção de backup de arquivos (`RealizarBackup`) seja solicitada.
2. **`Limpeza`**: Soma o custo fixo da mão de obra ao valor específico do insumo utilizado (ex.: pasta térmica comum vs. pasta térmica à base de prata).
3. **`ReparoHardware`**: Soma o valor da mão de obra técnica ao custo da peça de reposição utilizada no conserto.

---

## 🚀 Tecnologias Utilizadas

* **Linguagem:** C# (.NET)
* **Paradigma:** Programação Orientada por Objetos (POO)
* **Controle de Versão:** Git & GitHub

---

## 👥 Equipe e Divisão de Responsabilidades

| Integrante | Responsabilidade Principal |
| :--- | :--- |
| **Caique de Lima e Silva Gonçalves** | Modelagem e implementação das classes `Cliente` e `Computador`. |
| **Jean Lucas Pereira de Assis** | Estruturação das entidades `Tecnico` e `OrdemDeServico` (lógica de agregação). |
| **Kayky Miranda Gusmao Guimarães** | Implementação da classe abstrata `Servico` e da classe derivada `Formatacao`. |
| **Marco Antonio Alves Moreira** | Criação das classes derivadas `Limpeza` e `ReparoHardware` (regras de polimorfismo). |
| **Rafael Lucas Mota Oliveira** | Configuração do repositório GitHub, documentação (`README.md`) e revisão de encapsulamento. |

---

## 🔧 Como Executar o Projeto

*(Nota: O projeto encontra-se atualmente na fase de **Modelagem do Sistema**. As instruções abaixo serão ativadas na etapa de codificação).*

1. Clone este repositório em sua máquina local:
   ```bash
   git clone https://github.com/SEU-USUARIO/NOME-DO-REPOSITORIO.git
   ```
2. Acesse a pasta do projeto:
   ```bash
   cd NOME-DO-REPOSITORIO
   ```
3. Compile e execute a aplicação via terminal do .NET:
   ```bash
   dotnet run
   ```

---
*Pontifícia Universidade Católica de Minas Gerais (PUC Minas) — 2026*
