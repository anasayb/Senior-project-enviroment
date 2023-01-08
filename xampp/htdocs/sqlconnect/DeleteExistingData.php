<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }


    //Check if the table exist
    $table = $_POST["table"];
    $result = $con->query("SHOW TABLES LIKE '{$table}'");
    if($result->num_rows == 1)
    {
        //DO SOMETHING! IT EXISTS!
        $insertQuey = "DELETE FROM `".$table."`;";
        mysqli_query($con, $insertQuey) or die("4: deletion Failed");
    }
    else
    {
        //I can't find it, Create table

        $insertQuey = "CREATE TABLE `".$table."` (
            `name` text NOT NULL,
            `waitingTime` double NOT NULL,
            `Direction` text NOT NULL,
            `Streat` text NOT NULL
          ) ;";
          mysqli_query($con, $insertQuey) or die("4: creation Failed");
    }
    

    echo("0");

?>