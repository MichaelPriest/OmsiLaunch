# Instalação

Extraia `OmsiLaunch-0.1.0-beta1.zip` diretamente na raiz da instalação OMSI
compatível. O pacote instala `OmsiLaunch.exe` e dependências do controller na
raiz, além dos arquivos permanentes do produto em `plugins\`.

`.omsilaunch\` é o diretório privado do OmsiLaunch para assets, diagnósticos e
journal temporário. Não mova os binários do plugin para esse diretório. Apenas
arquivos `OmsiLaunch.*` pertencem ao produto; plugins de terceiros não são
alterados por transações do OmsiLaunch.

O perfil suportado nesta beta é `Omsi23004_692EBFBF`. Executáveis OMSI sem
perfil conhecido são rejeitados antes da inicialização.
