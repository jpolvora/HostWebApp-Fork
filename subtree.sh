git remote add --fetch MvcLib https://jpolvora@bitbucket.org/jpolvora/mvcfromdb.git

git subtree add --prefix=MvcLib MvcLib master --squash

git fetch MvcLib master
git subtree pull --prefix=MvcLib MvcLib master --squash