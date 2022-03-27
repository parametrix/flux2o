<?php header('Content-type: text/html; charset=utf-8'); ?>

<link rel="stylesheet" type="text/css" href="css/viewer_styles.css">

<script src="js/raphael.min.js" ></script>


</head>
<body>
  <!-- SIDE MENU -->
	<div id="mySidenav" class="sidenav">
		<a href="javascript:void(0)" class="closebtn" onclick="closeNav()">&times;</a>
    <!-- PROJECT SELECTOR ELEMENT -->
    <hr>
    <h1>flux2o</h1>
    <hr>
    <h4>PROJECTS:</h4>
    <select onchange="selectProject(this.value)" id="projectSelector" >
      <?php foreach($projects as $project):?>
        <option value=<?php echo $project['uid'];?>><?php echo $project['filename']; ?></option>
      <?php endforeach; ?>
    </select>
    
    <h4>LEVELS:</h4>
    <div id="levelSelector">
    </div>
	</div>
	<div style="font-size:30px;cursor:pointer;background-color:#666666;" onclick="openNav()">&#9776;</div>

  <!-- RAPHAEL CANVAS -->
  <div id="wrap"></div>


  <!--SIDE MENU SCRIPT-->
	<script>
		function openNav() {
				document.getElementById("mySidenav").style.width = "250px";
		}

		function closeNav() {
				document.getElementById("mySidenav").style.width = "0";
		}
    
    <!-- GLOBAL VARIABLES -->
    var paper;
    var curr_projectId;
    var project_json;
		var transform;
    var rooms;
    var doors;
    var levels;
    var furnishings;
    
    <!-- DOCUMENT ONLOAD -->
    window.onload = function() {
      paper = Raphael("wrap");
      // !! As of Raphael 2.1.0, specifying true as the `fit` parameter to `setViewBox()` translates into an invalid value for the SVG `preserveAspectRatio` attribute: "meet". To rule out this problem, we set that attribute directly.
      paper.canvas.setAttribute('preserveAspectRatio', 'none'); // always scale to fill container, without preserving aspect ratio.
      // select the first project
      var project_0 = "<?php echo $projects[0]['uid'] ?>";
      selectProject(project_0);
    }
    
    
    <!-- PROJECT SELECTOR FUNCTION -->
    function selectProject(projectId){
      curr_projectId = projectId;
      var http = new XMLHttpRequest();
      var url = 'getAllProjectData.php';
      var params = 'uid='+projectId;
      http.open('POST', url, true);

      //Send the proper header information along with the request
      http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');

      http.onreadystatechange = function() {//Call a function when the state changes.
          if(http.readyState == 4 && http.status == 200) {
              decodeProjectJson(http.responseText);
          }
      }
      http.send(params);
    }
    
    <!-- DECODE JSON AND SET LEVELS -->
    function decodeProjectJson(server_response){
      // remove additional indicators from json <flux2o> and possibly trailing 0
      jsonstring = server_response.replace('<flux2o>','');
      project_json = JSON.parse(jsonstring);
      rooms = project_json.filter(x=>x.category==="Rooms");
      if(rooms==''){ alert("No Rooms Found");}
      doors = project_json.filter(x=>x.category==="Doors");
      levels = getLevels(rooms);
      furnishings = project_json.filter(x=>x.category!=="Doors" && x.category!=="Rooms");
      createSideNavATags(levels, project_json, paper);
      // draw plan for first level
      drawRoomsOnLevel(levels[0])
    }
    
    <!-- HELPER FUNCTION GET LEVELS FROM JSON -->
    function getLevels(data){
      var levels = []
      var lookup = {};
      for(var i=0;i<data.length;i++)
      {
        var name = data[i].levelName;
        if(typeof name=='undefined'){continue;}
        if(!(name in lookup)){
          lookup[name]=1;
          levels.push(name);
        }
      }
      return levels.sort();
    }
    
    <!-- CREATE SIDE NAVIGATION LINKS-->
    function createSideNavATags(levels, data, paper){
      var sidenav = document.getElementById("levelSelector");
      // clear existing level notes
      while (sidenav.firstChild) {
          sidenav.removeChild(sidenav.firstChild);
      }
      if(levels.length<1){ alert("No Levels Found!");return;}
      for(var i=0;i<levels.length;i++){
        var aTag = document.createElement('a');
        aTag.setAttribute('href',"javascript:drawRoomsOnLevel(\""+levels[i]+"\");");
        aTag.innerHTML = levels[i];
        sidenav.appendChild(aTag);
      }
    }
    
    <!-- DRAW ROOMS ON LEVEL -->
    function drawRoomsOnLevel(levelName){
      paper.clear();
      var roomsOnLevel = project_json.filter(obj=>{
        return obj.levelName===levelName
      });
      var room_objects = roomsOnLevel.filter(obj=>{
        return obj.category==="Rooms"
      });
      var svgpaths = [];
      rooms = []; // this is used later
      var xcoords=[]; var ycoords=[];
      maxX=maxY=minX=minY=0;
      for(var i=0;i<room_objects.length;i++){
        var room = JSON.parse(room_objects[i].rawjson); // parse out the raw json
        rooms.push(room);
        var paths = room.svgPaths;
        // figure out max and mins
        for(var j=0;j<paths.length;j++){
          // trim all characters from string to get coordinates
          var path = paths[j].replace(/\D/,'');
          var coords = path.split(' ');
          for(k=0;k<coords.length;k++){
            var xy = coords[k].split(',');
            xcoords.push(parseFloat(xy[0]));
            ycoords.push(parseFloat(xy[1]));
          }
          svgpaths.push(paths[j]);
        }
      }
      maxX = Math.max(...xcoords);
      minX = Math.min(...xcoords);
      maxY = Math.max(...ycoords);
      minY = Math.min(...ycoords);
      console.log(maxX);
      console.log(minX);
      console.log(maxY);
      console.log(minY);
      // GET TRANSFORM FOR VIEW BOX
      w = maxX-minX+25; //console.log(w);
      h = maxY-minY+25; //console.log(h);
      paper.setViewBox(0, 0, w, h, true);
      transform = "t"+w/2.75+","+h/2.60;
      // DRAW PATHS
      for(var i=0;i<rooms.length;i++){
        //console.log(rooms[i]);
        
        if(rooms[i].svgPaths.length==1)
        {
          paper.path(rooms[i].svgPaths[0]).transform(transform).attr("stroke","#ffffff").attr("stroke-width", 0.35).attr("fill","#ffffcc").attr("fill-opacity","0.15");
        }
        else{
          // add paths together
          var compoundpath = rooms[i].svgPaths[0];
          for(var j=1;j<rooms[i].svgPaths.length;j++){
            compoundpath=compoundpath+"+"+rooms[i].svgPaths[j];
          }
          paper.path(compoundpath).transform(transform).attr("stroke","#ffffff").attr("stroke-width", 0.45).attr("fill","#ffffcc").attr("fill-opacity","0.15");
        }
        var name = rooms[i].name.toUpperCase();
        var loc = rooms[i].location;
        paper.text(loc.X,loc.Y,name).transform(transform).attr("font-family","Arial").attr("font-size","2"); 
      }
      // now draw doors
      drawDoors(transform, levelName);
      
      getUpdates(transform,levelName);
      
      //drawFurnishing(levelName); //- DOES NOT TRANSFORM CORRECTLY
    }
    
    <!-- DRAW DOORS ON LEVEL ********** called from drawRooms()-->
    function drawDoors(roomtransform, levelName){
      var doorsOnLevel = doors.filter(obj=>{
        return obj.levelName===levelName
      });

      for(i=0;i<doorsOnLevel.length;i++){
        door_obj = doorsOnLevel[i];
        door = JSON.parse(door_obj['rawjson']);
        if(null==door.location){continue;}
        paper.circle(door.location.X,door.location.Y,door.width/2).transform(roomtransform).attr("stroke","#ffffff").attr("stroke-width", 0.25).attr("fill", "#669900");
        // draw a unit path in facing orientation of door
        var endptX = door.location.X+door.facingorientation.X*door.width;
        var endptY = door.location.Y+door.facingorientation.Y*door.width;
        var dirline = paper.path("M"+door.location.X+" "+door.location.Y+"L"+endptX+" "+endptY).transform(roomtransform).attr("stroke","#f00").attr("stroke-width", 0.25);
      }
    }
    
    <!-- DRAW FURNISHINGS ON LEVEL ********** called from drawRooms()--!!!!!!!!! DOES NOT TRANSFORM CORRECTLY>
    function drawFurnishing(levelName){
      var furnOnLevel = furnishings.filter(obj=>{
        return obj.levelName===levelName
      });
      for(i=0;i<furnOnLevel.length;i++){
        furn_obj = furnOnLevel[i];
        furn = JSON.parse(furn_obj['rawjson']);console.log(furn);
        // draw svg loops
        if(null==furn['svgPaths']){continue;}
        var compoundpath = furn['svgPaths'][0];
        for(j=1;j<furn['svgPaths'].length;j++){
          compoundpath=compoundpath+"+"+furn['svgPaths'][j];
        }
        //console.log(compoundpath);
        var loc = furn.location;
        var rotation = Math.degrees(furn.rotation);
        var symbol = paper.path(compoundpath).attr("stroke","#ffffff").attr("stroke-width", 0.15).attr("fill","#ffffcc").attr("fill-opacity","0.15").attr("title",furn.uid);
        //symbol.transform("t0,0");
        // try drawing symbols for furniture
        var loc = furn['location'];  
        paper.circle(loc.X,loc.Y,2).transform(transform).attr("fill","#ffffcc").attr("fill-opacity","0.15").attr("title",furn.uid);
        symbol.transform(transform);
        
      }
    }
    
    <!-- GET EGRESS PATH UPDATES ---->
    function getUpdates(roomtransform, levelName){
      var http = new XMLHttpRequest();
      var url = 'getUpdates.php';
      var params = 'query=GET_UPDATES&uid='+curr_projectId;
      http.open('POST', url, true);

      //Send the proper header information along with the request
      http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');

      http.onreadystatechange = function() {//Call a function when the state changes.
          if(http.readyState == 4 && http.status == 200) {
              if(http.responseText!=='ERROR:TABLE_DOES_NOT_EXIST'){
                decodeUpdates(http.responseText);
              }
          }
      }
      http.send(params);
    }
    
    function decodeUpdates(updates_str){
      if(null==updates_str){return;}
      updates = updates_str.replace('<flux2o>','');
      var json_objs = JSON.parse(updates);
      
      // interate to object
      for(i=0;i<json_objs.length;i++){
        var pts = JSON.parse(json_objs[i].pointjson); // convert string to objects
        // initalize the path
        
        var firstpt = pts[0].split(',');
        var path_str = "M"+firstpt[0]+" "+firstpt[1]+"L";
        for(j=1;j<pts.length;j++){
          var pt = pts[j].split(',');
          path_str=path_str+pt[0]+" "+pt[1];
        }
        // draw path
        paper.path(path_str).transform(transform).attr("stroke","#f00").attr("stroke-width", 0.25);;
      }
    }
    
    
    // ******************** UTILITY FUNCTIONS *******************/
    // Converts from degrees to radians.
    // from: http://cwestblog.com/2012/11/12/javascript-degree-and-radian-conversion/
    Math.radians = function(degrees) {
      return degrees * Math.PI / 180;
    };
     
    // Converts from radians to degrees.
    Math.degrees = function(radians) {
      return radians * 180 / Math.PI;
    };
    
    
</script>
