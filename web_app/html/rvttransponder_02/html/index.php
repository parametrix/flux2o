<?php
  /***************************************/
  set_include_path("../_includes");
  require_once("config.php");
  /***************************************/
  $projects = query("SELECT * FROM projects");
  
  render("projectviewer.php", array("title"=>"Home", "projects"=>$projects));
?>
