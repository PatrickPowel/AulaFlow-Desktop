# AulaFlow Desktop

Painel de portfólio para organizar materiais de videoaulas, desenvolvido em **C#, .NET 10, WPF e SQLite com Entity Framework Core**.

Simulação independente de uma necessidade de operação audiovisual, inspirada na descrição de uma vaga. Não é um produto oficial da Estratégia Educacional e não integra seus sistemas. O visual usa referências gerais de plataformas educacionais, com identidade própria.

## Recursos

- Cadastro, listagem, edição e exclusão de materiais: vídeo, áudio e documento.
- Seleção de arquivo local e validação de existência e campos obrigatórios.
- Status manual: Pendente, Em processamento, Concluído e Erro.
- Busca por nome, curso, disciplina ou tipo, combinada com filtro de status.
- Contadores gerais e mensagem quando a busca não tem resultados.
- Abertura de arquivos com o programa associado no Windows.
- Automação: cópia assíncrona para `destino/Aula_curso/Aula_disciplina/id_arquivo`.
- Exportação CSV da lista filtrada, com escape de aspas e proteção contra fórmulas.
- Persistência local, preservada ao fechar o programa.

Excluir um cadastro não apaga o arquivo. A organização cria uma cópia e mantém o cadastro apontando para o original. Uma cópia já existente é recusada, sem sobrescrita. Os contadores usam todo o catálogo; a tabela e o CSV respeitam os filtros.

## Executar no Windows

Pré-requisito: SDK .NET 10. No Visual Studio, instale a carga de trabalho de desenvolvimento desktop com .NET. Abra `AulaFlow.slnx` e defina AulaFlow.Desktop como projeto de inicialização.

No PowerShell, na raiz do projeto:

```powershell
dotnet restore AulaFlow.slnx
dotnet build AulaFlow.slnx -c Release
dotnet test AulaFlow.slnx -c Release
dotnet run --project src/AulaFlow.Desktop
```

O banco é criado automaticamente em `%LOCALAPPDATA%/AulaFlow/aulaflow.db`. Nenhum arquivo pessoal ou banco é distribuído no repositório. Para demonstrar, crie um TXT de exemplo e cadastre como Documento; não precisa usar vídeos privados.

## Gerar executável

```powershell
dotnet publish src/AulaFlow.Desktop -c Release -r win-x64 --self-contained true -o artifacts/publish
```

Distribua a pasta completa de saída para Windows x64. O modo self-contained inclui o runtime; o SDK é necessário apenas para compilar. O diretório artifacts é ignorado pelo Git.

## Estrutura

```text
src/AulaFlow.Desktop/
  App.xaml.cs                     Inicialização e criação do banco
  MainWindow.xaml / .xaml.cs       Painel, eventos, consultas e filtros
  CadastroMaterialWindow.*         Formulário de cadastro e edição
  Models/Material.cs              Dados de um material
  Data/AulaFlowDbContext.cs        Acesso ao SQLite
  Services/MaterialTools.cs        Validação, filtros, CSV e cópia
tests/AulaFlow.Tests/              Testes de regras, arquivos e SQLite
docs/                             Guia técnico e roteiro de validação
```

## Limites desta versão

Aplicação local de um usuário. Não possui login, API, sincronização, edição/conversão de vídeo, fila de processamento, integração com Jira ou acesso aos sistemas da empresa. Status não executa processamento. A busca carrega o catálogo local em memória; para grandes volumes, evoluir para filtros SQL e paginação. O banco usa `EnsureCreated`, adequado ao protótipo: alterações futuras no esquema precisam de uma estratégia de migrações e backup. Não apague um banco existente para atualizar a aplicação.

Leia [o guia técnico](docs/GUIA-TECNICO.md) para entender o código e [a validação](docs/VALIDACAO.md) para preparar a demonstração.

## Publicar no seu GitHub

O `.gitignore` exclui binários, configurações pessoais e bancos locais. Antes do primeiro commit, confira os arquivos com `git status` e não inclua relatórios CSV com dados reais.

```powershell
git init
git add .
git diff --cached --stat
git commit -m "Conclui MVP do AulaFlow Desktop"
git branch -M main
```

Crie um repositório vazio no seu GitHub e execute os comandos de `git remote add origin` e `git push -u origin main` fornecidos por ele. A publicação remota não é feita automaticamente.
