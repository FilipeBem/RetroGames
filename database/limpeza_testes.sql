-- Remove os jogos criados pelo scripts/testar-api.mjs (a API nunca apaga jogo, so desativa).
-- Rode no MySQL Workbench depois dos testes.
USE retrogames;

DELETE FROM tb_jogo_genero
 WHERE co_jogo IN (SELECT co_seq_jogo FROM tb_jogo WHERE no_jogo LIKE 'TESTE API%');
DELETE FROM tb_jogo WHERE no_jogo LIKE 'TESTE API%';
DELETE FROM tb_plataforma WHERE no_plataforma LIKE 'TESTE API%';

SELECT COUNT(*) AS jogos FROM tb_jogo;   -- deve voltar a 20 (se so existirem os de exemplo)
