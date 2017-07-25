[![Build status](https://ci.appveyor.com/api/projects/status/r9ovipu6yu7numn6?svg=true)](https://ci.appveyor.com/project/carloscds/boleto2net)


# boleto2net
Nova versão do Boleto.Net

### Carteiras Homologadas
* Bradesco - Carteira 09
* Brasil - Carteira 17/019
* Caixa Econômica Federal - Carteira SIG14
* Itau - Carteira 109
* Santander - Carteira 101
* Sicoob - Carteira 1-01

### Carteiras Implementadas (Não foi homologada. Falta teste unitário)
* Banco do Brasil Carteira 11

> Atenção: Para manter a ordem do projeto, qualquer solicitação de Pull Request de um novo banco ou carteira implementada, deverá seguir o formato dos bancos/carteiras já implementados e vir acompanhado de teste unitário da geração do boleto (PDF), arquivo remessa e geração de 9 boletos, com dígitos da linha digitável variando de 1 a 9, checando além do próprio dígito verificador, o cálculo do nosso número, linha digitável e código de barras.
