#Allows sync from original repo from this fork
# git remote add hostwebapp https://jpolvora@bitbucket.org/jpolvora/hostwebapp.git 

# Adiciona a subtree p/ pull e atualiza
git remote add --fetch Frankstein https://jpolvora@bitbucket.org/jpolvora/frankstein.git
git subtree add --prefix=Frankstein Frankstein master --squash

# Cria um remote para push
git remote add Frankstein_Push https://jpolvora@bitbucket.org/jpolvora/frankstein.git