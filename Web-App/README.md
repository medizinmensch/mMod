# 3D Configurator



If you already have npm, more simple to start from here:

```
$ cd 3d-configurator
$ npm install -g gulp
$ npm install
$ gulp serve
```

## Install npm

npm need node.js to be install. Follow [this guide](https://docs.npmjs.com/getting-started/installing-node) to install node.js in your computer. If you already have installed node.js run `node -v` to check the version of your NPM version. The version should be higher than 0.12.0

**Updating npm**

Node comes with npm installed so you should have a version of npm. However, npm gets updated more frequently than Node does, so you'll want to make sure it's the latest version.

```
npm install npm@latest -g
```
Alternative:
```
npm install -g npm

```
Test: Run `npm -v`. The version should be higher than 2.1.8.

**Git clone the project**

Clone the project on your local machine:

```
git clone git@bitbucket.org:3yourmind/3d-configurator.git
```

The directory explained as follows:

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

Gulp will open browser and you can start coding now

## Deploy

Just run:

```
gulp dist
```

You can find all the code in `dist/` directory, gzipped, minified, and it's ready to ship