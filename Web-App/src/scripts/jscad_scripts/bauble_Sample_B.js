/* created by jakob 06.10.16
 * 
 * 
 * 
 */ 


function getParameterDefinitions() {
	// see http://spencermortensen.com/articles/bezier-circle/
    var c = 0.551915024494;
    var lower_quarter = [0,1, c,1, 1,c, 1,0];
    var upper_quarter = [1,0, 1,-c, c,-1, 0,-1];
    for(var i = 0; i < 4; i++) {
        lower_quarter[2*i] *= 0.25;
        lower_quarter[2*i+1] *= 0.25;
        lower_quarter[2*i+1] += 0.5;

        upper_quarter[2*i] *= 0.25;
        upper_quarter[2*i+1] *= 0.25;
        upper_quarter[2*i+1] += 0.5;
    }

    var half_circle = lower_quarter.concat(upper_quarter.slice(2));
    
    // pattern sets the initial state of pixelgrid. note that the arrays
    // of pattern are the columns of the pixelgrid, where the first array
    // is the very left column. I
    var N = 12;
	var pattern = [];
	for (i = 0; i<N; i++) {
		row = []
		for (var j = 0; j<N; j++) {
			if (j < 2) row.push(true); // creates the solid lines at the top of the pattern.
			else if (j < 6) row.push(((i+j)%2==0)); //creates the chessboard pattern.
			else row.push(false); // creates the empty part at the bottom half.
		}
		pattern.push(row);
	}
		
	
    //console.log(half_circle);
    return [
        {name: 'bezier1', 		  type: 'bezier', 	initial: half_circle,   		                  container_id: "bezier-container1"},
        {name: 'pixelgrid',		  type: 'pixel',	initial: pattern,    patternsize: N,			  container_id: "pixelgrid"},
        {name: 'radius', 		  type: 'slider', 	initial: 3, 		 min:2.5,    max:3.5, step:0.1,  caption: 'Ring radius'},
        {name: 'thickness',    	  type: 'slider', 	initial: 1, 		 min:0.5,  max:1.2, step:0.05, caption: 'Ring thickness'},
        {name: 'label',			  type: 'text', 	initial: 'bauble_Sample_B.js',			  		   caption: 'Your words on ball'},
        {name: 'textsize',		  type: 'slider',   initial: 15,		 min:10,   max:40,  step:3,    caption: 'Letter size'}
		
    ];
}


function main(params) {
	// resolution defines how deatiled the ball is going to be. it will have 'resolution' 
	// faces from north to south and 2*'resolution' faces in its circumference.
	var resolution = 48;
		
	// Takes the points on bezier curve and rotates them along the z axis to create ball.		    
    var lut = params.bezier1.getLUT(resolution);

    var points = lut.map(function(p) {
       return [p.x*50, (-p.y+0.5)*50];
    });
    try {
        var polyg = polygon({points:points});
        ball = rotate_extrude({fn:2*resolution}, polyg).setColor(0.6,0.6,0.6);
    } catch (err) {
        throw "Not Printable!";
    }

    // if (params.spikeoption) spikify(ball, params.spikelength, params.label);
    
    // creates the pixel patterns according to pixelgrid.
    extrudePixels(ball, params.pixelgrid.getMatrix(), params.bezier1, resolution, 0.15);
    
    // creates the ring.
    var ring = addRing(params.radius, params.thickness);
	
	// creates the text.
	var text = addText(params.label, params.bezier1, 2, params.textsize);

	return [ball, text.setColor(0.3,0.3,0.3), ring.setColor(0.6,0.6,0.6)];
    
    // return stlfiles.house.scale([0.1, 0.1, 0.1])
}

// works through a CSG ball face by face and if the pattermatrix is true builds a pixel on top
// of that face. The order in which we go through the faces is column by column from east to west
// and each column South to North.

