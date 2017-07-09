function main(params) {

	//Sketch1: 
	var circle1 = CAG.circle ({ center:[params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius});
	var circle2 = CAG.circle ({ center:[params.circle2_CenterX, params.circle2_CenterY], radius: params.circle2_Radius});
	var sketch1 = union(circle1, circle2);
	var Extrusion1 = sketch1.extrude({ offset: [0,0,params.Extrusion1] });

	//Sketch2: 
	var polygon5 = CAG.fromPoints ( [[2.5,15], [39.654,0], [2.5,-15], [-15.979,0], [2.5,15] ] );
	var sketch2 = union(polygon5);
	var Extrusion5 = sketch2.extrude({ offset: [0,0,params.Extrusion2] });

	//Sketch3: 
	var polygon8 = CAG.fromPoints ( [[-32.5,15], [-69.637,0], [-32.5,-15], [-15,0], [-32.5,15] ] );
	var sketch3 = union(polygon8);
	var Extrusion6 = sketch3.extrude({ offset: [0,0,params.Extrusion3] });

	var OskarTheGreat = union(Extrusion1, Extrusion5, Extrusion6);

	return OskarTheGreat;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '17.5', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '-15', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '15', step: 0.1 },
		{ name: 'circle2_Radius', caption: 'radius of circle2', type: 'float', initial: '17.5', step: 0.1 },
		{ name: 'circle2_CenterX', caption: 'X-Coordinate of circle2', type: 'float', initial: '-15', step: 0.1 },
		{ name: 'circle2_CenterY', caption: 'Y-Coordinate of circle2', type: 'float', initial: '-15', step: 0.1 },
		{ name: 'Extrusion1', caption: 'Length of Extrusion1', type: 'float', initial: '10', step: 0.1 },
		{ name: 'Extrusion2', caption: 'Length of Extrusion2', type: 'float', initial: '5', step: 0.1 },
		{ name: 'Extrusion3', caption: 'Length of Extrusion3', type: 'float', initial: '5', step: 0.1 },
	];
}
