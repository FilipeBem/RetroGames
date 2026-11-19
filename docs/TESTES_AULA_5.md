# Testes da Aula 5 (gravação) — anote o resultado de cada um

Faça no Swagger (http://localhost:5080/swagger). Preencha a coluna "Obtido" com o status e o responseCode que apareceram.

## Plataformas
| # | Requisição | Esperado | Obtido |
|---|---|---|---|
| 1 | POST /api/Plataforma `{"nome":"Teste","fabricante":"Teste","anoLancamento":1999}` | 201, responseCode 0, cabeçalho Location | |
| 2 | GET na URL do Location | 200, a plataforma criada | |
| 3 | PUT /api/Plataforma/{id} com outro nome | 200, registro alterado | |
| 4 | PUT mantendo o mesmo nome (só muda o fabricante) | 200 (não é duplicidade) | |
| 5 | DELETE /api/Plataforma/{id} | 200, responseCode 0 | |
| 6 | POST com `nome` vazio e `anoLancamento` 1800 | 400, responseCode 3, erros em `data` (nome, anoLancamento) | |
| 7 | POST com JSON malformado (sem uma aspa) | 400, responseCode 3 | |
| 8 | POST com nome já existente (ex.: "Super Nintendo") | 409, responseCode 101 | |
| 9 | DELETE /api/Plataforma/1 (tem jogos) | 409, responseCode 102 | |
| 10 | DELETE ou PUT /api/Plataforma/99 | 404, responseCode 100 | |

## Jogos
| # | Requisição | Esperado | Obtido |
|---|---|---|---|
| 11 | POST /api/Jogo com `idsGeneros` [1,2], plataforma 1, condição 2, preço 50 | 201, Location, jogo com os gêneros | |
| 12 | PUT /api/Jogo/{id} trocando os gêneros | 200, gêneros novos (os antigos saem) | |
| 13 | DELETE /api/Jogo/{id} | 200; no MySQL, `st_ativo = 0` e o registro continua | |
| 14 | PUT /api/Jogo/{id} com `"ativo": true` | 200, jogo reativado | |
| 15 | POST com preço 0 | 400, responseCode 3, campo `preco` | |
| 16 | POST com `idsGeneros` vazio | 400, responseCode 3, campo `idsGeneros` | |
| 17 | POST com `idPlataforma` 99 | 404, responseCode 100 | |
| 18 | POST com `idCondicaoJogo` 99 | 404, responseCode 104 | |
| 19 | POST com `idsGeneros` [99] | 404, responseCode 105 | |
| 20 | PUT/DELETE /api/Jogo/9999 | 404, responseCode 103 | |

Depois dos testes, confira no MySQL o que o POST, o PUT e o DELETE mudaram e apague os dados de teste que sobraram.
