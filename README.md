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

* **Linguagem:** C# (.NET 8.0)
* **Paradigma:** Programação Orientada por Objetos (POO)
* **Controle de Versão:** Git & GitHub

---

## 👥 Equipe e Divisão de Responsabilidades

| Responsável | Cards |
| :--- | :--- |
| **Rafael Lucas Mota Oliveira** | #1 Setup, #2 Serviço, #3 Serviços de bancada, #4 Ordem de serviço e #5 Integração e menu. |
| **Jean Lucas Pereira de Assis** | #6 Cliente e computador e #7 Slides, parte 1. |
| **Kayky Miranda Gusmao Guimarães** | #8 Técnico e formatação e #9 Slides, parte 2. |

---

## 🔧 Como Executar o Projeto

Instale o SDK do .NET 8.0 e execute:

```bash
git clone https://github.com/Rafaellcs/POO_assitencia_tecnica.git
cd POO_assitencia_tecnica
dotnet build
dotnet run
```

A versão inicial imprime o nome do sistema no console.

---
*Pontifícia Universidade Católica de Minas Gerais (PUC Minas) — 2026*
