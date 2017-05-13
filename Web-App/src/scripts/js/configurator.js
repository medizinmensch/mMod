/**
 * Created by jakob on 27.09.16.
 */

function Configurator(script_path) { 
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