function extrudePixels(ball, matrix, bezier, resolution, height){
	
	height = height || 1;
	var equator = resolution/2;
	var patternsize = matrix.length;
	var N = ball.polygons.length;
	
	// We have an offset that leaves enough space for the letters. 
    var letterframe = Math.ceil(resolution/20) +1;
    var offset_south = equator - letterframe - patternsize + 1;
    var offset_north = equator + letterframe + 1;
    if (offset_south < 1) throw new Error("Pixelgrid doesn't fit on hemisphere. Either change resolution or decrease size of grid.")
    
    var i = 0, j = 0, column = -1; 
    var patterncolumn, patternrow; // needed to go through the matrix
    var left_south, above_south, right_south, below_south; // needed because we don't want to build faces between neighbouring pixels
    var left_north, above_north, right_north, below_north; // that won't be seen anyway.
    
	for (i; i<N; i++){
		var posY = i % (resolution+1); // cell position from south to north
		if (posY === 0) column++; // column position from east to west
    	face = ball.polygons[j]; // after we built a pixel we delete the face beneath. Therefore we have j that only
								 // gets incremented only if we don't delete a cell so we avoid skipping faces.
    	
    	
    	if (posY < equator) {	// Here we build the pattern on the southern hemisphere
			patterncolumn = column%patternsize;
			patternrow = posY-offset_south

			// check if we are in the are we want to add the pattern.
    	    if (posY < offset_south || posY > offset_south + patternsize) {
				j++;
				continue;
			}
			
			// if the Matrix cell is true, we extrude the face.
			if (matrix[patterncolumn][patternrow]) {
				
				// Here we check if neighbouring cells are false in order to add walls later.
				if (patterncolumn === 0) left_south = matrix[patternsize-1][patternrow];
				else left_south  = matrix[patterncolumn-1][patternrow];
			
				if (patterncolumn === patternsize-1) right_south = matrix[0][patternrow];
				else right_south = matrix[patterncolumn+1][patternrow];
			
				if (patternrow === 0) above_south = false;
				else above_south = matrix[patterncolumn][patternrow-1];
			
				if (patternrow === patternsize-1) below_south = false;
				else below_south  = matrix[patterncolumn][patternrow+1];
				
				// old and new vertices that we want to connect.
				var vertices_old = []; 
				face.vertices.forEach(function(v){
					vertices_old.push(v.pos);
				});
				// Here we create the vertices floating above the old face.
				var vertices_new = bezierExtrudeFace(posY, column, bezier, resolution, height);
				
				// adding faces if they need to be added
				addFace(ball, vertices_new);
				ball.polygons.splice(j,1);
				if (!left_south)  addFace(ball, [vertices_old[0],vertices_old[1],vertices_new[1], vertices_new[0]]);
				if (!below_south) addFace(ball, [vertices_old[1],vertices_old[2],vertices_new[2], vertices_new[1]]);
				if (!right_south) addFace(ball, [vertices_old[2],vertices_old[3],vertices_new[3], vertices_new[2]]);
				if (!above_south) addFace(ball, [vertices_old[3],vertices_old[0],vertices_new[0], vertices_new[3]]);
				
			} else {
			j++;

			}
		} else { // Here we build the pattern on the northern Hemisphere. Exactly the same as above only flipped.
			patterncolumn = patternsize - column%patternsize -1;
			patternrow = patternsize - posY + offset_north -1;

    	    if (posY < offset_north || posY > offset_north + patternsize) {
				j++;
				continue;
			}
			
			
			if (matrix[patterncolumn][patternrow]) {
				if (patterncolumn === 0) left_south = matrix[patternsize-1][patternrow];
				else left_south  = matrix[patterncolumn-1][patternrow];
				
				if (patterncolumn === patternsize-1) right_south = matrix[0][patternrow];
				else right_south = matrix[patterncolumn+1][patternrow];
				
				if (patternrow === 0) above_south = false;
				else above_south = matrix[patterncolumn][patternrow-1];
				
				if (patternrow === patternsize-1) below_south = false;
				else below_south  = matrix[patterncolumn][patternrow+1];

				var vertices_old = []; 
				face.vertices.forEach(function(v){
					vertices_old.push(v.pos);
				});
				var vertices_new = bezierExtrudeFace(posY, column, bezier, resolution, height);
				
				addFace(ball, vertices_new);
				ball.polygons.splice(j,1);
				if (!right_south)  addFace(ball, [vertices_old[0],vertices_old[1],vertices_new[1], vertices_new[0]]);
				if (!above_south) addFace(ball, [vertices_old[1],vertices_old[2],vertices_new[2], vertices_new[1]]);
				if (!left_south) addFace(ball, [vertices_old[2],vertices_old[3],vertices_new[3], vertices_new[2]]);
				if (!below_south) addFace(ball, [vertices_old[3],vertices_old[0],vertices_new[0], vertices_new[3]]);
				
			} else {
			j++;
			}
		
		}

	}
	return ball;
	
}

