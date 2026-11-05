-- =====================================================================
-- RetroGames - 002_dados_exemplo.sql
-- Executar logo depois do 001_criar_tabelas.sql.
-- Nao insere usuarios: o administrador sera criado pela API (aula de autenticacao).
-- =====================================================================
USE retrogames;

-- Condicoes de jogo (chaves fixas)
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

-- Perfis (chaves fixas)
INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- Plataformas (7; a ultima nao tem jogos, para testar contagem com 0)
INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    (1, 'Super Nintendo',  'Nintendo', 1990),
    (2, 'Mega Drive',      'Sega',     1988),
    (3, 'Nintendo 64',     'Nintendo', 1996),
    (4, 'PlayStation',     'Sony',     1994),
    (5, 'Atari 2600',      'Atari',    1977),
    (6, 'Game Boy',        'Nintendo', 1989),
    (7, 'Dreamcast',       'Sega',     1998);

-- Generos (8)
INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
    (1, 'Ação'),
    (2, 'Aventura'),
    (3, 'RPG'),
    (4, 'Plataforma'),
    (5, 'Luta'),
    (6, 'Corrida'),
    (7, 'Esporte'),
    (8, 'Quebra-cabeça');

-- Jogos (20). Ids fixos para facilitar o vinculo com os generos.
-- Inativo: Tekken 3 (id 16). Estoque zero: Pokemon Red (id 20).
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (1,  'Super Mario World',                       'Mario e Yoshi atravessam a Dinosaur Land.',                    1990, 189.90, 4, TRUE,  UTC_TIMESTAMP(), 1, 2),
    (2,  'The Legend of Zelda: A Link to the Past', 'Link viaja entre o Mundo da Luz e o Mundo das Trevas.',        1991, 249.90, 2, TRUE,  UTC_TIMESTAMP(), 1, 2),
    (3,  'Super Metroid',                           'Samus Aran explora o planeta Zebes.',                          1994, 299.00, 1, TRUE,  UTC_TIMESTAMP(), 1, 3),
    (4,  'Street Fighter II',                       'Clássico de luta com oito lutadores do mundo todo.',           1991, 159.90, 5, TRUE,  UTC_TIMESTAMP(), 1, 2),
    (5,  'Chrono Trigger',                          'RPG sobre viagens no tempo para salvar o futuro.',             1995, 459.90, 1, TRUE,  UTC_TIMESTAMP(), 1, 3),
    (6,  'Sonic the Hedgehog',                      'O ouriço azul corre contra o Dr. Robotnik.',                   1991, 129.90, 6, TRUE,  UTC_TIMESTAMP(), 2, 2),
    (7,  'Streets of Rage 2',                       'Beat em up cooperativo pelas ruas da cidade.',                 1992, 179.90, 3, TRUE,  UTC_TIMESTAMP(), 2, 2),
    (8,  'Phantasy Star IV',                        'Fim da saga de RPG espacial da Sega.',                         1993, 349.90, 1, TRUE,  UTC_TIMESTAMP(), 2, 3),
    (9,  'Mortal Kombat II',                        'Luta brutal com novos personagens e fatalities.',              1993, 119.90, 4, TRUE,  UTC_TIMESTAMP(), 2, 2),
    (10, 'Super Mario 64',                          'Mario em 3D dentro do castelo da Princesa Peach.',             1996, 279.90, 3, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (11, 'GoldenEye 007',                           'Tiro em primeira pessoa com modo multijogador.',               1997, 229.90, 2, TRUE,  UTC_TIMESTAMP(), 3, 2),
    (12, 'Mario Kart 64',                           'Corridas de kart com itens e até quatro jogadores.',           1996, 219.90, 5, TRUE,  UTC_TIMESTAMP(), 3, 1),
    (13, 'Final Fantasy VII',                       'Cloud e o grupo enfrentam a Shinra e Sephiroth.',              1997, 199.90, 2, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (14, 'Crash Bandicoot',                         'Plataforma 3D com o marsupial mais famoso do PlayStation.',    1996, 149.90, 3, TRUE,  UTC_TIMESTAMP(), 4, 2),
    (15, 'Gran Turismo',                            'Simulador de corrida com dezenas de carros licenciados.',      1997, 139.90, 4, TRUE,  UTC_TIMESTAMP(), 4, 3),
    (16, 'Tekken 3',                                'Luta 3D com Jin Kazama e Eddy Gordo. Fora de catálogo.',       1997, 169.90, 2, FALSE, UTC_TIMESTAMP(), 4, 2),
    (17, 'Pitfall!',                                'Pitfall Harry atravessa a selva atrás de tesouros.',           1982, 89.90,  2, TRUE,  UTC_TIMESTAMP(), 5, 2),
    (18, 'Pac-Man',                                 NULL,                                                           1982, 79.90,  3, TRUE,  UTC_TIMESTAMP(), 5, 2),
    (19, 'Tetris',                                  'Encaixe os blocos e limpe as linhas.',                         1989, 99.90,  7, TRUE,  UTC_TIMESTAMP(), 6, 3),
    (20, 'Pokémon Red',                             'Capture os 151 Pokémon e vire campeão da Liga.',               1996, 259.90, 0, TRUE,  UTC_TIMESTAMP(), 6, 2);

-- Vinculos jogo-genero (1 a 3 generos por jogo)
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (1, 4), (1, 2),
    (2, 2), (2, 1), (2, 3),
    (3, 1), (3, 2),
    (4, 5),
    (5, 3), (5, 2),
    (6, 4), (6, 1),
    (7, 1), (7, 5),
    (8, 3),
    (9, 5),
    (10, 4), (10, 2),
    (11, 1),
    (12, 6),
    (13, 3), (13, 2),
    (14, 4), (14, 1),
    (15, 6), (15, 7),
    (16, 5),
    (17, 4), (17, 2),
    (18, 1), (18, 8),
    (19, 8),
    (20, 3), (20, 2);

-- ---------------------------------------------------------------------
-- Verificacoes da etapa 4 (rode depois de popular)
-- ---------------------------------------------------------------------
-- SELECT (SELECT COUNT(*) FROM tb_plataforma) AS plataformas,
--        (SELECT COUNT(*) FROM tb_genero)     AS generos,
--        (SELECT COUNT(*) FROM tb_jogo)       AS jogos;            -- 7, 8, 20
-- SELECT j.co_seq_jogo, j.no_jogo FROM tb_jogo j
--  WHERE NOT EXISTS (SELECT 1 FROM tb_jogo_genero jg WHERE jg.co_jogo = j.co_seq_jogo); -- 0 linhas
-- SELECT no_jogo, st_ativo, qt_estoque FROM tb_jogo WHERE st_ativo = FALSE OR qt_estoque = 0; -- Tekken 3 e Pokémon Red
-- SELECT COUNT(*) FROM tb_usuario;                                  -- 0
