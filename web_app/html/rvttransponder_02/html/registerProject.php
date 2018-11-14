<?php
  /***************************************/
  set_include_path("../_includes");
  require_once("config.php");
  require_once("constants.php");
  /***************************************/
  
  if($_SERVER["REQUEST_METHOD"]=="POST")
  {
    //dump($_POST);
    if(empty($_POST['fileName']) || empty($_POST['uid']))
    {
      echo("FAILED_TRANSMISSION");
      exit();
    }
    
    // check if project has already been registered
    $existingProject = query("SELECT 1 FROM projects WHERE uid=?",$_POST['uid']);
    if(!empty($existingProject))
    {
      echo("MSG:PROJECT_ALREADY_EXISTS");
      exit();
    }
    
    // else insert into projects
    query("INSERT INTO projects (uid,filename) VALUES(?,?)",$_POST['uid'],$_POST['fileName']);
    echo("SUCCESS: PROJECT ".$_POST['fileName']."  REGISTERED");
    
  }
  ELSE
  {
    echo("FAILURE:POST_NOT_RECIEVED");
  }
  
?>