// This function is a helper function for extrudePixels() and is resposible for creating the top
// face of the pixel. we have to make sure, that the face is parallel to the face on the ball
// and that the new vertices of the new face are positioned o the normals of the old vertices.
//
// However, because we need the normals of bezier curve and not the normal of the face, we can't use 
// only the face as an input, we have to take its position on the ball into account. For that we take
// advantage of the fact, that for the ball the faces are stored column by column from east to west 
// (parameter column, we have 2*resolution columns) and on every column from south to north (parameter 
// posY, we have resolution rows). 

function bezierExtrudeFace(posY, column, bezier, resolution, height){

	var vertices_new = [];
	var x,y,z;
	
	var t1 = (posY-1) /resolution;
	var t2 =  posY    /resolution;
	var alpha1 =  column   *180/resolution;
	var alpha2 = (column+1)*180/resolution;
	
	// TODO: Could improve performance by just using the LUT of main() and a list of normals
	// 		because these values are the same for every column.
	
	var point1 = bezier.get(t1);
	var point2 = bezier.get(t2);
	var normal1 = bezier.normal(t1);
	var normal2 = bezier.normal(t2);
	
	var point_list  = [point1,  point2,  point2,  point1]
	var normal_list = [normal1, normal2, normal2, normal1]
	var alpha_list  = [alpha1,  alpha1,  alpha2,  alpha2]
	
	for (var i=0; i<4; i++){ 
		x = cos(-alpha_list[i])*(point_list[i].x*50 + height*normal_list[i].x);
		y = sin(-alpha_list[i])*(point_list[i].x*50 + height*normal_list[i].x);
		z = (-point_list[i].y+0.5)*50  - height*normal_list[i].y;  

		vertices_new.push(new CSG.Vector3D(x,y,z)) ;
	}
	
	return vertices_new;
}
	


function addFace(geometry, list_of_vertices){
	var vertices = [];
	list_of_vertices.forEach(function(v){
		vertices.push(new CSG.Vertex(new CSG.Vector3D(v.x, v.y, v.z)));
	});
	face = new CSG.Polygon(vertices, false);

	geometry.polygons.push(face.setColor(0.3,0.3,0.3));

    return;
}

// adds the ring.
function addRing(radius, thickness){
	var ring = rotate_extrude( translate([radius,0,0], circle({r: thickness, fn: 30, center: true}) ) );
	ring = ring.rotateX(90);
	console.log(radius, thickness);
	ring = ring.translate([0,0,12.5+radius+thickness]);
	
	return ring;
}

// Creates letter for the inscription of the christmas ball. To make it ready for later 
// deformation, it partitions the paths of the vectortext into smaller pieces if needed.
// it then extrudes the path to make it into workable 3D-objects.

