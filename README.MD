#HostWebApp

* 1) fork this project (HostWebApp.git)
* 2) Clone your fork
* 3) run shell init scripts (it will include a subtree (MvcLib.git)
    
	bash -x init_cfg_subtree.sh

Rename *.sln to match your appharbor app slug

Add a new Project with your classes, views, etc.
post build action => copy dll's from your project to ~/_plugins.

Run It!
