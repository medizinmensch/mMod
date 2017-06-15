function main(params) {
	var circle1 = CAG.circle ( { center: [ params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius } );
	var circle2 = CAG.circle ( { center: [ params.circle2_CenterX, params.circle2_CenterY], radius: params.circle2_Radius } );
	var circle3 = CAG.circle ( { center: [ params.circle3_CenterX, params.circle3_CenterY], radius: params.circle3_Radius } );
	var circle4 = CAG.circle ( { center: [ params.circle4_CenterX, params.circle4_CenterY], radius: params.circle4_Radius } );
	var circle5 = CAG.circle ( { center: [ params.circle5_CenterX, params.circle5_CenterY], radius: params.circle5_Radius } );
	var circle6 = CAG.circle ( { center: [ params.circle6_CenterX, params.circle6_CenterY], radius: params.circle6_Radius } );
	var circle7 = CAG.circle ( { center: [ params.circle7_CenterX, params.circle7_CenterY], radius: params.circle7_Radius } );
	var circle8 = CAG.circle ( { center: [ params.circle8_CenterX, params.circle8_CenterY], radius: params.circle8_Radius } );
	var circle9 = CAG.circle ( { center: [ params.circle9_CenterX, params.circle9_CenterY], radius: params.circle9_Radius } );
	var circle10 = CAG.circle ( { center: [ params.circle10_CenterX, params.circle10_CenterY], radius: params.circle10_Radius } );
	var circle11 = CAG.circle ( { center: [ params.circle11_CenterX, params.circle11_CenterY], radius: params.circle11_Radius } );
	var circle12 = CAG.circle ( { center: [ params.circle12_CenterX, params.circle12_CenterY], radius: params.circle12_Radius } );

	var sketches = union(circle1, circle2, circle3, circle4, circle5, circle6, circle7, circle8, circle9, circle10, circle11, circle12);
	sketches = sketches.extrude({ offset: [0,0,10] });

	return sketches;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '1.39866096114877', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '0.833076712988895', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '1.64950997937706', step: 0.1 },
		{ name: 'circle2_Radius', caption: 'radius of circle2', type: 'float', initial: '3.93143669360755', step: 0.1 },
		{ name: 'circle2_CenterX', caption: 'X-Coordinate of circle2', type: 'float', initial: '-4.12911728726221', step: 0.1 },
		{ name: 'circle2_CenterY', caption: 'Y-Coordinate of circle2', type: 'float', initial: '0', step: 0.1 },
		{ name: 'circle3_Radius', caption: 'radius of circle3', type: 'float', initial: '16.7670588952291', step: 0.1 },
		{ name: 'circle3_CenterX', caption: 'X-Coordinate of circle3', type: 'float', initial: '-7.69862310224565', step: 0.1 },
		{ name: 'circle3_CenterY', caption: 'Y-Coordinate of circle3', type: 'float', initial: '-20.1576352550415', step: 0.1 },
		{ name: 'circle4_Radius', caption: 'radius of circle4', type: 'float', initial: '9.69537161778274', step: 0.1 },
		{ name: 'circle4_CenterX', caption: 'X-Coordinate of circle4', type: 'float', initial: '11.36023498804', step: 0.1 },
		{ name: 'circle4_CenterY', caption: 'Y-Coordinate of circle4', type: 'float', initial: '-14.126350577671', step: 0.1 },
		{ name: 'circle5_Radius', caption: 'radius of circle5', type: 'float', initial: '15.6588277226866', step: 0.1 },
		{ name: 'circle5_CenterX', caption: 'X-Coordinate of circle5', type: 'float', initial: '-12.198064716029', step: 0.1 },
		{ name: 'circle5_CenterY', caption: 'Y-Coordinate of circle5', type: 'float', initial: '-46.8999968623735', step: 0.1 },
		{ name: 'circle6_Radius', caption: 'radius of circle6', type: 'float', initial: '17.6234430148897', step: 0.1 },
		{ name: 'circle6_CenterX', caption: 'X-Coordinate of circle6', type: 'float', initial: '-27.8568924387156', step: 0.1 },
		{ name: 'circle6_CenterY', caption: 'Y-Coordinate of circle6', type: 'float', initial: '-71.9179810913588', step: 0.1 },
		{ name: 'circle7_Radius', caption: 'radius of circle7', type: 'float', initial: '16.3096400308094', step: 0.1 },
		{ name: 'circle7_CenterX', caption: 'X-Coordinate of circle7', type: 'float', initial: '-20.7863275391921', step: 0.1 },
		{ name: 'circle7_CenterY', caption: 'Y-Coordinate of circle7', type: 'float', initial: '-101.97690244111', step: 0.1 },
		{ name: 'circle8_Radius', caption: 'radius of circle8', type: 'float', initial: '3.96460011405302', step: 0.1 },
		{ name: 'circle8_CenterX', caption: 'X-Coordinate of circle8', type: 'float', initial: '-27.8568924387156', step: 0.1 },
		{ name: 'circle8_CenterY', caption: 'Y-Coordinate of circle8', type: 'float', initial: '-39.4319418686465', step: 0.1 },
		{ name: 'circle9_Radius', caption: 'radius of circle9', type: 'float', initial: '9.4648570236512', step: 0.1 },
		{ name: 'circle9_CenterX', caption: 'X-Coordinate of circle9', type: 'float', initial: '-48.7915323973323', step: 0.1 },
		{ name: 'circle9_CenterY', caption: 'Y-Coordinate of circle9', type: 'float', initial: '-75.4653072133791', step: 0.1 },
		{ name: 'circle10_Radius', caption: 'radius of circle10', type: 'float', initial: '6.42968758719892', step: 0.1 },
		{ name: 'circle10_CenterX', caption: 'X-Coordinate of circle10', type: 'float', initial: '-10.233449423826', step: 0.1 },
		{ name: 'circle10_CenterY', caption: 'Y-Coordinate of circle10', type: 'float', initial: '-71.9179810913588', step: 0.1 },
		{ name: 'circle11_Radius', caption: 'radius of circle11', type: 'float', initial: '3.14305174990618', step: 0.1 },
		{ name: 'circle11_CenterX', caption: 'X-Coordinate of circle11', type: 'float', initial: '-8.06055398086976', step: 0.1 },
		{ name: 'circle11_CenterY', caption: 'Y-Coordinate of circle11', type: 'float', initial: '-91.775968891827', step: 0.1 },
		{ name: 'circle12_Radius', caption: 'radius of circle12', type: 'float', initial: '3.97671570415948', step: 0.1 },
		{ name: 'circle12_CenterX', caption: 'X-Coordinate of circle12', type: 'float', initial: '-4.91750223096358', step: 0.1 },
		{ name: 'circle12_CenterY', caption: 'Y-Coordinate of circle12', type: 'float', initial: '-105.743166010154', step: 0.1 },
	];
}
