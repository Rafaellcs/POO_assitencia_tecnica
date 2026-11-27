# Busca por CPF mantinha o cliente anterior

**Local:** janela Nova OS, ao localizar um cliente por CPF.

**Impacto:** depois de uma busca sem resultado, a ficha ainda permitia abrir uma OS para o cliente e o equipamento que estavam selecionados antes da busca.

## Como reproduzir

1. Na janela Nova OS, cadastre um cliente e um equipamento. Mantenha ambos selecionados.
2. Digite um CPF válido que ainda não esteja cadastrado e clique em **Buscar CPF**.
3. Feche a mensagem de cliente não encontrado e observe os campos **Cliente** e **Equipamento**.

Na primeira execução do teste de interface, os dois campos mantiveram os valores anteriores. A verificação falhou na etapa 8.

## Causa e correção

A busca atribuía o resultado diretamente à seleção. Quando o CPF não era encontrado, a exceção interrompia a atribuição antes que o formulário limpasse a seleção antiga.

A janela agora limpa a seleção do cliente antes da consulta. A alteração também esvazia a lista de equipamentos. Uma busca bem-sucedida volta a selecionar o cliente correto e carrega suas máquinas.

## Verificação

O mesmo roteiro passou pelas 28 etapas após a correção. A busca sem resultado deixou os campos vazios. Em seguida, a busca pelo CPF cadastrado recuperou o equipamento correto. O teste abriu uma OS, verificou a mensagem de fechamento com pendência, concluiu dois serviços e conferiu o total de R$ 166,50 no recibo. Também abriu uma segunda visita e voltou à primeira para conferir o relato original.
