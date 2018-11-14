<?php header('Content-type: text/html; charset=utf-8'); ?>
<form class="form-signin" action="getProjectElements.php" method="post">
  <?php foreach($projects as $project): ?>
    <div>
      <input name="uid" type="hidden" value=<?php echo $project['uid']; ?>/>
      <button type="submit" class="btn"><?php echo $project['filename']; ?></button>
    </div>
  <?php endforeach; ?>
</form>