function addText(string, bezier, height, fontsize, bold) {
    var path = vector_text(0,0,string), empty_path = [];

	// Here every path is checked for its length and partitioned if too long.
    path.forEach(function(line) {
		new_path = [];
		for (var j = 1; j < line.length; j++){
			var X = line[j-1], Y = line[j];
			var d = Math.floor(distance(X, Y));
			if (d>6) {
				d = d + (6-(d%6));
				var i
				if (j===1) i = 0; else i = 6;
				for (i; i<=d; i+=6){
					var x_1 = (i/d) * Y[0] + (1-i/d) * X[0];
					var y_1 = (i/d) * Y[1] + (1-i/d) * X[1];
					new_path.push([x_1,y_1]);
				}
			} else {
				if ( j === 1 ) new_path.push(X);
				new_path.push(Y);
			}
		}
		empty_path.push(new_path);
    });
    
    //console.log(path, empty_path);

    path = extrudeAlternative(empty_path, bezier, height, fontsize);
    return path;
}

function extrudeAlternative(path, bezier, height, fontsize){
	var width = fontsize/10 || 1;
	var height = height || 4;
	var fontsize = fontsize || 10
	var geometry = new CSG(); //Going to be the distorted text.
	
	path.forEach(function(stroke){

		/* The idea is to turn a two-dimensional path on the XY-plane into a 
		 * three-dimensional object by exturding it rectangularly and distorting the result
		 * with regards to the bezier curve. To save calculation time, we only
		 * create the final object and not the simply extruded state in between, by 
		 * the cost of making this part of the code less readable.
		 */
		var N = stroke.length, i=0, v1,v2,v3,v4,v5,v6,v7,v8, vertex0, vertex1, vertex2, direction;
		while (true){
			//assignment also means to bezierDistort them.
			vertex0 = stroke[i];
			vertex1 = stroke[i+1];
			if (i===0){
				// In this case, direction is found by rotating the path vector 90° counterclockwise
				direction = [vertex0[1]-vertex1[1], vertex1[0]-vertex0[0]];
				
				[v1,v2,v3,v4] = findFourSupportingPoints(vertex0, direction, bezier, width, fontsize, height);

				geometry.polygons.push(new CSG.Polygon([v1,v2,v3,v4], false));

				old_vertices = [v1,v2,v3,v4];
		
			}
			if (i === N-2){
				[v1,v2,v3,v4] = old_vertices;
				// In this case, direction is found by rotating the path vector 90° counterclockwise
				direction = [vertex0[1]-vertex1[1], vertex1[0]-vertex0[0]];
				
				[v5,v6,v7,v8] = findFourSupportingPoints(vertex1, direction, bezier, width, fontsize, height);
				geometry.polygons.push(new CSG.Polygon([v5,v8,v7,v6], false));				
				geometry.polygons.push(new CSG.Polygon([v1,v5,v6,v2], false));				
				geometry.polygons.push(new CSG.Polygon([v2,v6,v7,v3], false));				
				geometry.polygons.push(new CSG.Polygon([v3,v7,v8,v4], false));				
				geometry.polygons.push(new CSG.Polygon([v4,v8,v5,v1], false));	
				break;	
				
			} else {
				[v1,v2,v3,v4] = old_vertices;
				vertex2 = stroke[i+1];
				// In this case, direction is found by rotating the vector between the first vertex and next after the next.
				direction = [vertex0[1]-vertex2[1], vertex2[0]-vertex0[0]];
				[v5,v6,v7,v8] = findFourSupportingPoints(vertex1, direction, bezier, width, fontsize, height);

			
				geometry.polygons.push(new CSG.Polygon([v1,v5,v6,v2], false));				
				geometry.polygons.push(new CSG.Polygon([v2,v6,v7,v3], false));				
				geometry.polygons.push(new CSG.Polygon([v3,v7,v8,v4], false));				
				geometry.polygons.push(new CSG.Polygon([v4,v8,v5,v1], false));
			    old_vertices = [v5,v6,v7,v8];
			}
			i++;
		}
		
	});

	return geometry;
	
}

//takes vecrtices belonging to a flat threedimensional objekt on the xy-plane and wraps it around a bezier
// object, by turning it into scalar coordinates.
function bezierDistort(x,y,z, fontsize, bezier){
	x *= fontsize*0.008*Math.PI;
	y = (y-10.5)*fontsize*0.01;
	z *= fontsize*0.01;
    var t = 0.5 + 0.02*y;
	var point = bezier.get(t);
	var normal = bezier.normal(t);

	var a = cos(x-70)*(point.x*50 + z*normal.x);
	var b = sin(x-70)*(point.x*50 + z*normal.x);
	var c = (-point.y+0.5)*50  - z*normal.y;   

    return new CSG.Vertex(new CSG.Vector3D(a, b, c));
}

