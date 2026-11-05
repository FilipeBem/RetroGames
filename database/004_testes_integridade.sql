-- =====================================================================
-- RetroGames - 004_testes_integridade.sql  (Aula 1, etapa 5)
-- Execute UM teste por vez (selecione o comando e rode). Anote o resultado
-- obtido no campo "Obtido". Ao final, rode a limpeza.
-- Os codigos "Esperado" vem do roteiro; confirme-os ao executar.
-- =====================================================================
USE retrogames;

-- Teste 1: jogo com co_plataforma inexistente
-- Esperado: Error 1452 (FK).  Obtido: [preencher]
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('TESTE FK', 1990, 10.00, 1, UTC_TIMESTAMP(), 9999, 1);

-- Teste 2: plataforma com nome ja cadastrado
-- Esperado: Error 1062 (UNIQUE).  Obtido: [preencher]
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);

-- Teste 3: excluir plataforma que possui jogos
-- Esperado: Error 1451 (FK).  Obtido: [preencher]
DELETE FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo';

-- Teste 4: jogo com no_jogo nulo
-- Esperado: Error 1048 (NOT NULL).  Obtido: [preencher]
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (NULL, 1990, 10.00, 1, UTC_TIMESTAMP(), 1, 1);

-- Teste 5: jogo com vl_preco = 0
-- Esperado: ACEITO (a regra fica no Service, RN06).  Obtido: [preencher]
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('TESTE PRECO ZERO', 1990, 0, 1, UTC_TIMESTAMP(), 1, 1);

-- ---------------------------------------------------------------------
-- Limpeza: remove os dados criados pelos testes
-- ---------------------------------------------------------------------
DELETE FROM tb_jogo WHERE no_jogo LIKE 'TESTE%';
-- Conferencia: deve devolver 20 jogos, como no 002 (os ids do teste foram consumidos pelo AUTO_INCREMENT)
SELECT COUNT(*) AS jogos FROM tb_jogo;
