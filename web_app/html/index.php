<?php
  /***************************************/
  set_include_path("rvttransponder_02/_includes");
  require_once('config.php');
  /***************************************/

  if($_SERVER["REQUEST_METHOD"]!="POST")
  {
    redirect("/rvttransponder_02/html/");
  }

  if($_SERVER["REQUEST_METHOD"]=="POST")
  {
    echo($_POST);
    if(!empty($_POST) && $_POST['query']=="GET_ALL_PROJECTS")
    {
      redirect("/rvttransponder_02/html/getAllProjectData.php");
    }
  }

?>