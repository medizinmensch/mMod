function main(params) {

	//Sketch1: 
	var circle1 = CAG.circle ( { center: [ params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius } );
	var sketch1 = circle1;
	var object1 = sketch1.extrude({ offset: [0,0,25.4] });

	//Sketch2: 
	var circle2 = CAG.circle ( { center: [ params.circle2_CenterX, params.circle2_CenterY], radius: params.circle2_Radius } );
	var sketch2 = circle2;
	var object2 = sketch2.extrude({ offset: [0,0,50.8] });

	//Sketch3: 
	var circle3 = CAG.circle ( { center: [ params.circle3_CenterX, params.circle3_CenterY], radius: params.circle3_Radius } );
	var sketch3 = circle3;
	var object3 = sketch3.extrude({ offset: [0,0,76.2] });

	var OskarTheGreat = union(object1, object2, object3);

	return OskarTheGreat;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '12.7', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '24', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle2_Radius', caption: 'radius of circle2', type: 'float', initial: '25.4', step: 0.1 },
		{ name: 'circle2_CenterX', caption: 'X-Coordinate of circle2', type: 'float', initial: '-21.233', step: 0.1 },
		{ name: 'circle2_CenterY', caption: 'Y-Coordinate of circle2', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle3_Radius', caption: 'radius of circle3', type: 'float', initial: '38.1', step: 0.1 },
		{ name: 'circle3_CenterX', caption: 'X-Coordinate of circle3', type: 'float', initial: '88.411', step: 0.1 },
		{ name: 'circle3_CenterY', caption: 'Y-Coordinate of circle3', type: 'float', initial: '0', step: 0.1 },
	];
}
