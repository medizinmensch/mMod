function main(params) {

	//Sketch1: 
	var circle1 = CAG.circle ({ center:[params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius});
	var polygon2 = CAG.fromPoints ( [[3,-8.517], [3,-3.517], [-3,-3.517], [-3,-8.517], [3,-8.517] ] );
	var sketch1 = union(circle1, polygon2);
	var Extrusion1 = sketch1.extrude({ offset: [0,0,params.Extrusion1] });

	//Sketch2: 
	var circle3 = CAG.circle ({ center:[params.circle3_CenterX, params.circle3_CenterY], radius: params.circle3_Radius});
	var sketch2 = circle3;
	var Extrusion2 = sketch2.extrude({ offset: [0,0,params.Extrusion2] });
	var Subtraction1 = Extrusion1.subtract(Extrusion2);

	var OskarTheGreat = union(Subtraction1);

	return OskarTheGreat;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '5', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '-12.5', step: 0.1 },
		{ name: 'Extrusion1', caption: 'Length of Extrusion1', type: 'float', initial: '10', step: 0.1 },
		{ name: 'circle3_Radius', caption: 'radius of circle3', type: 'float', initial: '1.5', step: 0.1 },
		{ name: 'circle3_CenterX', caption: 'X-Coordinate of circle3', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle3_CenterY', caption: 'Y-Coordinate of circle3', type: 'float', initial: '-12.5', step: 0.1 },
		{ name: 'Extrusion2', caption: 'Length of Extrusion2', type: 'float', initial: '10', step: 0.1 },
	];
}
