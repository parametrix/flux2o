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
  
  $projectId = $data[0]['projectId'];

  // check if project is registered
  $projectRow = query("SELECT 1 FROM projects WHERE uid=?",$projectId);
  if(empty($projectRow))
  {
    echo("FAILURE:PROJECT_NOT_REGISTERED");
    exit();
  }
  // determine if table exists
  echo($projectId."\r\n");
  if(empty(query("SHOW TABLES LIKE '".$projectId."'")))
  {
    query("CREATE TABLE IF NOT EXISTS ".$projectId." (uid VARCHAR(64) NOT NULL PRIMARY KEY, category VARCHAR(24) NOT NULL, INDEX(category), levelName VARCHAR(64) NOT NULL, INDEX(levelName), rawjson TEXT, updated TIMESTAMP)");
  }
  // insert or update data
  foreach($data as $item)
  {
    $rows = query("SELECT * FROM ".$projectId." WHERE uid=? LIMIT 1", $item['uid']);
    if(empty($rows))
    {
      query("INSERT INTO ".$projectId." (uid,category,levelname,rawjson) VALUES (?,?,?,?)", $item['uid'],$item['category'],$item['levelName'],json_encode($item));
      echo('SUCCESS:INSERTED_ELEMENTS'.$item['category']."\r\n");
    }
    else
    {
      query("UPDATE ".$projectId." SET levelname=?,rawjson=? WHERE uid=?", $item['levelName'],json_encode($item),$item['uid']);
      echo('SUCCESS:UPDATED_ELEMENTS'.$item['category']."\r\n");
    }
  }
  exit();
?>
