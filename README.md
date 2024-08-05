# Como Utilizar o MoverDif
Baixe o Executável

Acesse o repositório MigrateChanges e navegue até o diretório MoverDif. Baixe o arquivo executável self-contained disponível lá.

Execute o Programa

Abra um terminal ou prompt de comando e navegue até o diretório onde o executável foi baixado, que deve ser MoverDif.

Comando de Execução

Execute o programa com o seguinte comando:

bash
Copiar código
MoverDif.exe <diretórioOrigem> <diretórioDestino> <caminhoDoArquivoDeLog>
Onde:

<diretórioOrigem> é o diretório de origem onde as mudanças foram registradas.
<diretórioDestino> é o diretório para onde os arquivos modificados devem ser movidos.
<caminhoDoArquivoDeLog> é o caminho para o arquivo de log que contém as alterações.
Exemplo:

Para mover arquivos modificados de C:\Pasta1 para D:\Backup e usar C:\Logs\log.json como o arquivo de log, use:

bash
Copiar código
MoverDif.exe "C:\Pasta1" "D:\Backup" "C:\Logs\log.json"
Parar a Execução

Para parar a execução, pressione Enter no terminal onde o programa está em execução. O programa finalizará após completar o processo de movimentação.
