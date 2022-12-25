<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }


    // Delete the Data in the table
    $insertQuey = "DELETE FROM `waitingtimes`;";
    mysqli_query($con, $insertQuey) or die("4: Insert Failed");

    echo("0");

?>