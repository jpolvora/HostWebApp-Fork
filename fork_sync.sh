git commit -mav "commit before sync with fork"
git fetch hostwebapp
git checkout master
git merge hostwebapp/master
git push origin master