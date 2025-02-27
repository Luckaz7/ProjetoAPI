from flask import Flask, request, jsonify
import fitz
import requests

app = Flask(__name__)

API_URL = "https://api.groq.com/openai/v1/chat/completions"
API_KEY = "SUA API KEY"
MODEL = "llama-3.3-70b-versatile"


def extrairTextoPdf(arquivoPdf):
    documentoPdf = fitz.open(stream=arquivoPdf.read(), filetype="pdf")
    texto = ""
    for pagina in documentoPdf:
        texto = pagina.get_text()
    return texto


def chamandoApi(texto):
    cabecalho = {
        "Authorization": f"Bearer {API_KEY}",
        "Content-Type": "application/json"
    }

    requisicao = {
        "model": MODEL,
        "messages": [
            {"role": "system",
                "content": "Extraia as seguintes informações do texto: número do processo e partes envolvidas(autor, réu, advogados)."},
            {"role": "user", "content": texto}
        ]
    }

    resposta = requests.post(API_URL, headers=cabecalho, json=requisicao)
    return resposta.json()


@app.route("/extract", methods=["POST"])
def extrair():
    if 'file' not in request.files:
        return jsonify({"error": "Nenhum arquivo enviado."}), 400

    file = request.files['file']

    if file.filename == '':
        return jsonify({"error": "Nome de arquivo inválido."}), 400

    try:
        texto = extrairTextoPdf(file)
        resposta_llm = chamandoApi(texto)
        return jsonify(resposta_llm), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500


if __name__ == "__main__":
    app.run(debug=True)