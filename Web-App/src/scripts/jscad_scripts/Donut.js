function main(params) {

	//Sketch1: 
	var circle1 = CAG.circle ({ center:[params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius});
	
	var sketch1 = circle1;
	var Revolution2 = rotate_extrude(sketch1);

	var OskarTheGreat = Revolution2;

	return OskarTheGreat;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '10', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '18', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '18', step: 0.1 },
	];
}
