# API de Extração de Informações de Processos Jurídicos

Esta API em Python lê um arquivo PDF contendo um processo jurídico, extrai seu conteúdo e utiliza uma API de Large Language Model (LLM) para obter informações específicas, como o número do processo e as partes envolvidas.

## Como Executar a API

1. Clone o repositório:
   
       git clone https://github.com/seu_usuario/seu_repositorio.git
       cd seu_repositorio
   
3. Crie um ambiente virtual e ative-o:

       python -m venv venv

       # No Windows:
       venv\Scripts\activate

       # No Unix/Mac:
       source venv/bin/activate

4. Instale as dependências:

       pip install -r requirements.txt

   O arquivo requirements.txt deve conter:

       Flask==3.1.3
       PyMuPDF==1.22.5
       requests==2.31.0

5. Defina a variável de ambiente para a chave da API:

       # No Windows
       set API_KEY=sua_chave_de_api_aqui

       # No Unix/Mac
       export API_KEY=sua_chave_de_api_aqui

6. Execute a aplicação:

       python app.py

## Como Utilizar a API:
  A API possui um endpoint /extract que aceita requisições POST com um arquivo PDF.

-> Exemplo de requisição usando curl:
  
    curl -X POST http://127.0.0.1:5000/extract -F "file=@seu_arquivo.pdf"

  obs: Substitua seu_arquivo.pdf pelo caminho do arquivo PDF que deseja enviar.

-> Resposta esperada:
  A API retornará um JSON com as informações extraídas, por exemplo:
  
    {
        "choices": [
        {
          "message": {
            "content": "Número do processo: 123456789\nPartes envolvidas: Autor - João Silva, Réu - Maria Souza, Advogado - Dr. Pedro Lima"
          }
        }
      ]
    }

## Explicação da Chamada à API de LLM
  Dentro do código, a função chamar_api_llm é responsável por enviar o texto extraído do PDF para a API de LLM. Ela faz uma requisição POST para o endpoint especificado (API_URL) com os seguintes componentes:

  Cabeçalhos(headers): Incluem a autorização (Bearer {API_KEY}) e o tipo de conteúdo (application/json).

  Requisição(payload): Um dicionário em formato JSON contendo:

    model: O modelo de LLM a ser utilizado.
    messages: Uma lista de mensagens que define o contexto e o conteúdo a ser analisado pelo modelo. Inclui:
  
    Uma mensagem com o papel de "system" que instrui o modelo sobre as informações a serem extraídas.
    Uma mensagem com o papel de "user" que contém o texto extraído do PDF.
  
  A resposta da API de LLM é então retornada como um dicionário JSON, que é posteriormente enviado como resposta da nossa API Flask.

  Observação: Certifique-se de substituir "sua_chave_de_api_aqui" pela sua chave de API real e de que o endpoint da API de LLM (API_URL) está correto e acessível.

## Exemplo de teste:
  Foi utilizado como exemplo de teste o arquivo teste.pdf presente dentro deste repositorio.
 
