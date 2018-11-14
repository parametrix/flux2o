<?php
  /***************************************/
  set_include_path("../_includes");
  require_once("config.php");
  /***************************************/
  $projects = query('SELECT * FROM projects');
  if($_SERVER["REQUEST_METHOD"]=="POST")
  {
    if(!empty($_POST['query']))
    {
      if($_POST['query']=="GET_UPDATES" && !empty($_POST['uid']))
      {
        // name of rhino db
        $rhProjectId = "rhino".$_POST['uid'];
        
            
        // determine if table exists
        if(empty(query("SHOW TABLES LIKE '".$rhProjectId."'")))
        {
          echo("ERROR:TABLE_DOES_NOT_EXIST");
          exit();
        }

        $rows = query("SELECT * FROM ".$rhProjectId);
        echo("<flux2o>".json_encode($rows));
        exit();
      }
      else
      {
        echo("ERROR:INVALID_OR_EMPTY_UID");
      }
    }
  }
?>
