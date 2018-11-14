<?php
  /***************************************/
  set_include_path("../_includes");
  require_once("config.php");
  /***************************************/
  if($_SERVER["REQUEST_METHOD"]=="POST")
  {
    if(empty($_POST['uid']))
    {
      echo("ERROR:EMPTY_UID");
      exit();
    }
    
    $projectRows = query("SELECT * FROM ".$_POST['uid']);
    echo("<flux2o>".json_encode($projectRows));
    exit();
  }
  echo("ERROR:NO_POST_RECEIVED");
  
?>
