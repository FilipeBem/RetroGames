-- =====================================================================
-- RetroGames - SQL usado nos Repositories (Aula 2)
-- Rode cada comando no MySQL Workbench com valores reais ANTES de confiar
-- no codigo C#. No C#, cada @parametro vira um parametro de comando.
-- Cada consulta le uma unica tabela (sem JOIN e sem subconsulta).
-- =====================================================================
USE retrogames;

-- ---------------------- PlataformaRepository -------------------------
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
  FROM tb_plataforma ORDER BY no_plataforma;                                   -- Listar

SET @id = 1;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento
  FROM tb_plataforma WHERE co_seq_plataforma = @id;                             -- BuscarPorId

SET @nome = 'Super Nintendo'; SET @idIgnorar = NULL;
SELECT COUNT(*) FROM tb_plataforma
 WHERE no_plataforma = @nome AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar);  -- ExisteComNome: 1
SET @idIgnorar = 1;
SELECT COUNT(*) FROM tb_plataforma
 WHERE no_plataforma = @nome AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar);  -- ignora a propria: 0

SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = @id;                         -- PossuiJogos: >0
SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = 7;                           -- Dreamcast: 0

-- Inserir / Alterar / Excluir (rode, confira e desfaca)
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES ('TESTE', 'Teste', 2000);
SET @novo = LAST_INSERT_ID();
UPDATE tb_plataforma SET no_plataforma = 'TESTE 2', no_fabricante = 'Teste', nu_ano_lancamento = 2001 WHERE co_seq_plataforma = @novo;
DELETE FROM tb_plataforma WHERE co_seq_plataforma = @novo;

-- ---------------------- JogoRepository --------------------------------
-- Listar sem filtro (so ativos), pagina 1 com 5 itens, e o total
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
  FROM tb_jogo WHERE st_ativo = TRUE ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
SELECT COUNT(*) FROM tb_jogo WHERE st_ativo = TRUE;                             -- 19

-- Segunda pagina
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE st_ativo = TRUE ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 5;

-- Filtro por busca
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE st_ativo = TRUE AND no_jogo LIKE '%mario%' ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
SELECT COUNT(*) FROM tb_jogo WHERE st_ativo = TRUE AND no_jogo LIKE '%mario%';

-- Filtro por plataforma
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE st_ativo = TRUE AND co_plataforma = 3 ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;

-- Filtro por lista de ids (vinda do filtro por genero, resolvido no Service com LINQ)
SELECT co_seq_jogo, no_jogo FROM tb_jogo WHERE st_ativo = TRUE AND co_seq_jogo IN (1, 6, 10) ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;

-- Incluindo inativos (admin): sem o filtro st_ativo
SELECT COUNT(*) FROM tb_jogo;                                                   -- 20

-- BuscarPorId, Inserir, Alterar, Desativar (desfaca ao final)
SELECT * FROM tb_jogo WHERE co_seq_jogo = 1;
INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('TESTE REPO', NULL, 1995, 10.00, 1, TRUE, UTC_TIMESTAMP(), 1, 1);
SET @novo = LAST_INSERT_ID();
UPDATE tb_jogo SET no_jogo = 'TESTE REPO 2', ds_jogo = 'x', nu_ano_lancamento = 1996, vl_preco = 12.50, qt_estoque = 2,
       co_plataforma = 2, co_condicao_jogo = 2 WHERE co_seq_jogo = @novo;
UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @novo;                   -- Desativar
-- ---------------------- JogoGeneroRepository --------------------------
SELECT co_jogo, co_genero FROM tb_jogo_genero ORDER BY co_jogo, co_genero;       -- ListarTodos
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (1, 2, 3) ORDER BY co_jogo, co_genero;  -- ListarPorJogos
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@novo, 1), (@novo, 2);   -- Inserir
DELETE FROM tb_jogo_genero WHERE co_jogo = @novo;                                -- RemoverPorJogo
-- Limpeza do teste de jogo
DELETE FROM tb_jogo WHERE co_seq_jogo = @novo;

-- ---------------------- Genero e CondicaoJogo -------------------------
SELECT co_seq_genero, no_genero FROM tb_genero ORDER BY no_genero;
SELECT co_condicao_jogo, ds_condicao_jogo FROM tb_condicao_jogo ORDER BY co_condicao_jogo;
