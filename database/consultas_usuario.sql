-- =====================================================================
-- RetroGames - SQL do UsuarioRepository (Aula 6, etapa 1)
-- Rode no MySQL Workbench ANTES de confiar no codigo C#.
-- =====================================================================
USE retrogames;

SET @email = 'admin@retrogames.com';
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil
  FROM tb_usuario WHERE ds_email = @email;                                      -- BuscarPorEmail

SET @id = 1;
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil
  FROM tb_usuario WHERE co_seq_usuario = @id;                                   -- BuscarPorId

SELECT COUNT(*) FROM tb_usuario WHERE co_perfil = 1;                            -- ExisteAdministrador (1 = Administrador)

-- Inserir (so para testar a estrutura; o administrador real nasce pela API):
-- INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
-- VALUES ('Teste', 'teste@teste.com', 'hash-de-teste', TRUE, UTC_TIMESTAMP(), 2);
-- DELETE FROM tb_usuario WHERE ds_email = 'teste@teste.com';
