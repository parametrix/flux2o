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
      if($_POST['query']==="GET_PROJECT_LIST")
      {
        echo(json_encode($projects));
        exit();
      }
    }
    if(empty($_POST['projectId'])) // this is used for the python endpoint to retrieve the project list
    {
      echo(json_encode($projects));
      exit();
    }
    render("projectviewer.php", array("title"=>"Project Viewer", "projects"=>$projects));
  }
?>
