#<div id="start">Start
#</div>
#<svg id="svg-element" xmlns="http://www.w3.org/2000/svg">
#  <path d="M 50 10 c 120 120 120 120 120 20 z" id="p1"></path>
#  <path d="M 150 10 c 120 120 120 120 120 20 z" id="p2"></path>
#</svg>
#<div id="time"></div>
#<div id="result"></div>

#svg {
#  fill: none;
#  stroke: black;
#}
#start {
#  border: 1px solid;
#  display: inline-block;
#  position: absolute;
#}



var path1 = document.getElementById("p1"),
	path2 = document.getElementById("p2"),
	time = document.getElementById("time"),
	btn = document.getElementById("start"),
  result = document.getElementById("result");


btn.addEventListener("click", getIntersection);

function getIntersection() {
	var start = Date.now(),
    path1Length = path1.getTotalLength(),
    path2Length = path2.getTotalLength(),
    path2Points = [];
 
 	for (var j = 0; j < path2Length; j++) {      
   	path2Points.push(path2.getPointAtLength(j));
 	}
  
  for (var i = 0; i < path1Length; i++) {  
    var point1 = path1.getPointAtLength(i);

    for (var j = 0; j < path2Points.length; j++) {
      if (pointIntersect(point1, path2Points[j])) {
        var end = Date.now();
        time.innerHTML = (end - start) / 1000 + "s";
        result.innerHTML = point1.x + ',' + point1.y + ' ' + path2Points[j].x + ',' + path2Points[j].y;
        return;
      }
    }
  }
}

function getIntersectionOriginal() {
var start = Date.now();
  for (var i = 0; i < path1.getTotalLength(); i++) {
    for (var j = 0; j < path2.getTotalLength(); j++) {
      var point1 = path1.getPointAtLength(i);
      var point2 = path2.getPointAtLength(j);

      if (pointIntersect(point1, point2)) {
        var end = Date.now();
        time.innerHTML = (end - start) / 1000 + "s";
        result.innerHTML = point1.x + ',' + point1.y + ' ' + point2.x + ',' + point2.y;
        return;
      }
    }

  }
}

function pointIntersect(p1, p2) {
  p1.x = Math.round(p1.x);
  p1.y = Math.round(p1.y);
  p2.x = Math.round(p2.x);
  p2.y = Math.round(p2.y);
  return p1.x === p2.x && p1.y === p2.y;
}
