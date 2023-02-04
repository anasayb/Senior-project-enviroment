<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults(Two)');

    // Check the connection
    if(mysqli_connect_errno())
    {
        echo("1"); //error code #1 = connection field
        exit();
    }

    $table = $_POST["table"];
    
    $data = explode(" ",$_POST["data"]);

    foreach($data as $item){
        if($item == ""){
            continue;
        }
        $info = explode("_" , $item);
        $name = $info[0];
        $time0 = $info[1];
        $time1 = "";
        if($name != "Overall#AVG#Waiting#time"){
            $time1 = $info[2];
        }
        $streat = "";
        $turning0 = "";
        $turning1 = "";
        if($name != "Overall#AVG#Waiting#time" || $name != "AVG#Waiting#time" || $name != "Flow#rate" || $name != "Congestion#north" || $name != "Congestion#west" || $name != "Congestion#south" || $name != "Congestion#east" || $name != "Max#Waiting#time"){
            $streat = $info[3];
            $turning0 = $info[4];
            $turning1 = $info[5];
        }
       
        echo ($name." and ".$time." and ".$streat." and ".$turning);

        // add times to the table
        $insertQuey = "INSERT INTO `$table`(`name`, `waitingTimeIntersection0`, `waitingTimeIntersection1`, `Direction0`, `Direction1`, `Streat`) VALUES ('".$name."','". $time0."','". $time1."','". $turning0."','". $turning1."','". $streat."');";
        mysqli_query($con, $insertQuey) or die("4: Insert Failed");

    }

    
    
    echo("0");

?>