function main(params) {
	//Sketch1: 
	var circle1 = CAG.circle ( { center: [ params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius } );
	var sketch1 = circle1;
	var extrusion1 = sketch1.extrude({ offset: [0,0,5.08] });

	//Sketch2: 
	var polygon2 = CAG.fromPoints ( [[7.341,1.7927], [9.2281,-3.1765], [1.3497,-4.2301] ] );
	var polygon3 = CAG.fromPoints ( [[-3.656,0.7494], [-8.6673,0.7494], [-8.6673,-2.0915], [-3.656,-2.0915] ] );
	var sketch2 = union(polygon2, polygon3);
	var extrusion2 = sketch2.extrude({ offset: [0,0,5.08] });

	var OskarTheGreat = union(extrusion1, extrusion2);

	return OskarTheGreat;
}
function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '1.0874', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '1.7639', step: 0.1 },
	];
}
