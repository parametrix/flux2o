<?php
  /***************************************/
  set_include_path("../_includes");
  require_once("config.php");
  require_once("constants.php");
  /***************************************/
  
  $data = json_decode(file_get_contents('php://input'), true);
  if(empty($data))
  {
    echo("FAILED_TRANSMISSION");
    exit();
  }
  
  $projectId = $data['projectId'];
  $egressPaths = $data['egressPaths'];
  $useCategory = $data['useCategory'];
    
  if(empty($egressPaths))
  {
    echo("NO_EGRESS_PATHS_FOUND");
    exit();
  }
  
  // check if project exists
  $project = query("SELECT * FROM projects WHERE uid=? LIMIT 1",$projectId);
  if(empty($project))
  {
    echo("PROJECT_NOT_REGISTERED");
    exit();
  }
  // name of rhino db
  $rhProjectId = "rhino".$projectId;
  
  // determine if table exists
  if(empty(query("SHOW TABLES LIKE '".$rhProjectId."'")))
  {
    query("CREATE TABLE IF NOT EXISTS ".$rhProjectId." (uid VARCHAR(32) PRIMARY KEY, pointjson TEXT)");
  }
  
  // else insert the egress paths into the table
  // THIS APP WORKS BY RE-DRAWING ALL THE PATHS IN A GIVEN LEVEL
  
  $pathJson = json_decode($egressPaths, true);

  $levelZ = 0;
  $counter=0;
  foreach($pathJson as $key=>$val)
  {
    // check if key already exists
    $row = query("SELECT 1 FROM ".$rhProjectId." WHERE uid=? LIMIT 1",$key);
    $pointjson = json_encode($val);
    //echo($pointjson);

    if(!empty($row))
    {
      query("UPDATE ".$rhProjectId." SET pointjson=? WHERE uid=?",$pointjson,$key);
      continue;
    }
    

    // write json to database
    query("INSERT INTO ".$rhProjectId." (uid,pointJson) VALUES (?,?)", str_replace('-','',$key),$pointjson);
    $counter++;  
  }
  echo("Updated ".$counter." Successfully!");
?>
