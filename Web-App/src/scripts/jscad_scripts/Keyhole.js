function main(params) {

	//Sketch1: 
	var polygon1 = CAG.fromPoints ( [[-24.12,0], [-24.12,2.828], [-17.588,2.828], [-11.133,9.438], [-6,19.782], [-1.467,0], [-24.12,0] ] );
	
	var sketch1 = polygon1;
	var Revolution1 = rotate_extrude(sketch1);

	var OskarTheGreat = Revolution1;

	return OskarTheGreat;
}

