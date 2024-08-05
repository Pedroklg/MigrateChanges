# Como Utilizar o MoverDif
MoverDif é um aplicativo que copia ou exclui arquivos com base em entradas de log. Ele usa o robocopy para mover arquivos modificados e manter a estrutura de diretórios.

Passos para Utilizar
Baixe o Executável

Acesse o repositório MigrateChanges e navegue até o diretório MoverDif. Baixe o arquivo executável self-contained disponível lá.

Execute o Programa

Abra um terminal ou prompt de comando e navegue até o diretório onde o executável foi baixado, que deve ser MoverDif.

Comando de Execução

Execute o programa com o seguinte comando:

bash
Copiar código
.\MoverDif.exe <diretórioDestino> <caminhoDoArquivoDeLog> [níveisParaPular]
Onde:

<diretórioDestino> é o diretório para onde os arquivos modificados devem ser movidos.
<caminhoDoArquivoDeLog> é o caminho completo para o arquivo de log que contém as alterações.
[níveisParaPular] (opcional) é o número de níveis de diretório a serem ignorados na construção do caminho relativo.
Exemplo:

Para mover arquivos modificados para D:\Backup, usando C:\Logs\log.json como o arquivo de log e ignorando 1 nível de diretório, use:

bash
Copiar código
.\MoverDif.exe "D:\Backup" "C:\Logs\log.json" 1
Parar a Execução

Para parar a execução, pressione Enter no terminal onde o programa está em execução. O programa finalizará após completar o processo de movimentação.

O que o Programa Faz
Criação e Alteração: Copia arquivos criados ou alterados para o diretório de destino.
Exclusão: Remove arquivos do diretório de destino que foram excluídos do diretório de origem.
Renomeação: Move arquivos renomeados do diretório de origem para o destino, excluindo o arquivo antigo.
Notas
O programa utiliza o robocopy para copiar arquivos. Certifique-se de que o robocopy esteja disponível no seu sistema.
O arquivo de log deve estar no formato JSON gerado pelo aplicativo de monitoramento.
