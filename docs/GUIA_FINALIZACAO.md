# Guia: finalizar os Roteiros 1 e 2 e subir para o GitHub

O código das duas aulas já está escrito nesta pasta, com um commit por etapa.
Falta o que só dá para fazer na sua máquina: compilar, rodar o SQL no MySQL, criar o `.env` e dar o push.
Leia o código antes de enviar: você precisa conseguir explicá-lo.

## 0. Pré-requisitos
No terminal, cada comando deve mostrar a versão:

    dotnet --version     (10.x)
    node -v              (22.x)
    mysql --version      (8.x)
    git --version

Também: MySQL Workbench, VS Code com C# Dev Kit, conta no GitHub.

## 1. Abrir o projeto
1. Descompacte `RetroGames.zip`. A pasta `RetroGames` já é um repositório Git (pasta oculta `.git`).
2. Abra um terminal dentro dela e confira o histórico: `git log --oneline` (11 commits).
3. Configure seu nome e e-mail (os mesmos da sua conta do GitHub):

        git config user.name "Seu Nome"
        git config user.email "seu-email-do-github@exemplo.com"

   Os commits já existentes usam outro nome. Se quiser que apareçam como seus (isto também troca as datas para agora):

        git rebase -r --root --exec "git commit --amend --reset-author --no-edit"

## 2. Criar o repositório no GitHub e enviar (Aula 1, etapa 1)
1. No GitHub: New repository, nome `RetroGames`, **sem** README, sem .gitignore, sem licença.
2. No terminal da pasta:

        git remote add origin https://github.com/SEU_USUARIO/RetroGames.git
        git push -u origin main

   Se pedir senha, use um Personal Access Token (GitHub, Settings, Developer settings) ou rode `gh auth login`.
3. Confira no GitHub: `AGENTS.md` na raiz, `.env.example` presente e **nenhum** `.env`.

## 3. Quadro de tarefas (Aula 1, etapa 1, passo 5)
1. No repositório, aba **Projects**, New project, modelo Board.
2. Crie 6 cartões, um por linha de `docs/QUADRO_DE_TAREFAS.md`.
3. Mova para **Done** as funcionalidades que já tiverem sido feitas até o fim da aula (neste momento, nenhuma está completa; as tarefas das aulas 1 e 2 são a base).

## 4. Conferir a solução (Aula 1, etapa 2)
    dotnet build
    cd retrogames.web
    npm install
    npm run dev          (abre em http://localhost:5173)

Se `dotnet build` der erro, copie a mensagem completa para a IA. CS0246 indica referência faltando entre projetos.

## 5. Banco de dados (Aula 1, etapas 3 a 5)
No MySQL Workbench:
1. Abra `database/001_criar_tabelas.sql` e execute tudo (raio). Depois `SHOW TABLES;` deve listar 7 tabelas.
2. Abra `database/002_dados_exemplo.sql` e execute. Rode os `SELECT` de verificação comentados no fim do arquivo (7, 8 e 20; nenhum jogo sem gênero; `tb_usuario` vazia).
3. Abra `database/003_consultas.sql` e rode cada consulta separadamente.
4. Abra `database/004_testes_integridade.sql`. **Rode um teste por vez**: selecione o comando e use Ctrl+Shift+Enter. Em cada teste, troque `[preencher]` pelo erro que o Workbench mostrou (esperado: 1452, 1062, 1451, 1048; o 5 é aceito).
5. Rode a limpeza no fim do arquivo; `SELECT COUNT(*) FROM tb_jogo;` deve dar 20.
6. Teste "apagar e recriar": `DROP DATABASE retrogames;` e rode 001 e 002 de novo, sem erro.

Commit:

    git add database
    git commit -m "Aula 1: resultado dos testes de integridade"

## 6. Modelos (Aula 1, etapa 6)
Compare cada classe de `RetroGames.Model/Model` com a tabela no Workbench (Table Inspector): colunas, tipos e nulabilidade. Só `Jogo.Descricao` é anulável.

## 7. Connection string e .env (Aula 2, etapa 2)
1. No Workbench, execute: `SELECT @@hostname AS servidor, @@port AS porta, CURRENT_USER() AS usuario, DATABASE() AS banco;`
2. Copie o modelo: `cp .env.example .env` (no Windows: `copy .env.example .env`).
3. Edite o `.env` e troque `SUA_SENHA` pela senha do seu MySQL (e `root` pelo seu usuário, se for outro).
4. Confira: `git status` **não** pode listar `.env`.

Nunca coloque a senha no `.env.example`.

## 8. SQL do Repository (Aula 2, etapas 3 a 5)
1. Abra `database/consultas_repository.sql` e rode cada comando no Workbench. O roteiro exige que todo SQL rode antes de ir para o código.
2. Rode `dotnet build` de novo: sem erros.
3. Confira os itens "Confira" de cada etapa no PDF do roteiro (por exemplo, só o projeto Repository referencia MySqlConnector).

## 9. Último commit e push
    git status
    git add -A
    git commit -m "Aula 2: conferência final do Repository"
    git push

Confira no GitHub que o histórico mostra um commit por etapa e que não há `.env`, `bin/`, `obj/` nem `node_modules/`.

## 10. Checklist final (copiado dos roteiros)
- [ ] Repositório no GitHub com tudo, um commit por etapa
- [ ] `dotnet build` sem erros e front abre em localhost:5173
- [ ] Banco com as 7 tabelas e os dados de exemplo
- [ ] Consultas e testes de integridade com resultado anotado
- [ ] Classes de modelo batem com as tabelas
- [ ] Classes do Repository existem; `.env` fora do Git
- [ ] Cada SQL do Repository rodou no Workbench
- [ ] Cartões do quadro atualizados
