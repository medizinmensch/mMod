# 3D Configurator Web-App

## What is This?

mMod is a Tool to prepare and edit a 3D-Model in the Browser.

The `Inventor Addin` helps you marking parameters in an Inventor model and then export it as a jsCad-File.

Then, this File can be added to this [Folder](Web-App/src/scripts/jscad_scripts). The `Web-App` will include your model in the Select-Dropdown menu.

For further information see the seperate folders.

**Git clone the project**

Clone the project on your local machine:

```
git clone https://github.com/medizinmensch/mMod.git
```

The directory explained as follows:

```
* dist                # deploy files 
* src                 # source file for project
    * images          
    * scripts         # all the javascript files go here
    * scss            # css style files
    * index.html 
*.gitgnore
* README.md           # this file you are reading
* gulpfile.js         # gulp tasks
* package.json        # npm dependency 

```


```
|-- dist                # deploy files 
|-- src                 # source file for project
|   |-- images          
|   |-- scripts         # all the javascript files go here
|   |-- scss            # css style files
|   |-- index.html 
|-- .gitgnore
|-- README.md           # this file you are reading
|-- gulpfile.js         # gulp tasks
|-- package.json        # npm dependency 

```

## Install gulp

Gulp will automatically watch all the changes you made in project and compile or compress the scss, js files for you, and serve your project with local server. 

Run `sudo npm install -g gulp` to install gulp globally, so you can easily use `gulp` in your terminal.

After you install `gulp`, you need to install the dependencies for this project. 

```
cd 3d-configurator
npm install
```

## Develop

After you install everything, it's ready to gulp. Run:

```
gulp serve
```

Gulp will open browser and you can start coding now ðŸ’¼

## Deploy

Just run:

```
gulp dist
```

You can find all the code in `dist/` directory, gzipped, minified, and it's ready to ship ðŸ›³

14:36