// Transforma uma falha de requisição em mensagem para o usuário.
export function obterMensagemErro(erro) {
  const resposta = erro?.response
  if (!resposta) {
    return 'Não foi possível conectar ao servidor.'
  }

  // Erro da API: o envelope traz errorMessage. Se não vier (por exemplo, o proxy sem API por trás),
  // trata como falta de conexão.
  return resposta.data?.errorMessage || 'Não foi possível conectar ao servidor.'
}
