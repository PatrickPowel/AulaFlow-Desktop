# Roteiro de demonstração e validação

Use arquivos fictícios. Preserve seus vídeos e documentos pessoais.

## Verificação realizada em 12/11/2026

- Compilação Release realizada com sucesso.
- Cinco testes automatizados aprovados: busca, CSV, validação, cópia e CRUD SQLite.
- Painel aberto e inspecionado visualmente com os quatro registros já existentes, sem alteração desses cadastros.
- Busca sem resultados verificada na interface; contadores gerais preservados.
- O restante do roteiro abaixo é uma lista para validação manual, não uma declaração de que todos os passos foram executados na interface.
- A recompilação durante a inspeção encontrou o executável em uso; após fechar a janela, a compilação e os cinco testes terminaram com sucesso.

## Roteiro manual

1. Abra o programa; confira que o painel e os contadores aparecem.
2. Crie um TXT temporário com um texto de exemplo. Cadastre como Documento, preenchendo nome, curso e disciplina.
3. Tente salvar com um campo vazio e depois com um caminho inexistente: deve haver aviso, sem cadastro inválido.
4. Feche e reabra o aplicativo: o material deve continuar listado.
5. Selecione e edite o status. Confira a tabela e os contadores.
6. Busque um trecho do curso e combine com o status. Uma busca inexistente deve mostrar a mensagem de lista vazia.
7. Abra o arquivo pelo botão e confira o programa associado do Windows.
8. Organize uma cópia em uma pasta temporária. Confira conteúdo, estrutura e original preservado. Repita para o mesmo destino: deve recusar sobrescrita.
9. Exporte a busca para CSV. Confira acentos, colunas e quantidade; caminhos pessoais não devem aparecer.
10. Mova o TXT de origem fora do aplicativo e tente abrir: deve avisar. Edite o cadastro para selecionar o novo caminho.
11. Exclua apenas o cadastro de demonstração pela interface e confira que o arquivo continua no disco.
12. Reduza a janela e verifique a rolagem, os botões e a tabela. Use Tab para percorrer os controles.

Os testes automatizados são executados com `dotnet test AulaFlow.slnx -c Release`. Registre falhas reais antes de apresentar; não trate a compilação como prova de que todos os fluxos visuais foram aprovados.
