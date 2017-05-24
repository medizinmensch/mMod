# 3D Configurator Web-App

## What is this?

mMod is a Tool to prepare and edit a 3D-Model in the Browser.

The `Inventor Addin` helps you marking parameters in an Inventor model and then export it as a jsCad-File.

Then, this File can be added to this [Folder](Web-App/src/scripts/jscad_scripts). The `Web-App` will include your model in the Select-Dropdown menu.

For further information see the seperate folders.

## Git clone the project

Clone the project on your local machine:

```
git clone https://github.com/medizinmensch/mMod.git
```

## Directory Explaination

The directory explained as follows:


```
|-- AddIn                       # Inventor Addin files go here
|-- Web-App                     # source file for project
|   |-- dist                    # deploy files   
|   |-- scr                     # source file for project
|   |   |-- images          
|   |   |-- scripts             # all the javascript files go here
|   |   |   |-- js
|   |   |   |-- jscad_scripts   # all the new models you want to add
|   |   |-- scss                # css style files
|   |   |-- index.html          # main/startup html file
|-- .gitgnore
|-- README.md                   # this file you are reading
|-- gulpfile.js                 # gulp tasks
```

## How we collaborate

1. clone (dont fork) your local copy
2. make your branch (by `git branch myBranch`)
3. set your branch as active (by `git checkout myBranch`)
4. set your upstream (by `it branch --set-upstream-to=origin/myBranch myBranch`)


## Credits

@joostn - we took several jsCad Files as placeholders