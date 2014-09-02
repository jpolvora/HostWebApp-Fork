#Allows sync from original repo from this fork
git remote add hostwebapp https://jpolvora@bitbucket.org/jpolvora/hostwebapp.git 

# Adiciona a subtree p/ pull e atualiza
git remote add --fetch MvcLib https://jpolvora@bitbucket.org/jpolvora/mvcfromdb.git
git subtree add --prefix=MvcLib MvcLib master --squash

# Cria um remote para push
git remote add MvcLib_Push https://jpolvora@bitbucket.org/jpolvora/mvcfromdb.git