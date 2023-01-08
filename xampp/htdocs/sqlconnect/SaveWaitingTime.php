<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }

    $table = $_POST["table"];
    $name = $_POST["name"];
    $time = $_POST["waiting_time"];
    $streat = $_POST["streat"];
    $turning = $_POST["turningDirection"];
    
    echo ($name." and ".$time." and ".$streat." and ".$turning);

    // add times to the table
    $insertQuey = "INSERT INTO `$table`(`name`, `waitingTime`, `Direction`, `Streat`) VALUES ('".$name."','". $time."', '". $turning."','". $streat."');";
    mysqli_query($con, $insertQuey) or die("4: Insert Failed");

    echo("0");

?>