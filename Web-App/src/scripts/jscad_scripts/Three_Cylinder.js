function main(params) {

	//Sketch1: 
	var circle1 = CAG.circle ({ center:[params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius});
	var sketch1 = circle1;
	var Extrusion1 = sketch1.extrude({ offset: [0,0,params.Extrusion1] });

	//Sketch2: 
	var circle2 = CAG.circle ({ center:[params.circle2_CenterX, params.circle2_CenterY], radius: params.circle2_Radius});
	var sketch2 = circle2;
	var Extrusion2 = sketch2.extrude({ offset: [0,0,params.Extrusion2] });

	//Sketch3: 
	var circle3 = CAG.circle ({ center:[params.circle3_CenterX, params.circle3_CenterY], radius: params.circle3_Radius});
	var sketch3 = circle3;
	var Extrusion3 = sketch3.extrude({ offset: [0,0,params.Extrusion3] });

	var OskarTheGreat = union(Extrusion1, Extrusion2, Extrusion3);

	return OskarTheGreat;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '1.5', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '-10', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '0', step: 0.1 },
		{ name: 'Extrusion1', caption: 'Length of Extrusion1', type: 'float', initial: '3', step: 0.1 },
		{ name: 'circle2_Radius', caption: 'radius of circle2', type: 'float', initial: '2', step: 0.1 },
		{ name: 'circle2_CenterX', caption: 'X-Coordinate of circle2', type: 'float', initial: '-5', step: 0.1 },
		{ name: 'circle2_CenterY', caption: 'Y-Coordinate of circle2', type: 'float', initial: '0', step: 0.1 },
		{ name: 'Extrusion2', caption: 'Length of Extrusion2', type: 'float', initial: '4', step: 0.1 },
		{ name: 'circle3_Radius', caption: 'radius of circle3', type: 'float', initial: '2.5', step: 0.1 },
		{ name: 'circle3_CenterX', caption: 'X-Coordinate of circle3', type: 'float', initial: '1', step: 0.1 },
		{ name: 'circle3_CenterY', caption: 'Y-Coordinate of circle3', type: 'float', initial: '0', step: 0.1 },
		{ name: 'Extrusion3', caption: 'Length of Extrusion3', type: 'float', initial: '5', step: 0.1 },
	];
}
