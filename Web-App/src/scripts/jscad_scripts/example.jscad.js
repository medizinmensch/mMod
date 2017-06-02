function main(params) {
    return (union(
        difference(
            cube({size: params.cubeSize1, center: true}),
            sphere({r:params.sphereRadius1, center: true})
        ),
        intersection(
            sphere({r: params.sphereRadius2, center: true}),
            cube({size: params.cubeSize2, center: true})
        )
    ).translate([params.translate1, params.translate2, params.translate3]).scale(params.scale)).setColor(0, 1, 1);
}

function getParameterDefinitions() {
    return [
        { name: 'cubeSize1', caption: 'size of difference-cube:', type: 'float', default: 3.0, step:0.1 },
        { name: 'sphereRadius1', caption: 'radius of difference-sphere:', type: 'float', default: 2.0, step:0.1 },
        { name: 'sphereRadius2', caption: 'radius of intersection-sphere:', type: 'float', default: 1.3, step:0.1 },
        { name: 'cubeSize2', caption: 'size of intersection-cube:', type: 'float', default: 2.1, step:0.1 },
        { name: 'translate1', caption: 'translate1:', type: 'float', default: 0.0, step:0.1 },
        { name: 'translate2', caption: 'translate2:', type: 'float', default: 0.0, step:0.1 },
        { name: 'translate3', caption: 'translate3:', type: 'float', default: 1.0, step:0.1 },
        { name: 'scale', caption: 'scale:', type: 'float', default: 10, step:0.1 }
    ];
}