// helper function for extrudeAlternative(). Takes point on a path and finds four points on a rectangle
// that's perpendicular to path if the point is a starting point of the path or thats dividing the angle between the path segments.
function findFourSupportingPoints(vertex, direction, bezier, width, fontsize, height){
	// With direction and findpoint() we find two points, one to the left, one on the 
	// right of the path, equidistant and perpendicular path at vertex0.
	var helppoint1 = findPointOnLine(vertex, direction, width);
	var helppoint2 = findPointOnLine(vertex, direction, -width);
	// ...and then we find our points in the third dimension by simply going above and below,
	// and immediately distorting them.
	var v1 = bezierDistort(helppoint1[0], helppoint1[1], - height/2, fontsize, bezier);
	var v2 = bezierDistort(helppoint2[0], helppoint2[1], - height/2, fontsize, bezier);
	var v3 = bezierDistort(helppoint2[0], helppoint2[1], height/2, fontsize, bezier);
	var v4 = bezierDistort(helppoint1[0], helppoint1[1], height/2, fontsize, bezier);

	return [v1,v2,v3,v4];	
}

// --- helper functions --- //

function distance(vector, vector2){
	var sum = 0;
	if (!vector2) {
		vector.forEach(function(x){
		sum += x*x;
		});
	
		return Math.sqrt(sum);
	}
	var N = vector.length; i
	if (vector2.length != N) throw "vector and vector1 have different dimensions"
	
	for (var i = 0; i<N; i++){
		sum += Math.pow((vector2[i]-vector[i]), 2);
	}
	
	return Math.sqrt(sum);
}

function findPointOnLine(start_point, line_direction, awayness){
	var N = start_point.length;
	if (line_direction.length != N) throw "start_point and distance have different dimensions.";
	var point = [];
	var length = distance(line_direction);
	for (var i = 0; i < N; i++){
		point.push(start_point[i] + awayness*line_direction[i]/length);
	}
	return point;
}

// takes a 'geometry' and turns all its quads and turns them into spikes of length 'length'.
// NOT USED
//~ function spikify(geometry, length){
	//~ length = length || 1;
	//~ var N = 10;
    //~ //var N = geometry.polygons.length;
    //~ var i = 0, j = 0;
    
    
	//~ for (i=0; i<N; i++){
    	//~ face = geometry.polygons[j];
    	//~ console.log(face);
    	//~ if (face.vertices.length != 4 || !face.plane.normal.x) {
    	    //~ j++;
    	    //~ continue;
    	//~ }
    	//~ n = face.plane.normal;
    	
    	//~ var v = new CSG.Vertex(0,0,0);
    	//~ var v1 = face.vertices[0].pos;
    	//~ var v2 = face.vertices[1].pos;
    	//~ var v3 = face.vertices[2].pos;
    	//~ var v4 = face.vertices[3].pos;
	
		//~ // here we calculate the pointy end for the spike by adding the normal of the plane
		//~ // 'length' times onto the center of the quad.
        //~ v.x = (v1.x + v2.x + v3.x + v4.x)*0.25+length*n.x;
    	//~ v.y = (v1.y + v2.y + v3.y + v4.y)*0.25+length*n.y;
    	//~ v.z = (v1.z + v2.z + v3.z + v4.z)*0.25+length*n.z;
    		
    	//~ geometry.polygons.splice(j,1);
    	//~ addFace( geometry, [v1,v2,v] );
    	//~ addFace( geometry, [v2,v3,v] );
    	//~ addFace( geometry, [v3,v4,v] );
    	//~ addFace( geometry, [v4,v1,v] );
	//~ }
	
	//~ return geometry.setColor(0.6,0.6,0.6);
	
//~ }
