/* author: Daniel Nieh
 * source: https://github.com/dnieh/pixel-grid/blob/master/pixelGrid.js 
 * 
 * customed by Moritz Vetter
 * added a matrix attribute to simplify input/output
 * added some interaction tools in the clickEventListener funciton.
 */

var pixelGrid = (function($) {
    var PIXEL_SCALE;
    
	var container = $("#pixelgrid-container");
    var $canvas = $('#pixelgrid');
    var canvas = container[0];
    var currentColor;
    // style
    var BLACK              = '#000';
    var backgroundColor    = '#fff';
    var gridcolorUnit      = '#999';
    var gridcolorEverySix  = '#a2a2a2';
    var gridOffset = 6;
    var MaxPIXEL_SCALE = 20;
    
    var pxWidth;
    var pxHeight;
    var c; // context
    var cachedImage;
	var matrix;

    function setDimensions(widthInput, heightInput) {
        // Save a copy of the current image
        var dataURL = $canvas[0].toDataURL();
        var image = new Image();

        image.src = dataURL;
        cachedImage = image;

        if (widthInput !== -1) {
            $canvas.attr('width', widthInput);
            width = widthInput;
        }
        if (heightInput !== -1) {
            $canvas.attr('height', heightInput);
            height = heightInput;
        }
    }


	// Here the interaction between mouse and canvas is defined, 
	// which for the most part aims to fill pixels with color. The
	// mechanics of this is handled by fillPixelAtPosition
    function clickEventListener() {
		var buttonDown = false;
		var mouseMove = false;
		
		// For 'click' we want the pixel filled with the complementary color
		// (black-->white, white-->black).
        $canvas.on('click', function(e) {
			
			if (!mouseMove) {	
				var x;
				var y;
				var i;
				// Get the relative position (offset)
				x = e.offsetX; // column or x-axis
				y = e.offsetY; // row or y-axis
				
				var imgDat = c.getImageData(x,y,2,2);
				// TODO: Check if clicked pixel is black, if so set currentColor to white.
				for (i = 0; i < 16; i+=4){	
					var r = imgDat.data[i];
					if (r === 0) currentColor = backgroundColor;
					else currentColor = BLACK;
				}
					
				fillPixelAtPosition(x,y);		
			}
        });
        
        // these determine the value of buttonDown.
        $canvas.on('mousedown', function(e) {
			buttonDown = true;
		});
			
		$canvas.on('mouseup', function(e) {
			buttonDown = false;
		});
		
		// here the possibility to color multiple pixels by dragging
		// the mouse while holding the button is defined.
		$canvas.on('mousemove', function(e) {
			// We set the color befor we click. That means one determines
			// the color before dragging and can't change it until the end 
			// of the stroke.
			if (!buttonDown) {
				mouseMove = false;
				// Get the relative position (offset)
				var x = e.offsetX; // column or x-axis
				var y = e.offsetY; // row or y-axis
				var imgDat = c.getImageData(x,y,2,2);
				// Check if clicked pixel is black. 
				for (var i = 0; i < 16; i+=4){	
					var r = imgDat.data[i];
					if (r === 0) currentColor = backgroundColor;
					else currentColor = BLACK;
				}
					
			}
			if (buttonDown) {
				mouseMove = true;
				var x = e.offsetX; // column or x-axis
				var y = e.offsetY; // row or y-axis
				// For this don't make edges of pixels interactive, to 
				// make painting diagonally easier.
				if (x%PIXEL_SCALE >= 4 && x%PIXEL_SCALE <= 16 && y%PIXEL_SCALE >= 4 && y%PIXEL_SCALE <= 17 ){
					fillPixelAtPosition(x,y);
				}
			}

		});
    }
    
    function fillPixelAtPosition(x,y){
		// Determine which pixel representation we're on. For example,
		// if the (x, y) coordinates are (8, 8), then we want to color
		// in the square starting from (1, 1) through (9, 9) while leaving
		// the border the existing grid colors of grey and red.
		// At the same time prepare to change Matrix in according cell.
		var intX = Math.floor(x / PIXEL_SCALE);
		var intY = Math.floor(y / PIXEL_SCALE);
		var startX = intX * PIXEL_SCALE + 1;
		var startY = intY * PIXEL_SCALE + 1;
		matrix[intX][intY] = (currentColor === BLACK);

		// Fill the square with the selected color
		c.fillStyle = currentColor;
		c.fillRect(startX, startY, PIXEL_SCALE-1, PIXEL_SCALE-1);

		// Update the live render and the css code output
		x = getScaledCoordinate(startX);
		y = getScaledCoordinate(startY);
		//$canvas.trigger('gridPixelAdded', [x, y, currentColor]);
	}

    function getScaledCoordinate(coordinate) {
        return (coordinate - 1) / PIXEL_SCALE;
    }

    /**
     * @param gWidth {number} grid width
     * @param gHeight {number} grid height
     * @param preserveCurrentImage {bool}
     */
    function draw(gWidth, gHeight, preserveCurrentImage, matrix) {
        preserveCurrentImage = preserveCurrentImage || false;

        // Light grey background
        c.fillStyle = backgroundColor;
        c.fillRect(0, 0, gWidth, gHeight);

        // Grey grid
        c.fillStyle = gridcolorUnit;
        for (var i = 0; i <= gWidth || i < gHeight; i += PIXEL_SCALE) {
			// i = Math.round(i);
            c.fillRect(i, 0, 1, gHeight);
            c.fillRect(0, i, gWidth, 1);
        }

        // Red overlay grid
        c.fillStyle = gridcolorEverySix;
        for (var i = 0; i <= gWidth || i < gHeight; i+= gridOffset*PIXEL_SCALE) {
			// i = Math.round(i);
            c.fillRect(i, 0, 1, gHeight);
            c.fillRect(0, i, gWidth, 1);
        }

        // Re-draw cached image if it exists
        if (cachedImage && preserveCurrentImage) {
            c.drawImage(cachedImage, 0, 0);
        } 
        
        if (matrix) {
			c.fillStyle = BLACK;
			for (var i = 0; i < gWidth/PIXEL_SCALE-1; i++){
				for (var j = 0; j < gWidth/PIXEL_SCALE-1; j++){
					if (matrix[i][j]) {
						c.fillRect(Math.round(i*PIXEL_SCALE)+1,Math.round(j*PIXEL_SCALE)+1, Math.round(PIXEL_SCALE)-1, Math.round(PIXEL_SCALE)-1);
					}
				}
			} 
		}
    } 

    // Redraw canvas when window size changes
    $(window).resize(function(){
        canvas.width = container.width(); 
        var int = matrix.length;
        PIXEL_SCALE = Math.floor(canvas.width/int);
        init(int, PIXEL_SCALE, matrix);
        draw(pxWidth, pxHeight, false, matrix); 
    });

    
    function clearGridListener() {
        $('#clearButton').on('click', function() {
            clear();
        });
    }

    function validateDimensionInput(side) {
        if (isNaN(side)) {
            return false;
        } else if (side < 0 || side > 1000) {
            return false;
        } else {
            return true;
        }
    }

	function setMatrix(intWidth, intHeight) {
		if (isNaN(intWidth) || isNaN(intHeight)) {
			console.error("Please enter valid numbers for matrix' dimensions");
		} 
		matrix = [];
		var i,j;
		for (i = 0; i<intWidth; i++){
			var column = [];
			for (j = 0; j < intHeight; j++){
				column.push(false);
			}
			matrix.push(column);
		}
	}	
    
    //==========================================================================
    // PUBLIC API
    //==========================================================================
    var init = function(intWidth, scale, initialMatrix) {
		container = $('#pixelgrid-container');
		$canvas = $('#pixelgrid');
		canvas = $canvas[0];
		intWidth = intWidth || 10;
		var intHeight = intWidth;
		PIXEL_SCALE = Math.floor(container.width()/intWidth);
        // setInitialWidthAndHeight(intWidth*PIXEL_SCALE, intHeight*PIXEL_SCALE);
        pxWidth =  intWidth*PIXEL_SCALE + 1;
        pxHeight = intHeight*PIXEL_SCALE + 1;
        canvas.width = pxWidth;
        canvas.height = pxHeight;
        
        if (initialMatrix) matrix = initialMatrix;
        else setMatrix(intWidth, intHeight);

        clickEventListener();
        clearGridListener();
        
        if (canvas.getContext) {
            c = canvas.getContext('2d');
            draw(pxWidth, pxHeight, false, initialMatrix);

            // Let width and height input know we're initialized to dynamically
            // set their values
            $canvas.trigger('gridDimensionsInitialized', [pxWidth, pxHeight]);
        } else {
            console.error('Error: Could not get canvas context.');
            return;
        }
    };
    

    var clear = function() {
        init((pxWidth-1)/PIXEL_SCALE, PIXEL_SCALE);
        clickEventListener();
    };

    /**
     * @param widthInput {number}
     */
    var width = function(widthInput) {
        if (!widthInput || !validateDimensionInput(widthInput)) {
            console.error('Error: please specify a valid width.');
            return;
        }
        setDimensions(widthInput, -1);
        draw(widthInput, pxHeight, true);
    };

    /**
     * @param heightInput {number}
     */
    var height = function(heightInput) {
        if (!heightInput || !validateDimensionInput(heightInput)) {
            console.error('Error: please specify a valid height.');
            return;
        }
        setDimensions(-1, heightInput);
        draw(pxWidth, heightInput, true);
    };

    /**
     * Public API Method for single pixel
     * TODO -- validate params
     */
    var colorPixel = function(x, y, color) {
        var startX = x * PIXEL_SCALE + 1;
        var startY = y * PIXEL_SCALE + 1;

        c.fillStyle = color;
        c.fillRect(startX, startY, PIXELSCALE--, PIXELSCALE--);
        startX = getScaledCoordinate(startX);
        startY = getScaledCoordinate(startY);
        $canvas.trigger('gridPixelAdded', [startX, startY, color]);
    };

    /**
     * Public API for JSON list of pixels
     * TODO -- validate params, JSON, and show error message
     */
    var batchColor = function(list) {
        var colors = list.colors;
        var details = list.coordinates;
        var colorCode;

        for (var i in details) {
            colorCode = colors[details[i].color];
            colorPixel(details[i].x, details[i].y, colorCode);
        }
    };
    
    var getMatrix = function(){
		return matrix;	
	}


    return {
        init: init,
        clear: clear,
        width: width,
        height: height,
        colorPixel: colorPixel,
        batchColor: batchColor,
        getMatrix:getMatrix
    };



}($));
