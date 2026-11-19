# Testes da Aula 6 (acesso) — anote o resultado de cada um

Sempre a partir de uma janela anônima do navegador, sem sessão.

| # | Requisição | Esperado | Obtido |
|---|---|---|---|
| 1 | POST /api/Plataforma sem login | 401, responseCode 4 | |
| 2 | POST /api/Autenticacao/login com senha errada | 401, responseCode 6 | |
| 3 | POST login com e-mail mal formado (`"abc"`) | 400, responseCode 3, campo `email` em `data` | |
| 4 | POST login do administrador (e-mail e senha do .env) | 200, responseCode 0, sem hash na resposta; cookie `retrogames.sessao` | |
| 5 | POST /api/Plataforma logado como administrador | 201, responseCode 0 | |
| 6 | GET /api/Jogo?incluirInativos=true sem login | 200, só jogos ativos | |
| 7 | GET /api/Jogo?incluirInativos=true logado como admin | 200, inclui o inativo (Tekken 3) | |
| 8 | GET /api/Jogo/16 (inativo) sem login | 404, responseCode 103 | |
| 9 | GET /api/Autenticacao/eu logado | 200, dados do usuário | |
| 10 | POST logout e depois GET eu | 200 e 401 (responseCode 0 e 4) | |
| 11 | GET /api/Plataforma, /api/Genero, /api/CondicaoJogo, /api/Jogo sem login | 200 em todos | |
| 12 | /swagger sem login | abre normalmente | |

Cookie: nas ferramentas do navegador (F12, Application/Armazenamento, Cookies), confira que `retrogames.sessao` está marcado HttpOnly e SameSite=Strict.

Banco: `SELECT co_seq_usuario, ds_email, LENGTH(ds_senha_hash) FROM tb_usuario;` deve mostrar 1 administrador com hash longo. Subir a API de novo não pode criar um segundo.
