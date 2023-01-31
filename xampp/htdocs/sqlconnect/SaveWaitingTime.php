<?php

    $con = mysqli_connect('localhost', 'root', '', 'trafficLightResults');

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
        $time = $info[1];
        $streat = "";
        $turning = "";
        if($name != "AVG#Waiting#time" || $name != "Flow#rate" || $name != "Congestion#north" || $name != "Congestion#west" || $name != "Congestion#south" || $name != "Congestion#east"){
            $streat = $info[2];
            $turning = $info[3];
        }
       
        echo ($name." and ".$time." and ".$streat." and ".$turning);

        // add times to the table
        $insertQuey = "INSERT INTO `$table`(`name`, `waitingTime`, `Direction`, `Streat`) VALUES ('".$name."','". $time."', '". $turning."','". $streat."');";
        mysqli_query($con, $insertQuey) or die("4: Insert Failed");

    }

    
    
    echo("0");

?>