-- =====================================================================
-- RetroGames - 003_consultas.sql  (Aula 1, etapa 5)
-- Consultas de exploracao. Aqui o JOIN e permitido; no Repository da aula 2
-- cada consulta le uma unica tabela.
-- =====================================================================
USE retrogames;

-- 1. Catalogo: jogos ativos com nome da plataforma e descricao da condicao, por nome
SELECT j.co_seq_jogo, j.no_jogo, p.no_plataforma, c.ds_condicao_jogo, j.vl_preco, j.qt_estoque
  FROM tb_jogo j
  JOIN tb_plataforma  p ON p.co_seq_plataforma = j.co_plataforma
  JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
 WHERE j.st_ativo = TRUE
 ORDER BY j.no_jogo;

-- 2. Generos de um jogo informado (troque o id)
SET @id_jogo = 2;
SELECT g.co_seq_genero, g.no_genero
  FROM tb_jogo_genero jg
  JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
 WHERE jg.co_jogo = @id_jogo
 ORDER BY g.no_genero;

-- 3. Jogos de um genero informado (troque o id)
SET @id_genero = 4;
SELECT j.co_seq_jogo, j.no_jogo
  FROM tb_jogo_genero jg
  JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
 WHERE jg.co_genero = @id_genero
 ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo plataformas sem jogos (LEFT JOIN)
SELECT p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos
  FROM tb_plataforma p
  LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
 GROUP BY p.co_seq_plataforma, p.no_plataforma
 ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, 5 itens por pagina
SET @busca  = '%o%';
SET @limite = 5;
-- pagina 1
SET @offset = 0;
SELECT co_seq_jogo, no_jogo FROM tb_jogo
 WHERE no_jogo LIKE @busca ORDER BY no_jogo LIMIT 5 OFFSET 0;
-- pagina 2
SELECT co_seq_jogo, no_jogo FROM tb_jogo
 WHERE no_jogo LIKE @busca ORDER BY no_jogo LIMIT 5 OFFSET 5;
-- total encontrado
SELECT COUNT(*) AS total FROM tb_jogo WHERE no_jogo LIKE @busca;
