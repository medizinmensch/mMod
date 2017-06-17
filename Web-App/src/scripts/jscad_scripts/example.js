
function main(params) {
	var circle1 = CAG.circle ( { center: [ params.circle1_CenterX, params.circle1_CenterY], radius: params.circle1_Radius } );
	var circle2 = CAG.circle ( { center: [ params.circle2_CenterX, params.circle2_CenterY], radius: params.circle2_Radius } );
	var circle3 = CAG.circle ( { center: [ params.circle3_CenterX, params.circle3_CenterY], radius: params.circle3_Radius } );
	var circle4 = CAG.circle ( { center: [ params.circle4_CenterX, params.circle4_CenterY], radius: params.circle4_Radius } );
	var circle5 = CAG.circle ( { center: [ params.circle5_CenterX, params.circle5_CenterY], radius: params.circle5_Radius } );
	var circle6 = CAG.circle ( { center: [ params.circle6_CenterX, params.circle6_CenterY], radius: params.circle6_Radius } );
	var polygon7 = CAG.fromPoints ( [[0.2392,1.8937], [1.4963,-0.4055], [-0.4014,-2.3478], [-1.5936,1.6828] ] );
	var polygon8 = CAG.fromPoints ( [[-0.4014,3.6168], [3.3908,3.6168], [1.9376,0.4551], [0,0.7441] ] );
	var polygon9 = CAG.fromPoints ( [[-1.903,-1.462], [2.611,-2.4444], [1.7552,-5.9651], [-2.4693,-5.2595] ] );

	var sketches = union(circle1, circle2, circle3, circle4, circle5, circle6, polygon7, polygon8, polygon9);
	sketches = sketches.extrude({ offset: [0,0,10] });

	return sketches;
}

function getParameterDefinitions() {
	return [
		{ name: 'circle1_Radius', caption: 'radius of circle1', type: 'float', initial: '1.2365', step: 0.1 },
		{ name: 'circle1_CenterX', caption: 'X-Coordinate of circle1', type: 'float', initial: '3.445', step: 0.1 },
		{ name: 'circle1_CenterY', caption: 'Y-Coordinate of circle1', type: 'float', initial: '-2.0377', step: 0.1 },
		{ name: 'circle2_Radius', caption: 'radius of circle2', type: 'float', initial: '1.1746', step: 0.1 },
		{ name: 'circle2_CenterX', caption: 'X-Coordinate of circle2', type: 'float', initial: '-3.2678', step: 0.1 },
		{ name: 'circle2_CenterY', caption: 'Y-Coordinate of circle2', type: 'float', initial: '-3.4118', step: 0.1 },
		{ name: 'circle3_Radius', caption: 'radius of circle3', type: 'float', initial: '0.732', step: 0.1 },
		{ name: 'circle3_CenterX', caption: 'X-Coordinate of circle3', type: 'float', initial: '-1.5936', step: 0.1 },
		{ name: 'circle3_CenterY', caption: 'Y-Coordinate of circle3', type: 'float', initial: '1.6828', step: 0.1 },
		{ name: 'circle4_Radius', caption: 'radius of circle4', type: 'float', initial: '0.6038', step: 0.1 },
		{ name: 'circle4_CenterX', caption: 'X-Coordinate of circle4', type: 'float', initial: '1.8666', step: 0.1 },
		{ name: 'circle4_CenterY', caption: 'Y-Coordinate of circle4', type: 'float', initial: '3.1432', step: 0.1 },
		{ name: 'circle5_Radius', caption: 'radius of circle5', type: 'float', initial: '0.6619', step: 0.1 },
		{ name: 'circle5_CenterX', caption: 'X-Coordinate of circle5', type: 'float', initial: '1.9376', step: 0.1 },
		{ name: 'circle5_CenterY', caption: 'Y-Coordinate of circle5', type: 'float', initial: '1.537', step: 0.1 },
		{ name: 'circle6_Radius', caption: 'radius of circle6', type: 'float', initial: '0.5981', step: 0.1 },
		{ name: 'circle6_CenterX', caption: 'X-Coordinate of circle6', type: 'float', initial: '0.2392', step: 0.1 },
		{ name: 'circle6_CenterY', caption: 'Y-Coordinate of circle6', type: 'float', initial: '2.744', step: 0.1 },
	];
}
