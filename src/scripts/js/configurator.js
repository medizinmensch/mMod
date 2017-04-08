/**
 * Created by jakob on 27.09.16.
 */

function Configurator(script_path) { //Configurator(script_path, stl_files) {

    //~ function loadStls(models) {
        //~ var stlfiles = {};
        //~ var loader = new THREE.STLLoader();
        //~ for (var name in models) {
            //~ (function (name) {
                //~ loader.load(models[name], function (geometry) {
                    //~ stlfiles[name] = THREE.CSG.toCSG(geometry);
                //~ });
            //~ })(name)
        //~ }
        //~ return stlfiles;
    //~ }

    //~ window.stlfiles = loadStls(stl_files);

    var gProcessor = new OpenJsCad.Processor(document.getElementById("viewerContext"), {useAsync: false});
    var xhr = new XMLHttpRequest();
    xhr.open("GET", script_path, true);
    xhr.onload = function () {
        var source = this.responseText;
        gProcessor.setJsCad(source, script_path);
    };
    xhr.send();

    this.refresh = function() {gProcessor.rebuildSolid();};
    this.get_stl = function() {return gProcessor.currentObject.toStlBinary();};
    this.processor = gProcessor;
}
