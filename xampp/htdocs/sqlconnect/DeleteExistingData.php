<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults(Two)');

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
        $insertQuey = "";
        if($table != "information"){
            $insertQuey = "CREATE TABLE `".$table."` (
                `name` text NOT NULL,
                `waitingTimeIntersection0` double NOT NULL,
                `waitingTimeIntersection1` double NOT NULL,
                `Direction0` text NOT NULL,
                `Direction1` text NOT NULL,
                `Streat` text NOT NULL
            ) ;";
        }else{
            $insertQuey = "CREATE TABLE `".$table."` (
                `algorithm` text NOT NULL,
                `carNumber` INT NOT NULL,
                `startingDirection` text NOT NULL,
                `overall#avg` double NOT NULL,
                `avg0` double NOT NULL,
                `avg1` double NOT NULL,
                `flowRate0` double NOT NULL,
                `flowRate1` double NOT NULL,
                `CongestionNorth0` double NOT NULL,
                `CongestionWest0` double NOT NULL,
                `CongestionSouth0` double NOT NULL,
                `CongestionEast0` double NOT NULL,
                `CongestionNorth1` double NOT NULL,
                `CongestionWest1` double NOT NULL,
                `CongestionSouth1` double NOT NULL,
                `CongestionEast1` double NOT NULL
            ) ;";
        }
        mysqli_query($con, $insertQuey) or die("4: creation Failed");
    }
    

    echo("0");

?>